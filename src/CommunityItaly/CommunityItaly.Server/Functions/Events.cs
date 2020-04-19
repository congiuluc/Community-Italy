using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CommunityItaly.Services;
using CommunityItaly.Services.Validations;
using CommunityItaly.EF.Entities;
using System;

namespace CommunityItaly.Server
{
	public class Events
    {
        private readonly ILogger<Events> log;
        private readonly ICommunityService communityService;
        private readonly IEventService eventServices;

        public Events(ILogger<Events> log,
            ICommunityService communityService,
            IEventService eventServices)
        {
            this.log = log;
            this.communityService = communityService;
            this.eventServices = eventServices;
        }


        [FunctionName("EventGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "Event")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);

            var result = await eventServices.GetAsync(take, skip);

            return new OkObjectResult(result);
        }

        [FunctionName("EventGetById")]
        public async Task<IActionResult> GetById(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "Event/{id}")] HttpRequest req,
           string id,
           ILogger log)
        {
            var result = await eventServices.GetById(id);
            return new OkObjectResult(result);
        }

        [FunctionName("EventPut")]
        public async Task<IActionResult> Put(
          [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.PUT, Route = "Event")] HttpRequest req,
          ILogger log)
        {
            var eventValidateRequest = await req.GetJsonBodyWithValidator(new EventUpdateValidator(communityService, eventServices));
            if (!eventValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return eventValidateRequest.ToBadRequest();
            }

            await eventServices.UpdateAsync(eventValidateRequest.Value);

            return new OkObjectResult(new { Id = eventValidateRequest.Value.Id });
        }

        [FunctionName("EventDelete")]
        public async Task<IActionResult> Delete(
          [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.DELETE, Route = "Event")] HttpRequest req,
          ILogger log)
        {
            await eventServices.DeleteAsync(req.Query["Id"].ToString());
            return new OkResult();
        }


        [FunctionName("EventConfirmedGet")]
        public async Task<IActionResult> GetConfirmed(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "EventConfirmed")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);

            var result = await eventServices.GetConfirmedAsync(take, skip);

            return new OkObjectResult(result);
        }

        
        [FunctionName("EventReportDetail")]
        public async Task<IActionResult> GetEvetReportDetail(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "EventReportDetail")] HttpRequest req)
        {
            DateTime startDate = DateTime.ParseExact(req.Query["from"].ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(req.Query["to"].ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

            var result = await eventServices.GetConfirmedIntervalledAsync(startDate, endDate);

            return new OkObjectResult(result);
        }
        
    }
}
