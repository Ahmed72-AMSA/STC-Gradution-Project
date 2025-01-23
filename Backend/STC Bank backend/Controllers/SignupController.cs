using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STC.Models;
using STC.Data.Context;


namespace STC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly STCSystemDbContext _context;

        public SignUpController(STCSystemDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignUp([FromBody] SignUp signUp)
        {
            if (signUp == null)
            {
                return BadRequest("SignUp data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(signUp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSignUp), new { id = signUp.Id }, signUp);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignUp>>> GetSignUps()
        {
            var signUps = await _context.Users.ToListAsync();
            return Ok(signUps);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SignUp>> GetSignUp(int id)
        {
            var signUp = await _context.Users.FindAsync(id);

            if (signUp == null)
            {
                return NotFound("SignUp not found.");
            }

            return Ok(signUp);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSignUp(int id, [FromBody] SignUp updatedSignUp)
        {
            if (id != updatedSignUp.Id)
            {
                return BadRequest("SignUp ID mismatch.");
            }

            var signUp = await _context.Users.FindAsync(id);
            if (signUp == null)
            {
                return NotFound("SignUp not found.");
            }

            // Update the SignUp details
            signUp.UserName = updatedSignUp.UserName;
            signUp.Email = updatedSignUp.Email;
            signUp.Password = updatedSignUp.Password;
            signUp.PhoneNumber = updatedSignUp.PhoneNumber;
            signUp.NationalId = updatedSignUp.NationalId;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return NoContent(); // Successfully updated, no content to return
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSignUp(int id)
        {
            var signUp = await _context.Users.FindAsync(id);

            if (signUp == null)
            {
                return NotFound("SignUp not found.");
            }

            _context.Users.Remove(signUp);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPost("Google")]
        public async Task<IActionResult> GoogleSignUp([FromBody] GoogleSignUpRequest googleSignUpRequest)
        {
            if (googleSignUpRequest == null)
            {
                return BadRequest("Google sign-up data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Gmail == googleSignUpRequest.Gmail);

            if (existingUser != null)
            {
                return BadRequest("User already exists with this Gmail.");
            }

            var signUp = new SignUp
            {
            
                UserName = googleSignUpRequest.UserName,
                Gmail = googleSignUpRequest.Gmail,
                Email = googleSignUpRequest.Email,
                Password = "gmail registration",
                PhoneNumber = "gmail registration",
                NationalId = "Not entered yet"

            
            

            };

            _context.Users.Add(signUp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSignUp), new { id = signUp.Id }, signUp);
        }



[HttpPut("Google/NationalId/{userId}")]
public async Task<IActionResult> GoogleSignUpNationalId(int userId, [FromBody] NationalIdRequestGoogle request)
{
    if (request == null || string.IsNullOrWhiteSpace(request.NationalId))
    {
        return BadRequest("National ID is required.");
    }


 if (!System.Text.RegularExpressions.Regex.IsMatch(request.NationalId, @"^\d{14}$"))
    {
        return BadRequest("National ID must be exactly 14 digits.");
    }


    // Check if the National ID is already registered
    var existingUserWithNationalId = await _context.Users
        .FirstOrDefaultAsync(u => u.NationalId == request.NationalId);

    if (existingUserWithNationalId != null)
    {
        return BadRequest($"This National ID is already registered for another user or you already has an account");
    }



    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
    {
        return NotFound($"User not found with ID {userId}.");
    }

    user.NationalId = request.NationalId;

    _context.Entry(user).Property(u => u.NationalId).IsModified = true;
    _context.Users.Update(user);
    await _context.SaveChangesAsync();

    return Ok(new
    {
        Message = "National ID updated successfully.",
        UserId = user.Id,
        NationalId = user.NationalId
    });
}




        [HttpPost("Facebook")]
        public async Task<IActionResult> FacebookSignUp([FromBody] GoogleSignUpRequest googleSignUpRequest)
        {
            if (googleSignUpRequest == null)
            {
                return BadRequest("Google sign-up data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Gmail == googleSignUpRequest.Gmail);

            if (existingUser != null)
            {
                return BadRequest("User already exists with this Gmail.");
            }

            var signUp = new SignUp
            {
            
                UserName = googleSignUpRequest.UserName,
                Gmail = googleSignUpRequest.Gmail,
                Email = googleSignUpRequest.Email,
                Password = "gmail registration",
                PhoneNumber = "gmail registration",
                NationalId = "Not entered yet"

            
            

            };

            _context.Users.Add(signUp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSignUp), new { id = signUp.Id }, signUp);
        }



[HttpPut("Facebook/NationalId/{userId}")]
public async Task<IActionResult> FacebookSignUpNationalId(int userId, [FromBody] NationalIdRequestGoogle request)
{
    if (request == null || string.IsNullOrWhiteSpace(request.NationalId))
    {
        return BadRequest("National ID is required.");
    }


 if (!System.Text.RegularExpressions.Regex.IsMatch(request.NationalId, @"^\d{14}$"))
    {
        return BadRequest("National ID must be exactly 14 digits.");
    }


    // Check if the National ID is already registered
    var existingUserWithNationalId = await _context.Users
        .FirstOrDefaultAsync(u => u.NationalId == request.NationalId);

    if (existingUserWithNationalId != null)
    {
        return BadRequest($"This National ID is already registered for another user or you already has an account");
    }



    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
    {
        return NotFound($"User not found with ID {userId}.");
    }

    user.NationalId = request.NationalId;

    _context.Entry(user).Property(u => u.NationalId).IsModified = true;
    _context.Users.Update(user);
    await _context.SaveChangesAsync();

    return Ok(new
    {
        Message = "National ID updated successfully.",
        UserId = user.Id,
        NationalId = user.NationalId
    });
}






}
}
