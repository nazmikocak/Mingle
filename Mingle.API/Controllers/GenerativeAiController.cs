using Microsoft.AspNetCore.Mvc;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;

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



        // POST: GeminiText
        [HttpPost]
        public async Task<IActionResult> GeminiText(AiRequest request)
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // POST: FluxImage
        [HttpPost]
        public async Task<IActionResult> FluxIamge(AiRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(new { responseImage = await _generativeAiService.FluxGenerateImageAsync(request) });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }
    }
}