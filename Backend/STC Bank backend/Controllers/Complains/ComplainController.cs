using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STC.Models.Chat_Service;
using STC.Data.Context;
using STC.Models;
using System;

namespace STC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplainsController : ControllerBase
    {
        private readonly STCSystemDbContext _context;

        public ComplainsController(STCSystemDbContext context)
        {
            _context = context;
        }






        [HttpPost]
        public async Task<IActionResult> CreateComplain([FromBody] Complain complain)
        {
            if (complain == null)
            {
                return BadRequest("Complain data is null.");
            }

            complain.Timestamp = DateTime.UtcNow;
            _context.Complains.Add(complain);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComplainById), new { id = complain.Id }, complain);
        }







        [HttpGet]
        public async Task<IActionResult> GetComplains()
        {
            var complains = await _context.Complains.ToListAsync();
            return Ok(complains);
        }





        [HttpGet("{id}")]
        public async Task<IActionResult> GetComplainById(int id)
        {
            var complain = await _context.Complains.FindAsync(id);
            if (complain == null)
            {
                return NotFound("Complain not found.");
            }
            return Ok(complain);
        }






        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplainById(int id)
        {
            var complain = await _context.Complains.FindAsync(id);
            if (complain == null)
            {
                return NotFound("Complain not found.");
            }

            _context.Complains.Remove(complain);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Complain deleted successfully." });
        }





        

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComplain(int id, [FromBody] Complain updatedComplain)
        {
            if (updatedComplain == null || id != updatedComplain.Id)
            {
                return BadRequest("Invalid complain data.");
            }

            var existingComplain = await _context.Complains.FindAsync(id);
            if (existingComplain == null)
            {
                return NotFound("Complain not found.");
            }

            existingComplain.Describtion = updatedComplain.Describtion;
            existingComplain.Recipient = updatedComplain.Recipient;
            existingComplain.Solved = updatedComplain.Solved;
            existingComplain.EndDate = updatedComplain.EndDate;
            
            await _context.SaveChangesAsync();
            return Ok(existingComplain);
        }
    }
}
