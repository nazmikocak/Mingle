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
        public async Task<IActionResult> GeminiText(TextRequest request)
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



        // POST: GeminiImage
        [HttpPost]
        public async Task<IActionResult> GeminiImage(ImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(new { images = await _generativeAiService.GeminiGenerateImagesAsync(request) });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }
    }
}