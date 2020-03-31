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
            var activateSendMail = new ActivateSendMail { Event = vmUpdate, InstanceId = context.InstanceId };
            await context.CallActivityAsync(ConfirmationTask.SendMailEvent, activateSendMail);

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
        public async Task ConfirmEvent([ActivityTrigger] ActivateSendMail activateSendMail,
            [SendGrid(ApiKey = "SendGridConnections:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            var eventData = activateSendMail.Event;
            SendGridMessage message = new SendGridMessage();
            message.SetFrom(new EmailAddress(sendGridSettings.From));
            message.AddTos(adminSettings.GetMails().Select(x => new EmailAddress(x)).ToList());
            message.Subject = $"New Event submitted: {eventData.Name}";
            message.SetTemplateId(sendGridSettings.TemplateId);
            string urlConfirmation = adminSettings.BaseUrl.Replace("{instanceId}", activateSendMail.InstanceId);
            message.SetTemplateData(new MailTemplateData 
            {
                confirmurl = urlConfirmation,
                eventname = eventData.Name,
                eventstartdate = eventData.StartDate,
                eventenddate = eventData.EndDate,
                eventbuyticket = eventData.BuyTicket.ToString(),
                eventcfpurl = eventData.CFP.Url.ToString(),
                eventcfpstartdate = eventData.CFP.StartDate,
                eventcfpstartend = eventData.CFP.EndDate,
                eventcommunityname = eventData.CommunityName
            });

            
            //message.AddSubstitution("confirmurl", adminSettings.GetConfirmationLink(activateSendMail.InstanceId, eventData.Id));
            //message.AddSubstitution("eventname", eventData.Name);
            //message.AddSubstitution("eventstartdate", eventData.StartDate.ToString("G"));
            //message.AddSubstitution("eventenddate", eventData.EndDate.ToString("G"));
            //message.AddSubstitution("eventbuyticket", eventData.BuyTicket.ToString());
            //message.AddSubstitution("eventcfpurl", eventData.CFP.Url.ToString());
            //message.AddSubstitution("eventcfpstartdate", eventData.CFP.StartDate.ToString("G"));
            //message.AddSubstitution("eventcfpstartend", eventData.CFP.EndDate.ToString("G"));
            //message.AddSubstitution("eventcommunityname", eventData.CommunityName);

            await messageCollector.AddAsync(message);
        }
        #endregion

        #region [ApproveFromHttp]
        [FunctionName(ConfirmationTask.ApproveEvent)]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "ApproveEvent")] HttpRequestMessage req,
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

    public class ActivateSendMail
    {
        public EventUpdateViewModel Event { get; set; }
        public string InstanceId { get; set; }
    }

    public class MailTemplateData
    {
        public string confirmurl { get; set; }
        public string eventname { get; set; }
        public DateTime eventstartdate { get; set; }
        public DateTime eventenddate { get; set; }
        public string eventbuyticket { get; set; }
        public string eventcfpurl { get; set; }
        public DateTime eventcfpstartdate { get; set; }
        public DateTime eventcfpstartend { get; set; }
        public string eventcommunityname { get; set; }
    }
}

/*
 
SendGrid Template
{
"confirmurl":"https://cloudgen.it",
"eventname":"Global Azure",
"eventstartdate":"2020-04-28",
"eventenddate":"2020-04-28",
"eventbuyticket":"https://cloudgen.it",
"eventcfpurl":"https://cloudgen.it",
"eventcfpstartdate":"2020-01-02",
"eventcfpstartend":"2020-02-29",
"eventcommunityname":"CloudGen Verona"
}
 */
