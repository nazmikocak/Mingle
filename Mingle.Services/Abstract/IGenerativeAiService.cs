using Mingle.Shared.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IGenerativeAiService
    {
        Task<string> GeminiGenerateTextAsync(AiRequest request);

        Task<string> HfGenerateImageAsync(AiRequest request);
    }
}