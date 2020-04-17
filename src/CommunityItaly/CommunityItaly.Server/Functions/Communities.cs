using CommunityItaly.Services;
using CommunityItaly.Services.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CommunityItaly.Server.Functions
{
	public class Communities
	{
        private readonly ILogger<Communities> log;
        private readonly ICommunityService communityService;

        public Communities(ILogger<Communities> log,
            ICommunityService communityService)
        {
            this.log = log;
            this.communityService = communityService;
        }


        [FunctionName("CommunityGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "Community")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);

            var result = await communityService.GetAsync(take, skip);

            return new OkObjectResult(result);
        }

        [FunctionName("CommunityGetById")]
        public async Task<IActionResult> GetById(
           [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "Community/{id}")] HttpRequest req,
           string id,
           ILogger log)
        {
            var result = await communityService.GetById(id);
            return new OkObjectResult(result);
        }

        [FunctionName("CommunityPut")]
        public async Task<IActionResult> Put(
          [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.PUT, Route = "Community")] HttpRequest req,
          ILogger log)
        {
            var communityValidateRequest = await req.GetJsonBodyWithValidator(new CommunityUpdateValidator(communityService));
            if (!communityValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return communityValidateRequest.ToBadRequest();
            }

            await communityService.UpdateAsync(communityValidateRequest.Value);

            return new OkObjectResult(new { Id = communityValidateRequest.Value.ShortName });
        }

        [FunctionName("CommunityDelete")]
        public async Task<IActionResult> Delete(
          [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.DELETE, Route = "Community")] HttpRequest req,
          ILogger log)
        {
            await communityService.DeleteAsync(req.Query["Id"].ToString());
            return new OkResult();
        }


        [FunctionName("CommunityConfirmedGet")]
        public async Task<IActionResult> GetConfirmed(
           [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "CommunityConfirmed")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);

            var result = await communityService.GetConfirmedAsync(take, skip);

            return new OkObjectResult(result);
        }

        [FunctionName("CommunitySelectGet")]
        public async Task<IActionResult> GetSelect(
          [HttpTrigger(AuthorizationLevel.Function, HttpVerbs.GET, Route = "CommunitySelect")] HttpRequest req)
        {
            var result = await communityService.GetConfirmedAsync();

            return new OkObjectResult(result);
        }
    }
}
