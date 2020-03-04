using FinancialTrackerApi.Models;
using FinancialTrackerApi.Services;
using FinancialTrackerApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<List<ListItem>>> ListAsync()
        {
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Listing data for user {User}", owner);

            var ledgers = await _ledgerService.Get(owner);

            if (ledgers == null || ledgers.Count == 0) {
                _logger.LogWarning("Data for user {User} not found", owner);
                return NotFound();
            }

            return ledgers.Select(x => {
                return new ListItem()
                {
                    Year = x.Year,
                    Month = x.Month,
                    Type = x.Type
                };
            }).ToList();
        }

        [HttpGet(Name = "GetLedger")]
        [Authorize]
        public async Task<ActionResult<MonthlyLedger>> GetAsync([FromQuery]int year, [FromQuery]int month)
        {
            var owner = Auth.GetUser(User.Claims);
            MonthlyLedger ledger = null;
            var type = "regular";
            
            if (year == 0 && month == 0)
            {
                type = "fixed";
                
            }

            _logger.LogInformation("Getting data for user {User}, {Year}/{Month}", owner, year, month);

            ledger = await _ledgerService.Get(owner, year, month, type);

            if (ledger == null)
            {
                _logger.LogWarning("Data for user {User}, {Year}/{Month} not found", owner, year, month);
                return NotFound();
            }

            return ledger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MonthlyLedger>> AddAsync(MonthlyLedger ledger)
        {
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Adding data for user {User}", owner);

            ledger.Owner = owner;
            ledger.UpdatedAt = DateTime.Now;
            await _ledgerService.Add(ledger);

            return CreatedAtRoute("GetLedger", new 
                {owner = ledger.Owner, year = ledger.Year.ToString(), month = ledger.Month.ToString() }
                , ledger);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAsync([FromQuery]string id, MonthlyLedger inLedger)
        {
            var ledger = _ledgerService.Find(id);
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Updating id {Id} for user {User}", id, owner);

            if (ledger == null) {
                _logger.LogWarning("Id {Id} for user {User} not found", id, owner);
                return NotFound();
            }

            inLedger.UpdatedAt = DateTime.Now;
            await _ledgerService.Update(id, inLedger);

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var ledger = _ledgerService.Find(id);
            var owner = Auth.GetUser(User.Claims);
            _logger.LogInformation("Deleting id {Id} for user {User}", id, owner);

            if (ledger == null) {
                _logger.LogWarning("Id {Id} for user {User} not found", id, owner);
                return NotFound();
            }

            await _ledgerService.Delete(id);

            return NoContent();
        }
    }

    public class ListItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Type { get; set; }
    }
}