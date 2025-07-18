using BankSystem.Data.Entities;
using BankSystem.Service.Services.AccountService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccountAsync([FromBody] int userId)
        {
          
                var account = await _accountService.CreateAccountAsync(userId);
                return Ok(account);
            
        
        }

        [HttpPut("update-secret")]
        public async Task<IActionResult> UpdateAccountSecretAsync([FromQuery] string accountSecret, [FromBody] string newSecret)
        {
            try
            {
                var result = await _accountService.UpdateAccountSecretAsync(accountSecret, newSecret);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetAccountBalanceAsync([FromQuery] string accountSecret)
        {
            try
            {
                var balance = await _accountService.GetAccountBalanceAsync(accountSecret);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetAccountDetailsBySecretAsync([FromQuery] string accountSecret)
        {
            try
            {
                var account = await _accountService.GetAccountDetailsBySecretAsync(accountSecret);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAccountDetailsByUserIdAsync(int userId)
        {
            try
            {
                var account = await _accountService.GetAccountDetailsByUserIdAsync(userId);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccountDetailsAsync([FromQuery] string accountSecret, [FromBody] AccountUpdateRequest request)
        {
            try
            {
                var result = await _accountService.UpdateAccountDetailsAsync(accountSecret, request.NewBalance, request.AccountStatus);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateAccountStatusAsync([FromQuery] string accountSecret, [FromBody] string status)
        {
            try
            {
                var result = await _accountService.UpdateAccountStatusAsync(accountSecret, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("tax/monthly")]
        public async Task<IActionResult> ApplyMonthlyTaxAsync([FromQuery] string accountSecret)
        {
            try
            {
                var newBalance = await _accountService.ApplyMonthlyTaxAsync(accountSecret);
                return newBalance.HasValue ? Ok(newBalance) : NotFound("Account not found or no tax applied.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("tax/annual")]
        public async Task<IActionResult> ApplyAnnualTaxAsync([FromQuery] string accountSecret)
        {
            try
            {
                var newBalance = await _accountService.ApplyAnnualTaxAsync(accountSecret);
                return newBalance.HasValue ? Ok(newBalance) : NotFound("Account not found or no tax applied.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("interest/monthly")]
        public async Task<IActionResult> ApplyMonthlyInterestAsync([FromQuery] string accountSecret)
        {
            try
            {
                var newBalance = await _accountService.ApplyMonthlyInterestAsync(accountSecret);
                return newBalance.HasValue ? Ok(newBalance) : NotFound("Account not found or no interest applied.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("interest/annual")]
        public async Task<IActionResult> ApplyAnnualInterestAsync([FromQuery] string accountSecret)
        {
            try
            {
                var newBalance = await _accountService.ApplyAnnualInterestAsync(accountSecret);
                return newBalance.HasValue ? Ok(newBalance) : NotFound("Account not found or no interest applied.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}