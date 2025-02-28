using Mingle.DataAccess.Configurations;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Newtonsoft.Json;
using System.Text;

namespace Mingle.Services.Concrete
{
    public sealed class GenerativeAiService : IGenerativeAiService
    {
        private readonly string _imageGenerationUrl;
        private readonly string _textGenerationUrl;
        private readonly HttpClient _httpClient;


        public GenerativeAiService(GeminiConfig geminiConfig)
        {
            _imageGenerationUrl = geminiConfig.ImageGenerationUrl;
            _textGenerationUrl = geminiConfig.TextGenerationUrl;
            _httpClient = new HttpClient();
        }


        public async Task<string> GeminiGenerateTextAsync(TextRequest request)
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

            HttpResponseMessage response = await _httpClient.PostAsync(_textGenerationUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Gemini API ile metin oluştururken hata oluştu.");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
            string responseText = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text ?? "Yanıt alınamadı.";

            return responseText;
        }

        public async Task<List<string>> GeminiGenerateImagesAsync(ImageRequest request)
        {
            var contentRequest = new
            {
                prompt = request.Prompt,
                number_of_images = request.NumberOfImages,
                aspect_ratio = request.AspectRatio
            };

            string jsonRequest = JsonConvert.SerializeObject(contentRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_imageGenerationUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Gemini API ile görüntü oluştururken hata oluştu.");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
            //

            List<string> imageUrls = new();
            foreach (var image in geminiResponse?.generated_images)
            {
                imageUrls.Add(image.image_url.ToString());
            }

            return imageUrls;
        }
    }
}