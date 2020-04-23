using System;
using System.Linq;
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
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;

namespace CommunityItaly.Server.Functions
{
    public class ConfirmationEvent
    {
        private readonly IEventService eventService;
        private readonly ICommunityService communityService;
        private readonly SendGridConnections sendGridSettings;
        private readonly AdminConfiguration adminSettings;

        public ConfirmationEvent(IEventService eventService, 
            ICommunityService communityService,
            IOptions<SendGridConnections> sendGridSettings,
            IOptions<AdminConfiguration> adminSettings)
        {
            this.eventService = eventService;
            this.communityService = communityService;
            this.sendGridSettings = sendGridSettings.Value;
            this.adminSettings = adminSettings.Value;
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
            EventViewModel vm = eventValidateRequest.Value;
            string instanceId = await starter.StartNewAsync(ConfirmationTask.ConfirmOrchestratorEvent, null, vm);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
        #endregion

        #region [Orchestrator]
        [FunctionName(ConfirmationTask.ConfirmOrchestratorEvent)]
        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var vm = context.GetInput<EventViewModel>();
            string id = await context.CallActivityAsync<string>(ConfirmationTask.CreateEvent, vm);
            var activateSendMail = new ActivateEventSendMail { Data = vm, InstanceId = context.InstanceId };
            await context.CallActivityAsync(ConfirmationTask.SendMailEvent, activateSendMail);

            await HumanInteractionEvent(context, vm);
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
        public async Task ConfirmEvent([ActivityTrigger] ActivateEventSendMail activateSendMail,
            [SendGrid(ApiKey = "SendGridConnections:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            var eventData = activateSendMail.Data;
            SendGridMessage message = new SendGridMessage();
            message.SetFrom(new EmailAddress(sendGridSettings.From));
            message.AddTos(adminSettings.GetMails().Select(x => new EmailAddress(x)).ToList());
            message.SetSubject($"New Event submitted: {eventData.Name}");
            message.SetTemplateId(sendGridSettings.TemplateEventId);
            message.SetTemplateData(new MailEventTemplateData 
            {
                confirmurl = adminSettings.GetConfirmationEventLink(activateSendMail.InstanceId, true),
                aborturl = adminSettings.GetConfirmationEventLink(activateSendMail.InstanceId, false),
                eventname = eventData.Name,
                eventstartdate = eventData.StartDate,
                eventenddate = eventData.EndDate,
                eventbuyticket = eventData.BuyTicket?.ToString(),
                eventcfpurl = eventData.CFP?.Url,
                eventcfpstartdate = eventData.CFP?.StartDate,
                eventcfpstartend = eventData.CFP?.EndDate,
                eventcommunityname = eventData.CommunityName
            });
            await messageCollector.AddAsync(message);
        }

        [FunctionName(ConfirmationTask.ApproveCancelEventOnCosmos)]
        public async Task ApproveCancelEventOnCosmos([ActivityTrigger] EventViewModel vm, ILogger log)
        {
            await eventService.UpdateAsync(vm).ConfigureAwait(false);
        }
        #endregion

        #region [ApproveFromHttp]
        [FunctionName(ConfirmationTask.ApproveFromHttpEvent)]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "ApproveEvent")] HttpRequestMessage req,
             [DurableClient] IDurableOrchestrationClient client,
             ILogger logger)
        {
            var keys = req.RequestUri.ParseQueryString();
            var approveResponse = new ApproveElement
            {
                InstanceId = keys.Get("InstanceId"),
                Result = bool.Parse(keys.Get("ApproveValue"))
            };
            await client.RaiseEventAsync(approveResponse.InstanceId, ConfirmationTask.ConfirmEventHuman, approveResponse.Result);
            return new AcceptedResult();
        }
        #endregion

        #region [WaitHumanInteraction]
        public async Task<bool> HumanInteractionEvent(IDurableOrchestrationContext context, EventViewModel vm)
        {
            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime expiration = context.CurrentUtcDateTime.AddDays(6);
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
                            vm.Confirmed = challengeResponseTask.Result;
                            await context.CallActivityAsync(ConfirmationTask.ApproveCancelEventOnCosmos, vm);
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