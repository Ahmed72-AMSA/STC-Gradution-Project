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

        var existingAccount = await _context.Users
        .FirstOrDefaultAsync(u=>u.Email == googleSignUpRequest.Email);



       if(existingAccount != null){
        return BadRequest("You Already Signed-Up by this Account");
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

            
            

            };

            _context.Users.Add(signUp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSignUp), new { id = signUp.Id }, signUp);
        }








[HttpPost("Facebook")]
public async Task<IActionResult> FacebookSignUp([FromBody] FacebookSignUpRequest facebookSignUpRequest)
{
    if (facebookSignUpRequest == null)
    {
        return BadRequest("Facebook sign-up data is null.");
    }

    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var existingUser = await _context.Users
        .FirstOrDefaultAsync(u => u.FacebookId == facebookSignUpRequest.FacebookId);
    
    var existingAccount = await _context.Users
    .FirstOrDefaultAsync(u=>u.Email == facebookSignUpRequest.Email);



    if(existingAccount != null){
        return BadRequest("You Already Signed-Up by this Account");
    }




    if (existingUser != null)
    {
        return BadRequest("User already exists with this Facebook ID.");
    }

    var signUp = new SignUp
    {
        UserName = facebookSignUpRequest.UserName,
        Email = facebookSignUpRequest.Email,
        FacebookId = facebookSignUpRequest.FacebookId,
        Password = "facebook registration",
        PhoneNumber = "facebook registration",
        Gmail = facebookSignUpRequest.Gmail,

    };

    _context.Users.Add(signUp);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetSignUp), new { id = signUp.Id }, signUp);
}











}
}
