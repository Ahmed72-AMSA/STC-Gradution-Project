using BankSystem.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        [HttpPost("DonationsCalculator")]
        public IActionResult CalculateDonation([FromBody] CalculateDonations request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            // Determine gold price based on karat
            double goldPrice = (request.GoldKarat == 21) ? request.Gold21Price : request.Gold18Price;

            // Calculate donation amounts
            double cashAmount = request.MoneyValue * request.DonationRate;
            double propertyAmount = (request.PropertySharesValue + request.BondsValue + request.ProfitValue) * request.DonationRate;
            double goldAmount = (request.GoldValue * goldPrice) * request.DonationRate;
            double buildingAmount = request.BuildingValue * request.DonationRate;

            // Calculate total donation amount
            double totalDonations = cashAmount + propertyAmount + goldAmount + buildingAmount;

            return Ok(new
            {
                MoneyResult = Math.Ceiling(cashAmount),
                PropertyResult = Math.Ceiling(propertyAmount),
                GoldResult = Math.Ceiling(goldAmount),
                BuildingResult = Math.Ceiling(buildingAmount),
                ZakatTotal = Math.Ceiling(totalDonations)
            });
        }

    }
}
