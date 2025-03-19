using Microsoft.AspNetCore.Mvc;
using Mingle.Services.Abstract;
using Mingle.Shared.DTOs.Request;

namespace Mingle.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class GenerativeAiController : BaseController
    {
        private readonly IGenerativeAiService _generativeAiService;



        public GenerativeAiController(IGenerativeAiService generativeAiService)
        {
            _generativeAiService = generativeAiService;
        }



        // POST: Text
        [HttpPost]
        public async Task<IActionResult> Text(AiRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(new { responseText = await _generativeAiService.GeminiGenerateTextAsync(request) });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        // POST: Image
        [HttpPost]
        public async Task<IActionResult> Image(AiRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(new { responseImage = await _generativeAiService.HfGenerateImageAsync(request) });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu.", errorDetails = ex.Message });
            }
        }
    }
}