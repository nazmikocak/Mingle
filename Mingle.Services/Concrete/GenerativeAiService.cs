using Mingle.DataAccess.Configurations;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Shared.DTOs.Request;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Mingle.Services.Concrete
{
    /// <summary>
    /// Yapay zeka metin ve görsel oluşturma işlemlerini yöneten servis sınıfıdır.
    /// HuggingFace ve Gemini API'leri ile entegre çalışarak, metin ve görsel üretme işlemleri gerçekleştirilir.
    /// </summary>
    public sealed class GenerativeAiService : IGenerativeAiService
    {
        private readonly HuggingFaceConfig _huggingFaceConfig;
        private readonly string _textGeneration;
        private readonly HttpClient _httpClient;



        /// <summary>
        /// GenerativeAiService sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="geminiConfig">Gemini API yapılandırma ayarlarını içeren config.</param>
        /// <param name="huggingFaceConfig">Hugging Face API yapılandırma ayarlarını içeren config.</param>
        public GenerativeAiService(GeminiConfig geminiConfig, HuggingFaceConfig huggingFaceConfig)
        {
            _textGeneration = geminiConfig.TextGeneration;
            _huggingFaceConfig = huggingFaceConfig;
            _httpClient = new HttpClient();
        }



        /// <summary>
        /// Gemini API kullanarak metin oluşturur.
        /// </summary>
        /// <param name="request">Yapay zeka modelinden metin üretmek için kullanılan istek verisi.</param>
        /// <returns>Gemini API tarafından üretilen metin.</returns>
        /// <exception cref="BadRequestException">Geçersiz AI modeli verildiğinde fırlatılır.</exception>
        /// <exception cref="Exception">Gemini API ile bağlantı hatası durumunda fırlatılır.</exception>
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



        /// <summary>
        /// Hugging Face API kullanarak görsel oluşturur.
        /// </summary>
        /// <param name="request">Yapay zeka modelinden görsel üretmek için kullanılan istek verisi.</param>
        /// <returns>Hugging Face API tarafından üretilen görselin Base64 kodlamalı hali.</returns>
        /// <exception cref="BadRequestException">Geçersiz AI modeli verildiğinde fırlatılır.</exception>
        /// <exception cref="Exception">Hugging Face API ile görsel oluşturulamadığında fırlatılır.</exception>
        public async Task<string> HfGenerateImageAsync(AiRequest request)
        {
            string url = request.AiModel switch
            {
                "Flux" => _huggingFaceConfig.FluxImage,
                "Artples" => _huggingFaceConfig.ArtplesImage,
                "Compvis" => _huggingFaceConfig.CompvisImage,
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