using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityItaly.Services;
using CommunityItaly.Services.FlatFiles;
using CommunityItaly.Services.FolderStructures;
using CommunityItaly.Services.Settings;
using CommunityItaly.Shared.ViewModels;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;

namespace CommunityItaly.Server.Functions
{
    public class ReportEvents
    {
        public const string ReportEvents_Generate = nameof(ReportEvents_Generate);
        public const string ReportEvents_Orchestrator = nameof(ReportEvents_Orchestrator);
        public const string ReportEvents_HttpStart = nameof(ReportEvents_HttpStart);
        public const string ReportEvents_SendMail = nameof(ReportEvents_SendMail);


        private readonly IEventService eventService;
        private readonly IFlatFileService flatFileService;
        private readonly IFileService fileService;
        private readonly SendGridConnections sendGridSettings;
        private readonly AdminConfiguration adminSettings;

        public ReportEvents(IEventService eventService, 
            IOptions<SendGridConnections> sendGridSettings,
            IOptions<AdminConfiguration> adminSettings,
            IFlatFileService flatFileService,
            IFileService fileService)
        {
            this.eventService = eventService;
            this.flatFileService = flatFileService;
            this.fileService = fileService;
            this.sendGridSettings = sendGridSettings.Value;
            this.adminSettings = adminSettings.Value;
        }

        [FunctionName(ReportEvents_Orchestrator)]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var filter = context.GetInput<ReportFilter>();
            // Set Filter FileName
            filter.FileName = $"Report-{context.CurrentUtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")}";
            ReportInformation reportInformation = await context.CallActivityAsync<ReportInformation>(ReportEvents_Generate, filter);
            MemoryStream eventsStream = await fileService.DownloadReport(reportInformation.BlobContainerName, reportInformation.FileName);
            ReportGeneration generation = new ReportGeneration
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                EventsReportStream = eventsStream
            };
            await context.CallActivityAsync(ReportEvents_SendMail, generation);
        }

        [FunctionName(ReportEvents_Generate)]
        public async Task<ReportInformation> ReportGeneration([ActivityTrigger] ReportFilter filter, ILogger log)
        {
            ReportInformation reportInformation = ReportStructure.Report(filter.FileName);
            ICollection<EventViewModelReadOnly> vm = await eventService.GetConfirmedIntervalledAsync(filter.StartDate, filter.EndDate);
            var dataStream = flatFileService.GenerateEventFlatFile(vm);
            await fileService.UploadReport(reportInformation.BlobContainerName, reportInformation.FileName, dataStream);
            return reportInformation;
        }

        [FunctionName(ReportEvents_SendMail)]
        public async Task ConfirmEvent([ActivityTrigger] ReportGeneration eventsData,
            [SendGrid(ApiKey = "SendGridConnections:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            SendGridMessage message = new SendGridMessage();
            message.SetFrom(new EmailAddress(sendGridSettings.From));
            message.AddTos(adminSettings.GetMails().Select(x => new EmailAddress(x)).ToList());
            message.SetSubject($"Send events report from {eventsData.StartDate.ToShortDateString()} to {eventsData.EndDate.ToShortDateString()}");
            await message.AddAttachmentAsync("EventReport", eventsData.EventsReportStream);          
            await messageCollector.AddAsync(message);
        }

        [FunctionName(ReportEvents_HttpStart)]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.POST, Route = "ReportEvents")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            ReportFilter filter = await JsonSerializer.DeserializeAsync<ReportFilter>(await req.Content.ReadAsStreamAsync());
            string instanceId = await starter.StartNewAsync("ReportEvents", null, filter);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }

    public class ReportFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FileName { get; set; }
    }

    public class ReportGeneration : ReportFilter
    {
        public MemoryStream EventsReportStream { get; set; }
    }
}