using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    public class HuggingFaceConfig
    {
        public string ApiKey { get; }
        public string FluxImageUrl { get; }
        public string ArtplesImageUrl { get; }
        public string CompvisImageUrl { get; }


        public HuggingFaceConfig(IConfiguration configuration)
        {
            var apiKey = configuration["HuggingFaceSettings:ApiKey"]!;
            var fluxImageUrl = configuration["HuggingFaceSettings:FluxImageUrl"]!;
            var artplesImageUrl = configuration["HuggingFaceSettings:ArtplesImageUrl"]!;
            var compvisImageUrl = configuration["HuggingFaceSettings:CompvisImageUrl"]!;

            ApiKey = apiKey;
            FluxImageUrl = fluxImageUrl;
            ArtplesImageUrl = artplesImageUrl;
            CompvisImageUrl = compvisImageUrl;
        }
    }
}