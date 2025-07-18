using BankSystem.Data.Entities.Login;
using BankSystem.Data.Entities;
using BankSystem.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BankSystem.Service.Helper;

namespace BankSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _loginService.LoginAsync(request);
            return HandleServiceResult(result);
        }

        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request)
        {
            var result = await _loginService.GoogleLoginAsync(request);
            return HandleServiceResult(result);
        }

        [HttpPost("FacebookLogin")]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginDto request)
        {
            var result = await _loginService.FacebookLoginAsync(request);
            return HandleServiceResult(result);
        }


    [HttpPost("VerifyOtp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequestDto request)
    {
        var result = await _loginService.VerifyOtp(request);

        if (result.Success && result.Data is UserInfoDto userData)
        {
            return Ok(new
            {
                message = result.Message,
                userId = userData.UserId,
                username = userData.Username
            });
        }

        return BadRequest(new { message = result.Message });
    }


    [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var result = await _loginService.ForgotPasswordAsync(request);
            return HandleServiceResult(result);
        }

        [HttpPost("SuspendUser/{userId}")]
        public async Task<IActionResult> SuspendUser(int userId)
        {
            var result = await _loginService.SuspendUserAsync(userId);
            return HandleServiceResult(result);
        }

        [HttpPost("UnsuspendUser/{userId}")]
        public async Task<IActionResult> UnsuspendUser(int userId)
        {
            var result = await _loginService.UnsuspendUserAsync(userId);
            return HandleServiceResult(result);
        }

        private IActionResult HandleServiceResult(ServiceResult result)
        {
            if (result.Success)
                return Ok(new { message = result.Message });
            else
                return BadRequest(new { message = result.Message });
        }
    }
}
