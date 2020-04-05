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

    public class ConfirmationPerson
    {
        private readonly IPersonService personService;
        private readonly SendGridConnections sendGridSettings;
        private readonly AdminConfiguration adminSettings;

        public ConfirmationPerson(IPersonService personService,
            IOptions<SendGridConnections> sendGridSettings,
            IOptions<AdminConfiguration> adminSettings)
        {
            this.personService = personService;
            this.sendGridSettings = sendGridSettings.Value;
            this.adminSettings = adminSettings.Value;
        }

        #region [Start]
        [FunctionName(ConfirmationTask.ConfirmPerson_Http)]
        public async Task<IActionResult> ConfirmPersonHttpStart(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.POST, Route = "Person")] HttpRequest req,
           [DurableClient] IDurableOrchestrationClient starter,
           ILogger log)
        {
            var personValidateRequest = await req.GetJsonBodyWithValidator(new PersonValidator(personService));
            if (!personValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return personValidateRequest.ToBadRequest();
            }

            string instanceId = await starter.StartNewAsync(ConfirmationTask.ConfirmOrchestratorPerson, null, personValidateRequest.Value);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
        #endregion

        #region [Orchestrator]
        [FunctionName(ConfirmationTask.ConfirmOrchestratorPerson)]
        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var vm = context.GetInput<PersonViewModel>();
            string id = await context.CallActivityAsync<string>(ConfirmationTask.CreatePerson, vm);

            var vmUpdate = PersonUpdateViewModel.Create(vm);
            vmUpdate.Id = id;
            var activateSendMail = new ActivatePersonSendMail { Data = vmUpdate, InstanceId = context.InstanceId };
            await context.CallActivityAsync(ConfirmationTask.SendMailPerson, activateSendMail);

            await HumanInteractionPerson(context, vmUpdate);
            return id;
        }
        #endregion

        #region [PersonToOrchestrate]
        [FunctionName(ConfirmationTask.CreatePerson)]
        public async Task<string> CreatePerson([ActivityTrigger] PersonViewModel personData, ILogger log)
        {
            string result = await personService.CreateAsync(personData);
            return result;
        }

        [FunctionName(ConfirmationTask.SendMailPerson)]
        public async Task ConfirmPerson([ActivityTrigger] ActivatePersonSendMail activateSendMail,
            [SendGrid(ApiKey = "SendGridConnections:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            var personData = activateSendMail.Data;
            SendGridMessage message = new SendGridMessage();
            message.SetFrom(new EmailAddress(sendGridSettings.From));
            message.AddTos(adminSettings.GetMails().Select(x => new EmailAddress(x)).ToList());
            message.SetSubject($"New Person submitted: {personData.Name}");
            message.SetTemplateId(sendGridSettings.TemplatePersonId);
            message.SetTemplateData(new MailPersonTemplateData
            {
                confirmurl = adminSettings.GetConfirmationPersonLink(activateSendMail.InstanceId, true),
                aborturl = adminSettings.GetConfirmationPersonLink(activateSendMail.InstanceId, false),
                personname = personData.Name,
                personsurname = personData.Surname,
                personmvpcode = personData.MVP_Code
            });
            await messageCollector.AddAsync(message);
        }

        [FunctionName(ConfirmationTask.ApproveCancelPersonOnCosmos)]
        public async Task ApproveCancelPersonOnCosmos([ActivityTrigger] PersonUpdateViewModel vm, ILogger log)
        {
            await personService.UpdateAsync(vm).ConfigureAwait(false);
        }
        #endregion

        #region [ApproveFromHttp]
        [FunctionName(ConfirmationTask.ApproveFromHttpPerson)]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "ApprovePerson")] HttpRequestMessage req,
             [DurableClient] IDurableOrchestrationClient client,
             ILogger logger)
        {
            var keys = req.RequestUri.ParseQueryString();
            var approveResponse = new ApproveElement
            {
                InstanceId = keys.Get("InstanceId"),
                Result = bool.Parse(keys.Get("ApproveValue"))
            };
            await client.RaiseEventAsync(approveResponse.InstanceId, ConfirmationTask.ConfirmPersonHuman, approveResponse.Result);
            return new AcceptedResult();
        }
        #endregion

        #region [WaitHumanInteraction]
        public async Task<bool> HumanInteractionPerson(IDurableOrchestrationContext context, PersonUpdateViewModel vm)
        {
            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime expiration = context.CurrentUtcDateTime.AddDays(6);
                Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

                bool authorized = false;
                for (int retryCount = 0; retryCount <= 3; retryCount++)
                {
                    Task<bool> challengeResponseTask = context.WaitForExternalEvent<bool>(ConfirmationTask.ConfirmPersonHuman);

                    Task winner = await Task.WhenAny(challengeResponseTask, timeoutTask);
                    if (winner == challengeResponseTask)
                    {
                        // We got back a response! Compare it to the challenge code.
                        if (challengeResponseTask.Result)
                        {
                            vm.Confirmed = challengeResponseTask.Result;
                            await context.CallActivityAsync(ConfirmationTask.ApproveCancelPersonOnCosmos, vm);
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
