using Microsoft.AspNetCore.Mvc;
using STC.Models;
using STC.Services;
using System;
using System.Linq;

namespace STC.Controllers
{
    [Route("api/predict")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly OnnxModelService _onnxModelService;

        public PredictionController(OnnxModelService onnxModelService)
        {
            _onnxModelService = onnxModelService;
        }

        [HttpPost("modelA")]
        public IActionResult PredictA([FromBody] CreditApprovalClassificationModelInput input)
        {
            try
            {
                if (input == null)
                {
                    return BadRequest(new { Error = "Invalid input data." });
                }

                // Perform prediction
                var result = _onnxModelService.PredictModelA(input);

                return Ok(new
                {
                    Label = result.Label?.FirstOrDefault(-1), // Returns first element or -1 if empty
                    Probabilities = result.Probabilities ?? new float[0] // Ensure it's an array
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Prediction failed: {ex}");
                return StatusCode(500, new
                {
                    Error = "An internal error occurred while processing the request.",
                    ExceptionType = ex.GetType().Name,
                    Details = ex.Message
                });
            }
        }
    }
}
