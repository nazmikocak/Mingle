using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    public class HuggingFaceConfig
    {
        public string ApiKey { get; }
        public string ImageUrl { get; }

        public HuggingFaceConfig(IConfiguration configuration)
        {
            var apiKey = configuration["HuggingFaceSettings:ApiKey"];
            var imageUrl = configuration["HuggingFaceSettings:ImageUrl"];

            ApiKey = apiKey;
            ImageUrl = imageUrl;
        }
    }
}