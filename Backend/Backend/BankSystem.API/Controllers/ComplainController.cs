using BankSystem.Data.Entities;
using BankSystem.Service.Services.ComplainService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BankSystem.API.Controllers.STC
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplainsController : ControllerBase
    {
        private readonly IComplainService _service;

        public ComplainsController(IComplainService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComplain([FromBody] ComplainDto dto)
        {
            if (dto == null)
                return BadRequest("Complain is null.");

            var created = await _service.CreateComplainAsync(dto);
            return CreatedAtAction(nameof(GetComplainById), new { id = created.Id }, created);
        }

        [HttpGet]
        public async Task<ActionResult<List<ComplainDto>>> GetComplains()
        {
            var complains = await _service.GetAllComplainsAsync();
            return Ok(complains);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComplainById(int id)
        {
            var complain = await _service.GetComplainByIdAsync(id);
            if (complain == null) return NotFound("Complain not found.");
            return Ok(complain);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComplain(int id, [FromBody] ComplainDto updatedDto)
        {
            if (updatedDto == null || id != updatedDto.Id)
                return BadRequest("Invalid data.");

            var success = await _service.UpdateComplainAsync(id, updatedDto);
            return success ? Ok(updatedDto) : NotFound("Complain not found.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplain(int id)
        {
            var success = await _service.DeleteComplainAsync(id);
            return success ? Ok(new { Message = "Deleted successfully." }) : NotFound("Complain not found.");
        }
    }
}
