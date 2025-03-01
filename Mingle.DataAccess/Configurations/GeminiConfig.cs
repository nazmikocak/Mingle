using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    public class GeminiConfig
    {
        public string TextGeneration { get; }

        public GeminiConfig(IConfiguration configuration)
        {
            var apiKey = configuration["GeminiSettings:ApiKey"];
            var textUrl = configuration["GeminiSettings:TextUrl"];

            TextGeneration = textUrl + apiKey;
        }
    }
}