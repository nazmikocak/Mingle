using Microsoft.Extensions.Options;
using Mingle.DataAccess.Configurations;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Shared.DTOs.Request;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace Mingle.Services.Concrete
{
    public sealed class GenerativeAiService : IGenerativeAiService
    {
        private readonly HuggingFaceConfig _huggingFaceConfig;
        private readonly string _textGeneration;
        private readonly HttpClient _httpClient;



        public GenerativeAiService(GeminiConfig geminiConfig, HuggingFaceConfig huggingFaceConfig)
        {
            _textGeneration = geminiConfig.TextGeneration;
            _huggingFaceConfig = huggingFaceConfig;
            _httpClient = new HttpClient();
        }


        public async Task<string> GeminiGenerateTextAsync(AiRequest request)
        {
            if (!request.AiModel.Equals("Gemini-2.0-Flash"))
            {
                throw new BadRequestException("Geçersiz AI modeli!");
            }

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
                throw new Exception("Gemini API ile yanıt oluşturulamadı!");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
            string responseText = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text ?? "Yanıt alınamadı.";

            return responseText;
        }


        public async Task<string> HfGenerateImageAsync(AiRequest request)
        {
            string url = request.AiModel switch
            {
                "Flux" => _huggingFaceConfig.FluxImageUrl,
                "Artples" => _huggingFaceConfig.ArtplesImageUrl,
                "Compvis" => _huggingFaceConfig.CompvisImageUrl,
                _ => throw new BadRequestException("Geçersiz AI modeli!"),
            };

            var contentRequest = new
            {
                inputs = request.Prompt
            };

            string jsonRequest = JsonConvert.SerializeObject(contentRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _huggingFaceConfig.ApiKey);

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Hugging Face API ile resim oluşturulamadı!");
            }

            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

            return Convert.ToBase64String(imageBytes);
        }
    }
}