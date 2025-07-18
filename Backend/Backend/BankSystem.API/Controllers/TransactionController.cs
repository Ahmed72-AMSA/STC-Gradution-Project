using BankSystem.Service.Services.TransactionService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistoryAsync([FromQuery] string accountSecret, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionHistoryAsync(accountSecret, startDate, endDate);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransactionDetailsAsync(int transactionId)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionDetailsAsync(transactionId);
                if (transaction == null) return NotFound("Transaction not found.");
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("cancel/{transactionId}")]
        public async Task<IActionResult> CancelTransactionAsync(int transactionId)
        {
            try
            {
                var result = await _transactionService.CancelTransactionAsync(transactionId);
                if (!result) return NotFound("Transaction cannot be canceled.");
                return Ok("Transaction canceled successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("status/{transactionId}")]
        public async Task<IActionResult> GetTransactionStatusAsync(int transactionId)
        {
            try
            {
                var status = await _transactionService.GetTransactionStatusAsync(transactionId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> InitiateWithdrawAsync([FromQuery] string accountSecret, [FromQuery] decimal amount)
        {
            try
            {
                var result = await _transactionService.InitiateWithdrawAsync(accountSecret, amount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw/confirm")]
        public async Task<IActionResult> ConfirmWithdrawAsync(
            [FromQuery] string accountSecret,
            [FromQuery] decimal amount,
            [FromQuery] string otp,
            [FromQuery] int TransactionId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountSecret))
                    return BadRequest("Account secret is required.");
                if (amount <= 0)
                    return BadRequest("Amount must be positive.");
                if (string.IsNullOrEmpty(otp))
                    return BadRequest("OTP is required.");
                if (TransactionId <= 0)
                    return BadRequest("Invalid transaction ID.");

                var result = await _transactionService.ConfirmWithdrawAsync(accountSecret, amount, otp, TransactionId);
                return result.success ? Ok(new { result.message, result.newBalance }) : BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> InitiateDepositAsync([FromQuery] string accountSecret, [FromQuery] decimal amount)
        {
            try
            {
                var result = await _transactionService.InitiateDepositAsync(accountSecret, amount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deposit/confirm")]
        public async Task<IActionResult> ConfirmDepositAsync([FromQuery] string accountSecret, [FromQuery] int transactionId, [FromQuery] string otp)
        {
            try
            {
                var result = await _transactionService.ConfirmDepositAsync(accountSecret, transactionId, otp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deposit/customer-service-confirm")]
        public async Task<IActionResult> CustomerServiceConfirmDepositAsync([FromQuery] int transactionId, [FromQuery] string otp)
        {
            try
            {
                var result = await _transactionService.CustomerServiceConfirmDepositAsync(transactionId, otp);

                // Check if the result contains "expired" to return specific status code
                if (result.Contains("expired", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result,
                        errorType = "OTP_EXPIRED"
                    });
                }

                // Check for other error cases
                if (result.Contains("invalid", StringComparison.OrdinalIgnoreCase) ||
                    result.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result
                    });
                }

                // Success case
                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferMoneyAsync([FromQuery] string fromAccountSecret, [FromQuery] string toAccountNumber, [FromQuery] decimal amount)
        {
            try
            {
                var result = await _transactionService.TransferMoneyAsync(fromAccountSecret, toAccountNumber, amount);
                return result.success ? Ok(result.message) : BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}