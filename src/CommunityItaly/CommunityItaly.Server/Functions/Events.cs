using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CommunityItaly.Services;

namespace CommunityItaly.Server
{
    public class Events
    {
        private readonly ILogger<Events> log;
        private readonly IImageService imageService;

        public Events(ILogger<Events> log, IImageService imageService)
        {
            this.log = log;
            this.imageService = imageService;
        }


        [FunctionName("EventGet")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "EventRoute")] HttpRequest req)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");
            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(new { method = req.Method });
        }

        [FunctionName("EventGetById")]
        public static async Task<IActionResult> GetById(
           [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "EventRoute/{id}")] HttpRequest req,
           string id,
           ILogger log)
        {
            return new OkObjectResult(new { method = req.Method, Parameter = id });
        }

        [FunctionName("EventPost")]
        public static async Task<IActionResult> Post(
           [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.POST, Route = "EventRoute")] HttpRequest req,
           ILogger log)
        {
            return new OkObjectResult(new { method = req.Method });
        }

        [FunctionName("EventPut")]
        public static async Task<IActionResult> Put(
          [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.PUT, Route = "EventRoute")] HttpRequest req,
          ILogger log)
        {
            return new OkObjectResult(new { method = req.Method });
        }

        [FunctionName("EventDelete")]
        public static async Task<IActionResult> Delete(
          [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.DELETE, Route = "EventRoute")] HttpRequest req,
          ILogger log)
        {
            return new OkObjectResult(new { method = req.Method });
        }
    }
}
