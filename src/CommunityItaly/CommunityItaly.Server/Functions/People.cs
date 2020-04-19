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
	public class People
	{
        private readonly ILogger<People> log;
        private readonly ICommunityService communityService;
        private readonly IPersonService personService;

        public People(ILogger<People> log,
            ICommunityService communityService,
            IPersonService personService)
        {
            this.log = log;
            this.communityService = communityService;
            this.personService = personService;
        }


        [FunctionName("PersonGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "Person")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);

            var result = await personService.GetAsync(take, skip);

            return new OkObjectResult(result);
        }

        [FunctionName("PersonGetById")]
        public async Task<IActionResult> GetById(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "Person/{id}")] HttpRequest req,
           string id,
           ILogger log)
        {
            var result = await personService.GetById(id);
            return new OkObjectResult(result);
        }

        [FunctionName("PersonPut")]
        public async Task<IActionResult> Put(
          [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.PUT, Route = "Person")] HttpRequest req,
          ILogger log)
        {
            var personValidateRequest = await req.GetJsonBodyWithValidator(new PersonUpdateValidator(personService));
            if (!personValidateRequest.IsValid)
            {
                log.LogError($"Invalid form data");
                return personValidateRequest.ToBadRequest();
            }

            await personService.UpdateAsync(personValidateRequest.Value);

            return new OkObjectResult(new { Id = personValidateRequest.Value.Id });
        }

        [FunctionName("PersonDelete")]
        public async Task<IActionResult> Delete(
          [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.DELETE, Route = "Person")] HttpRequest req,
          ILogger log)
        {
            await personService.DeleteAsync(req.Query["Id"].ToString());
            return new OkResult();
        }


        [FunctionName("PersonConfirmedGet")]
        public async Task<IActionResult> GetConfirmed(
           [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "PersonConfirmed")] HttpRequest req)
        {
            int take = 10, skip = 0;
            _ = int.TryParse(req.Query["take"].ToString(), out take);
            _ = int.TryParse(req.Query["skip"].ToString(), out skip);

            var result = await personService.GetConfirmedAsync(take, skip);

            return new OkObjectResult(result);
        }

        [FunctionName("PersonSelectGet")]
        public async Task<IActionResult> GetSelect(
          [HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.GET, Route = "PersonSelect")] HttpRequest req)
        {
            var result = await personService.GetSelectAsync();

            return new OkObjectResult(result);
        }
    }
}
