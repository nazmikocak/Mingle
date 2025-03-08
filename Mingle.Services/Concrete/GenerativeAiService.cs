using Microsoft.Extensions.Options;
using Mingle.DataAccess.Configurations;
using Mingle.Services.Abstract;
using Mingle.Shared.DTOs.Request;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Mingle.Services.Concrete
{
    public sealed class GenerativeAiService : IGenerativeAiService
    {
        private readonly HuggingFaceConfig _huggingFaceConfig;
        private readonly string _textGeneration;
        private readonly HttpClient _httpClient;


        public GenerativeAiService(IOptions<GeminiConfig> geminiConfig, IOptions<HuggingFaceConfig> huggingFaceConfig)
        {
            _textGeneration = geminiConfig.Value.TextGeneration;
            _huggingFaceConfig = huggingFaceConfig.Value;
            _httpClient = new HttpClient();
        }


        public async Task<string> GeminiGenerateTextAsync(AiRequest request)
        {
            var contentRequest = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = request.Prompt
                            }
                        }
                    }
                }
            };

            string jsonRequest = JsonConvert.SerializeObject(contentRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_textGeneration, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Gemini API ile metin oluştururken hata oluştu.");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
            string responseText = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text ?? "Yanıt alınamadı.";

            return responseText;
        }


        public async Task<string> FluxGenerateImageAsync(AiRequest request)
        {
            var contentRequest = new
            {
                inputs = request.Prompt
            };

            string jsonRequest = JsonConvert.SerializeObject(contentRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _huggingFaceConfig.ApiKey);

            HttpResponseMessage response = await _httpClient.PostAsync(_huggingFaceConfig.ImageUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Hugging Face Flux API ile resim oluşturulurken hata.");
            }

            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

            return Convert.ToBase64String(imageBytes);
        }
    }
}