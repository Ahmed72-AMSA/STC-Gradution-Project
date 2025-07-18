using BankSystem.Data.Entities;
using BankSystem.Data.Entities.Signup;
using BankSystem.Data.Entities.Signup.BankSystem.Data.Entities;
using BankSystem.Service.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace STC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _userService.RegisterUserAsync(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPost("facebook")]
        public async Task<IActionResult> FacebookRegister([FromBody] FacebookSignUpRequest fbDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, createdUser) = await _userService.FacebookRegisterAsync(fbDto);
            if (!success)
                return BadRequest(new { message });

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser!.Id }, createdUser);
        }

        [HttpPatch("facebook/update-contact")]
        public async Task<IActionResult> UpdatePhoneAndIDByFacebookId([FromBody] FacebookUpdatePhoneAndIDRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _userService.UpdatePhoneAndIDByFacebookIdAsync(request.FacebookId, request.PhoneNumber, request.NationalID);
            return success ? Ok(new { message }) : NotFound(new { message });
        }

        [HttpDelete("facebook")]
        public async Task<IActionResult> DeleteByFacebookId([FromQuery] string facebookId)
        {
            if (string.IsNullOrWhiteSpace(facebookId))
                return BadRequest(new { message = "Facebook ID is required." });

            var (success, message) = await _userService.DeleteUserByFacebookIdAsync(facebookId);
            return success ? NoContent() : NotFound(new { message });
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleSignup([FromBody] GoogleSignUpRequest request)
        {
            var (success, message, user) = await _userService.GoogleSignUpAsync(request);
            if (!success) return BadRequest(new { message });

            return CreatedAtAction(nameof(GetUserById), new { id = user!.Id }, new { userId = user.Id });
        }

        [HttpPatch("google/update-contact/{id}")]
        public async Task<IActionResult> UpdateGoogleUserContact(int id, [FromBody] GoogleUpdatePhoneAndIDRequest updateDto)
        {
            if (string.IsNullOrWhiteSpace(updateDto.PhoneNumber) || string.IsNullOrWhiteSpace(updateDto.NationalID))
                return BadRequest(new { message = "Phone number and National ID are required." });

            var result = await _userService.GoogleUpdatePhoneAndIDByUserIdAsync(id, updateDto.PhoneNumber, updateDto.NationalID);
            return result.Success ? NoContent() : NotFound(new { message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound(new { message = "User not found." }) : Ok(user);
        }

     

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            return deleted ? NoContent() : NotFound(new { message = "User not found." });
        }


        [HttpGet("BasicInfo/{id}")]
        public async Task<IActionResult> GetBasicInfo(int id)
        {
            var result = await _userService.GetUserBasicInfoAsync(id);
            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserFieldsById(int id, [FromBody] UserUpdateDto dto)
        {
            var (success, message) = await _userService.UpdateUserFieldsByIdAsync(id, dto);
            return success ? Ok(message) : NotFound(message);
        }

        [HttpGet("by-nationalid/{nationalId}")]
        public async Task<IActionResult> GetUserByNationalId(string nationalId)
        {
            var (success, message, user) = await _userService.GetUserByNationalIdAsync(nationalId);
            return success ? Ok(user) : NotFound(message);
        }

    }
}
