using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    /// <summary>
    /// Hugging Face API entegrasyonu için yapılandırma sınıfıdır.
    /// </summary>
    public class HuggingFaceConfig
    {
        /// <summary>Hugging Face API anahtarını içerir.</summary>
        public string ApiKey { get; }



        /// <summary>FLUX modelini kullanarak görsel üretimi için API URL'sini içerir.</summary>
        public string FluxImageUrl { get; }



        /// <summary>Artples modelini kullanarak görsel üretimi için API URL'sini içerir.</summary>
        public string ArtplesImageUrl { get; }



        /// <summary>CompVis modelini kullanarak görsel üretimi için API URL'sini içerir.</summary>
        public string CompvisImageUrl { get; }



        /// <summary>
        /// Hugging Face yapılandırmasını belirtilen <see cref="IConfiguration"/> nesnesine göre başlatır.
        /// </summary>
        /// <param name="configuration">Uygulamanın yapılandırma ayarlarını içeren <see cref="IConfiguration"/> nesnesi.</param>
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