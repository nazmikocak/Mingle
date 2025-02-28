using Mingle.Services.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IGenerativeAiService
    {
        Task<string> GeminiGenerateTextAsync(TextRequest request);

        Task<List<string>> GeminiGenerateImagesAsync(ImageRequest request);
    }
}