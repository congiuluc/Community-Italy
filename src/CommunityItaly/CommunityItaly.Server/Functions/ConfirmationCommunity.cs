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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityItaly.Server.Functions
{
    public class ConfirmationCommunity
    {
        private readonly ICommunityService communityService;
        private readonly SendGridConnections sendGridSettings;
        private readonly AdminConfiguration adminSettings;

        public ConfirmationCommunity(ICommunityService communityService,
            IOptions<SendGridConnections> sendGridSettings,
            IOptions<AdminConfiguration> adminSettings)
        {
            this.communityService = communityService;
            this.sendGridSettings = sendGridSettings.Value;
            this.adminSettings = adminSettings.Value;
        }

        #region [Start]
        [FunctionName(ConfirmationTask.ConfirmCommunity_Http)]
        public async Task<IActionResult> ConfirmCommuntyHttpStart(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.POST, Route = "Community")] HttpRequest req,
           [DurableClient] IDurableOrchestrationClient starter,
           ILogger log)
        {
            var communityValidateRequest = await req.GetJsonBodyWithValidator(new CommunityValidator(communityService));
            if (!communityValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return communityValidateRequest.ToBadRequest();
            }

            string instanceId = await starter.StartNewAsync(ConfirmationTask.ConfirmOrchestratorCommunity, null, communityValidateRequest.Value);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
        #endregion

        #region [Orchestrator]
        [FunctionName(ConfirmationTask.ConfirmOrchestratorCommunity)]
        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var vm = context.GetInput<CommunityViewModel>();
            string id = await context.CallActivityAsync<string>(ConfirmationTask.CreateCommunity, vm);

            var vmUpdate = CommunityUpdateViewModel.Create(vm);
            vmUpdate.Id = id;
            var activateSendMail = new ActivateCommunitySendMail { Data = vmUpdate, InstanceId = context.InstanceId };
            await context.CallActivityAsync(ConfirmationTask.SendMailCommunity, activateSendMail);

            await HumanInteractionCommunity(context, vmUpdate);
            return id;
        }
        #endregion

        #region [CommunityToOrchestrate]
        [FunctionName(ConfirmationTask.CreateCommunity)]
        public async Task<string> CreateCommunity([ActivityTrigger] CommunityViewModel communityData, ILogger log)
        {
            string result = await communityService.CreateAsync(communityData);
            return result;
        }

        [FunctionName(ConfirmationTask.SendMailCommunity)]
        public async Task ConfirmCommunity([ActivityTrigger] ActivateCommunitySendMail activateSendMail,
            [SendGrid(ApiKey = "SendGridConnections:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            var communityData = activateSendMail.Data;
            SendGridMessage message = new SendGridMessage();
            message.SetFrom(new EmailAddress(sendGridSettings.From));
            message.AddTos(adminSettings.GetMails().Select(x => new EmailAddress(x)).ToList());
            message.SetSubject($"New Community submitted: {communityData.Name}");
            message.SetTemplateId(sendGridSettings.TemplateCommunityId);
            message.SetTemplateData(new MailCommunityTemplateData
            {
                confirmurl = adminSettings.GetConfirmationCommunityLink(activateSendMail.InstanceId, true),
                aborturl = adminSettings.GetConfirmationCommunityLink(activateSendMail.InstanceId, false),
                communityname = communityData.Name,
                communitywebsite = communityData.WebSite.ToString(),
                communitymanagers = communityData.Managers.Select(t => new PersonTemplate { name = t.Name, surname = t.Surname }).ToList()
            });
            await messageCollector.AddAsync(message);
        }

        [FunctionName(ConfirmationTask.ApproveCancelCommunityOnCosmos)]
        public async Task ApproveCancelCommunityOnCosmos([ActivityTrigger] CommunityUpdateViewModel vm, ILogger log)
        {
            await communityService.UpdateAsync(vm).ConfigureAwait(false);
        }
        #endregion

        #region [ApproveFromHttp]
        [FunctionName(ConfirmationTask.ApproveFromHttpCommunity)]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "ApproveCommunity")] HttpRequestMessage req,
             [DurableClient] IDurableOrchestrationClient client,
             ILogger logger)
        {
            var keys = req.RequestUri.ParseQueryString();
            var approveResponse = new ApproveElement
            {
                InstanceId = keys.Get("InstanceId"),
                Result = bool.Parse(keys.Get("ApproveValue"))
            };
            await client.RaiseEventAsync(approveResponse.InstanceId, ConfirmationTask.ConfirmCommunityHuman, approveResponse.Result);
            return new AcceptedResult();
        }
        #endregion

        #region [WaitHumanInteraction]
        public async Task<bool> HumanInteractionCommunity(IDurableOrchestrationContext context, CommunityUpdateViewModel vm)
        {
            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime expiration = context.CurrentUtcDateTime.AddDays(6);
                Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

                bool authorized = false;
                for (int retryCount = 0; retryCount <= 3; retryCount++)
                {
                    Task<bool> challengeResponseTask = context.WaitForExternalEvent<bool>(ConfirmationTask.ConfirmCommunityHuman);

                    Task winner = await Task.WhenAny(challengeResponseTask, timeoutTask);
                    if (winner == challengeResponseTask)
                    {
                        // We got back a response! Compare it to the challenge code.
                        if (challengeResponseTask.Result)
                        {
                            vm.Confirmed = challengeResponseTask.Result;
                            await context.CallActivityAsync(ConfirmationTask.ApproveCancelCommunityOnCosmos, vm);
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
