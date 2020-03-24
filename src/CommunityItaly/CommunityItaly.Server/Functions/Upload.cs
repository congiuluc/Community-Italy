using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityItaly.Server.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CommunityItaly.Server.Functions
{
    public class Upload
    {
        [FunctionName("Upload_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
          [DurableClient] IDurableOrchestrationClient starter,
          ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Upload", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }


        [FunctionName("Upload")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("Upload_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("Upload_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("Upload_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName(UploadTasks.CreateMiniature)]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

      
    }
}