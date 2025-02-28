using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    public class GeminiConfig
    {
        public string TextGenerationUrl { get; }
        public string ImageGenerationUrl { get; }

        public GeminiConfig(IConfiguration configuration)
        {
            var apiKey = configuration["GeminiSettings:ApiKey"];
            var textUrl = configuration["GeminiSettings:TextUrl"];
            var imageUrl = configuration["GeminiSettings:ImageUrl"];

            TextGenerationUrl = textUrl + apiKey;
            ImageGenerationUrl = imageUrl + apiKey;
        }
    }
}