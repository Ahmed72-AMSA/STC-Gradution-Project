using BankSystem.Service.Services.LoanService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BankSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoansController> _logger;

        public LoansController(ILoanService loanService, ILogger<LoansController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ApplyForLoan([FromForm] LoanApplicationDto formDto)
        {
            var dto = new LoanApplicationDto
            {
                IncomeAnnum = formDto.IncomeAnnum,
                LoanAmount = formDto.LoanAmount,
                LoanTerm = formDto.LoanTerm,
                Education = formDto.Education,
                SelfEmployed = formDto.SelfEmployed,
                NationalIdFile = formDto.NationalIdFile
            };

            // UserId is coming from the form DTO (no claims)
            var result = await _loanService.ApplyForLoanAsync(dto, formDto.UserId, formDto.NationalIdFile);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoan(int loanId)
        {
            var result = await _loanService.GetLoanByIdAsync(loanId);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserLoans(int userId)
        {
            var result = await _loanService.GetLoansByUserIdAsync(userId);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        [HttpPut("{loanId}")]
        public async Task<IActionResult> UpdateLoan(int loanId, [FromBody] LoanUpdateDto dto, [FromQuery] int userId)
        {
            // userId is passed explicitly in query string or request
            var result = await _loanService.UpdateLoanAsync(loanId, dto, userId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{loanId}")]
        public async Task<IActionResult> DeleteLoan(int loanId, [FromQuery] int userId)
        {
            // userId is passed explicitly in query string or request
            var result = await _loanService.DeleteLoanAsync(loanId, userId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("{loanId}/approve")]
        public async Task<IActionResult> ApproveLoan(int loanId)
        {
            var result = await _loanService.ApproveLoanAsync(loanId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("{loanId}/deny")]
        public async Task<IActionResult> DenyLoan(int loanId)
        {
            var result = await _loanService.DenyLoanAsync(loanId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("{loanId}/paid")]
        public async Task<IActionResult> MarkAsPaid(int loanId)
        {
            var result = await _loanService.MarkLoanAsPaidAsync(loanId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }



        [HttpGet]
        public async Task<IActionResult> GetAllLoans()
        {
            var result = await _loanService.GetAllLoansAsync();
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

    }
}
