using Mingle.Services.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IGenerativeAiService
    {
        Task<string> GeminiGenerateTextAsync(AiRequest request);

        Task<string> FluxGenerateImageAsync(AiRequest request);
    }
}