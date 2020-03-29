using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityItaly.Server.Utilities;
using CommunityItaly.Services;
using CommunityItaly.Services.Settings;
using CommunityItaly.Services.Validations;
using CommunityItaly.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace CommunityItaly.Server.Functions
{
    public class ConfirmationEvent
    {
        private readonly IEventService eventService;
        private readonly ICommunityService communityService;
        private readonly SendGridConnections sendGridSettings;

        public ConfirmationEvent(IEventService eventService, ICommunityService communityService, SendGridConnections sendGridSettings)
        {
            this.eventService = eventService;
            this.communityService = communityService;
            this.sendGridSettings = sendGridSettings;
        }

		#region [Start]
		[FunctionName(ConfirmationTask.ConfirmEvent_Http)]
        public async Task<IActionResult> ConfirmEventHttpStart(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.POST, Route = "Event")] HttpRequest req,
           [DurableClient] IDurableOrchestrationClient starter,
           ILogger log)
        {
            var eventValidateRequest = await req.GetJsonBodyWithValidator(new EventValidator(communityService));
            if (!eventValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return eventValidateRequest.ToBadRequest();
            }

            string instanceId = await starter.StartNewAsync(ConfirmationTask.ConfirmOrchestrator, null, eventValidateRequest.Value);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
        #endregion

        #region [Orchestrator]
        [FunctionName(ConfirmationTask.ConfirmOrchestrator)]
        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var vm = context.GetInput<EventViewModel>();
            string id = await context.CallActivityAsync<string>(ConfirmationTask.CreateEvent, vm);

            var vmUpdate = EventUpdateViewModel.Create(vm);
            vmUpdate.Id = id;
            await context.CallActivityAsync(ConfirmationTask.SendMailEvent, vmUpdate);

            await HumanInteractionEvent(context, vmUpdate);
            return id;
        }
		#endregion

		#region [EventToOrchestrate]
		[FunctionName(ConfirmationTask.CreateEvent)]
        public async Task<string> CreateEvent([ActivityTrigger] EventViewModel eventData, ILogger log)
        {
            string result = await eventService.CreateAsync(eventData);
            return result;
        }

        [FunctionName(ConfirmationTask.SendMailEvent)]
        public async Task ConfirmEvent([ActivityTrigger] EventUpdateViewModel eventData,
            [SendGrid(ApiKey = "SendGrid:ApiKey")] SendGridMessage message,
            ILogger log)
        {
            message.SetTemplateId(sendGridSettings.TemplateId);
        }
        #endregion

        #region [ApproveFromHttp]
        [FunctionName(ConfirmationTask.ApproveEvent)]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.POST, Route = "ApproveEvent")] HttpRequestMessage req,
             [DurableClient] IDurableOrchestrationClient client,
             ILogger logger)
        {
            var approveResponse = await req.Content.ReadAsAsync<ApproveElement>();
            await client.RaiseEventAsync(approveResponse.InstanceId, ConfirmationTask.ConfirmEventHuman, approveResponse.Result);
            return new AcceptedResult();
        }
        #endregion

        #region [WaitHumanInteraction]
        public async Task<bool> HumanInteractionEvent(IDurableOrchestrationContext context, EventUpdateViewModel vm)
        {
            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime expiration = context.CurrentUtcDateTime.AddMonths(1);
                Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

                bool authorized = false;
                for (int retryCount = 0; retryCount <= 3; retryCount++)
                {
                    Task<bool> challengeResponseTask = context.WaitForExternalEvent<bool>(ConfirmationTask.ConfirmEventHuman);

                    Task winner = await Task.WhenAny(challengeResponseTask, timeoutTask);
                    if (winner == challengeResponseTask)
                    {
                        // We got back a response! Compare it to the challenge code.
                        if (challengeResponseTask.Result)
                        {
                            authorized = true;
                            vm.Confirmation = challengeResponseTask.Result;
                            await eventService.UpdateAsync(vm);
                            break;
                        }
                    }
                    else
                    {
                        // Timeout expired
                        break;
                    }
                }

                if (!timeoutTask.IsCompleted)
                {
                    // All pending timers must be complete or canceled before the function exits.
                    timeoutCts.Cancel();
                }

                return authorized;
            }
        }
		#endregion
	}
}