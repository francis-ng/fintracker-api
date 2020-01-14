using FinancialTrackerApi.Models;
using FinancialTrackerApi.Services;
using FinancialTrackerApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FinancialTrackerApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LedgerController : ControllerBase
    {
        private readonly LedgerService _ledgerService;
        private readonly ILogger _logger;

        public LedgerController(LedgerService ledgerService, ILogger<LedgerController> logger)
        {
            _ledgerService = ledgerService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<List<MonthlyLedger>> List()
        {
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Listing data for user {User}", owner);

            var ledgers = _ledgerService.Get(owner);

            if (ledgers == null || ledgers.Count == 0) {
                _logger.LogWarning("Data for user {User} not found", owner);
                return NotFound();
            }

            return ledgers;
        }

        [HttpGet(Name = "GetLedger")]
        [Authorize]
        public ActionResult<MonthlyLedger> Get([FromQuery]int year, [FromQuery]int month)
        {
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Getting data for user {User}, {Year}/{Month}", owner, year, month);

            var ledger = _ledgerService.Get(owner, year, month);

            if (ledger == null) {
                _logger.LogWarning("Data for user {User}, {Year}/{Month} not found", owner, year, month);
                return NotFound();
            }

            return ledger;
        }

        [HttpPost]
        [Authorize]
        public ActionResult<MonthlyLedger> Add(MonthlyLedger ledger)
        {
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Adding data for user {User}", owner);

            ledger.Owner = owner;
            _ledgerService.Add(ledger);

            return CreatedAtRoute("GetLedger", new 
                {owner = ledger.Owner, year = ledger.Year.ToString(), month = ledger.Month.ToString() }
                , ledger);
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update([FromQuery]string id, MonthlyLedger inLedger)
        {
            var ledger = _ledgerService.Find(id);
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Updating id {Id} for user {User}", owner);

            if (ledger == null) {
                _logger.LogWarning("Id {Id} for user {User} not found", id, owner);
                return NotFound();
            }

            _ledgerService.Update(id, inLedger);

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete(string id)
        {
            var ledger = _ledgerService.Find(id);
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Deleting id {Id} for user {User}", owner);

            if (ledger == null) {
                _logger.LogWarning("Id {Id} for user {User} not found", id, owner);
                return NotFound();
            }

            _ledgerService.Delete(id);

            return NoContent();
        }
    }
}