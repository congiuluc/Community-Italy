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
    public class ConfirmationArticle
    {
        private readonly IArticleService articleService;
        private readonly SendGridConnections sendGridSettings;
        private readonly AdminConfiguration adminSettings;

        public ConfirmationArticle(IArticleService articleService,
            IOptions<SendGridConnections> sendGridSettings,
            IOptions<AdminConfiguration> adminSettings)
        {
            this.articleService = articleService;
            this.sendGridSettings = sendGridSettings.Value;
            this.adminSettings = adminSettings.Value;
        }

        #region [Start]
        [FunctionName(ConfirmationTask.ConfirmArticle_Http)]
        public async Task<IActionResult> ConfirmArticleHttpStart(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.POST, Route = "Article")] HttpRequest req,
           [DurableClient] IDurableOrchestrationClient starter,
           ILogger log)
        {
            var eventValidateRequest = await req.GetJsonBodyWithValidator(new ArticleValidator(articleService));
            if (!eventValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return eventValidateRequest.ToBadRequest();
            }

            string instanceId = await starter.StartNewAsync(ConfirmationTask.ConfirmOrchestratorArticle, null, eventValidateRequest.Value);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
        #endregion

        #region [Orchestrator]
        [FunctionName(ConfirmationTask.ConfirmOrchestratorArticle)]
        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var vm = context.GetInput<ArticleViewModel>();
            string id = await context.CallActivityAsync<string>(ConfirmationTask.CreateArticle, vm);

            var vmUpdate = ArticleUpdateViewModel.Create(vm);
            vmUpdate.Id = id;
            var activateSendMail = new ActivateArticleSendMail { Data = vmUpdate, InstanceId = context.InstanceId };
            await context.CallActivityAsync(ConfirmationTask.SendMailArticle, activateSendMail);

            await HumanInteractionArticle(context, vmUpdate);
            return id;
        }
        #endregion

        #region [ArticleToOrchestrate]
        [FunctionName(ConfirmationTask.CreateArticle)]
        public async Task<string> CreateArticle([ActivityTrigger] ArticleViewModel eventData, ILogger log)
        {
            string result = await articleService.CreateAsync(eventData);
            return result;
        }

        [FunctionName(ConfirmationTask.SendMailArticle)]
        public async Task ConfirmArticle([ActivityTrigger] ActivateArticleSendMail activateSendMail,
            [SendGrid(ApiKey = "SendGridConnections:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            var articleData = activateSendMail.Data;
            SendGridMessage message = new SendGridMessage();
            message.SetFrom(new EmailAddress(sendGridSettings.From));
            message.AddTos(adminSettings.GetMails().Select(x => new EmailAddress(x)).ToList());
            message.SetSubject($"New Article submitted: {articleData.Name}");
            message.SetTemplateId(sendGridSettings.TemplateArticleId);
            message.SetTemplateData(new MailArticleTemplateData
            {
                confirmurl = adminSettings.GetConfirmationArticleLink(activateSendMail.InstanceId, true),
                aborturl = adminSettings.GetConfirmationArticleLink(activateSendMail.InstanceId, false),
                articlename = articleData.Name,
                articleurl = articleData.Url.ToString(),
                articlepublishdate = articleData.PublishDate,
                articleauthors = articleData.Authors.Select(t => new MailArticlePersonTemplateData { name = t.Name, surname = t.Surname}).ToList()
            });
            await messageCollector.AddAsync(message);
        }

        [FunctionName(ConfirmationTask.ApproveCancelArticleOnCosmos)]
        public async Task ApproveCancelArticleOnCosmos([ActivityTrigger] ArticleUpdateViewModel vm, ILogger log)
        {
            await articleService.UpdateAsync(vm).ConfigureAwait(false);
        }
        #endregion

        #region [ApproveFromHttp]
        [FunctionName(ConfirmationTask.ApproveFromHttpArticle)]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "ApproveArticle")] HttpRequestMessage req,
             [DurableClient] IDurableOrchestrationClient client,
             ILogger logger)
        {
            var keys = req.RequestUri.ParseQueryString();
            var approveResponse = new ApproveElement
            {
                InstanceId = keys.Get("InstanceId"),
                Result = bool.Parse(keys.Get("ApproveValue"))
            };
            await client.RaiseEventAsync(approveResponse.InstanceId, ConfirmationTask.ConfirmArticleHuman, approveResponse.Result);
            return new AcceptedResult();
        }
        #endregion

        #region [WaitHumanInteraction]
        public async Task<bool> HumanInteractionArticle(IDurableOrchestrationContext context, ArticleUpdateViewModel vm)
        {
            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime expiration = context.CurrentUtcDateTime.AddDays(6);
                Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

                bool authorized = false;
                for (int retryCount = 0; retryCount <= 3; retryCount++)
                {
                    Task<bool> challengeResponseTask = context.WaitForExternalEvent<bool>(ConfirmationTask.ConfirmArticleHuman);

                    Task winner = await Task.WhenAny(challengeResponseTask, timeoutTask);
                    if (winner == challengeResponseTask)
                    {
                        // We got back a response! Compare it to the challenge code.
                        if (challengeResponseTask.Result)
                        {
                            vm.Confirmed = challengeResponseTask.Result;
                            await context.CallActivityAsync(ConfirmationTask.ApproveCancelArticleOnCosmos, vm);
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

    public class MailArticleTemplateData
    {
        public string confirmurl { get; set; }
        public string aborturl { get; set; }
        public string articlename { get; set; }
        public string articleurl { get; set; }
        public DateTime articlepublishdate { get; set; }
        public List<MailArticlePersonTemplateData> articleauthors { get; set; }
    }

    public class MailArticlePersonTemplateData
    {
        public string name { get; set; }
        public string surname { get; set; }
    }
}
