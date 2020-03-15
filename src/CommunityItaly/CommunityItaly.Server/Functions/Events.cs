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
using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Shared.Validations;

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
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "EventRoute")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);


            return new OkResult();
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
            var eventValidateRequest = await req.GetJsonBody<EventViewModel, EventValidator>();
            if (!eventValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return eventValidateRequest.ToBadRequest();
            }

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
