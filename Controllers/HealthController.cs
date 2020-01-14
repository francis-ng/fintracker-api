using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancialTrackerApi.Controllers
{
    [Route("/")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return Ok();
        }
    }
}