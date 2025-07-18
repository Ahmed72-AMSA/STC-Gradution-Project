using BankSystem.Data.Entities;
using BankSystem.Data.Contexts;
using BankSystem.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BankSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequeController : ControllerBase
    {
        private readonly IChequeService _chequeService;
        private readonly BankingContext _dbContext;

        public ChequeController(IChequeService chequeService, BankingContext dbContext)
        {
            _chequeService = chequeService;
            _dbContext = dbContext;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateCheque(
            [FromQuery] string fromAccountName,
            [FromQuery] string toName,
            [FromQuery] string toBankName,
            [FromQuery] string toAccountNumber,
            [FromQuery] decimal amount)
        {
            if (string.IsNullOrEmpty(fromAccountName) || string.IsNullOrEmpty(toName))
                return BadRequest("All fields are required.");

            if (amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var user = await _dbContext.Users
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.UserName == fromAccountName);

            if (user == null)
                return NotFound("Sender account not found.");

            if (user.Account.Balance < amount)
                return BadRequest("Insufficient balance.");

            // Deduct amount immediately
            user.Account.Balance -= amount;

            byte[] chequePdf;
            string generatedChequeNumber;

            try
            {
                // Generate PDF and get generated cheque number
                chequePdf = await _chequeService.GenerateChequePdfAsync(
                    fromAccountName, toName, toBankName, toAccountNumber, amount);

                // Assume chequeNumber is generated inside the service, extract it back
                generatedChequeNumber = _chequeService.LastGeneratedChequeNumber;

                var cheque = new Cheque
                {
                    ChequeNumber = generatedChequeNumber,
                    SenderUserName = fromAccountName,
                    ReceiverName = toName,
                    ReceiverBankName = toBankName,
                    ReceiverAccountNumber = toAccountNumber,
                    Amount = amount,
                    Status = ChequeStatus.Pending
                };

                _dbContext.Cheques.Add(cheque);
                await _dbContext.SaveChangesAsync();

                return File(chequePdf, "application/pdf", $"{generatedChequeNumber}_Cheque.pdf");
            }
            catch (Exception ex)
            {
                // Rollback on failure
                user.Account.Balance += amount;
                await _dbContext.SaveChangesAsync();

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("confirm/{chequeNumber}")]
        public async Task<IActionResult> ConfirmCheque(string chequeNumber)
        {
            var cheque = await _dbContext.Cheques
                .FirstOrDefaultAsync(c => c.ChequeNumber == chequeNumber);

            if (cheque == null)
                return NotFound("Cheque not found.");

            if (cheque.Status != ChequeStatus.Pending)
                return BadRequest($"Cheque is already {cheque.Status}.");

            cheque.Status = ChequeStatus.Cleared;
            cheque.ClearanceDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Cheque confirmed successfully",
                ChequeNumber = cheque.ChequeNumber,
                Status = cheque.Status,
                ClearanceDate = cheque.ClearanceDate
            });
        }

        [HttpPost("cancel/{chequeNumber}")]
        public async Task<IActionResult> CancelCheque(string chequeNumber)
        {
            var cheque = await _dbContext.Cheques
                .Include(c => c.Sender)
                .ThenInclude(u => u.Account)
                .FirstOrDefaultAsync(c => c.ChequeNumber == chequeNumber);

            if (cheque == null)
                return NotFound("Cheque not found.");

            if (cheque.Status != ChequeStatus.Pending)
                return BadRequest($"Cannot cancel cheque that is already {cheque.Status}.");

            cheque.Sender.Account.Balance += cheque.Amount;
            cheque.Status = ChequeStatus.Cancelled;
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Cheque cancelled successfully",
                ChequeNumber = cheque.ChequeNumber,
                Status = cheque.Status,
                AmountRefunded = cheque.Amount,
                NewBalance = cheque.Sender.Account.Balance
            });
        }

        [HttpGet("history/{userName}")]
        public async Task<IActionResult> GetChequeHistory(string userName)
        {
            var cheques = await _dbContext.Cheques
                .Where(c => c.SenderUserName == userName)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();

            return Ok(cheques);
        }
    }
}
