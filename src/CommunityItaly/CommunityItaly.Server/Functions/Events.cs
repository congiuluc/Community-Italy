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
using CommunityItaly.Services.Validations;

namespace CommunityItaly.Server
{
    public class Events
    {
        private readonly ILogger<Events> log;
        private readonly IImageService imageService;
        private readonly ICommunityService communityService;
        private readonly IArticleService eventServices;

        public Events(ILogger<Events> log, 
            IImageService imageService, 
            ICommunityService communityService,
            IArticleService eventServices)
        {
            this.log = log;
            this.imageService = imageService;
            this.communityService = communityService;
            this.eventServices = eventServices;
        }


        [FunctionName("EventGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "Event")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);

            var result = await eventServices.GetAsync(take, skip);

            return new OkObjectResult(result);
        }

        [FunctionName("EventGetById")]
        public async Task<IActionResult> GetById(
           [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "Event/{id}")] HttpRequest req,
           string id,
           ILogger log)
        {
            var result = await eventServices.GetById(id);
            return new OkObjectResult(result);
        }

        //[FunctionName("EventPost")]
        //public async Task<IActionResult> Post(
        //   [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.POST, Route = "Event")] HttpRequest req,
        //   ILogger log)
        //{
        //    var eventValidateRequest = await req.GetJsonBodyWithValidator(new EventValidator(communityService));
        //    if (!eventValidateRequest.IsValid)
        //    {
        //        log.LogError($"Invalid form data");
        //        return eventValidateRequest.ToBadRequest();
        //    }

        //    var result = await eventServices.CreateAsync(eventValidateRequest.Value);

        //    return new CreatedResult("EventRoute/{Id}", new { Id = result });
        //}

        [FunctionName("EventPut")]
        public async Task<IActionResult> Put(
          [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.PUT, Route = "Event")] HttpRequest req,
          ILogger log)
        {
            var eventValidateRequest = await req.GetJsonBodyWithValidator(new EventValidator(communityService));
            if (!eventValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return eventValidateRequest.ToBadRequest();
            }

            var result = await eventServices.CreateAsync(eventValidateRequest.Value);

            return new OkObjectResult(new { Id = result });
        }

        [FunctionName("EventDelete")]
        public async Task<IActionResult> Delete(
          [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.DELETE, Route = "Event")] HttpRequest req,
          ILogger log)
        {
            await eventServices.DeleteAsync(req.Query["Id"].ToString());
            return new OkResult();
        }


        [FunctionName("EventConfirmedGet")]
        public async Task<IActionResult> GetConfirmed(
           [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "EventConfirmed")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);

            var result = await eventServices.GetConfirmedAsync(take, skip);

            return new OkObjectResult(result);
        }
    }
}
