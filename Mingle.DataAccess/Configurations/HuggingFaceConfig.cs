using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    /// <summary>
    /// Hugging Face API entegrasyonu için yapılandırma sınıfıdır.
    /// </summary>
    public class HuggingFaceConfig
    {
        /// <summary>Hugging Face API anahtarını içerir.</summary>
        public string apiKey { get; }



        /// <summary>FLUX modelini kullanarak görsel üretimi için API URL'sini içerir.</summary>
        public string fluxImage { get; }



        /// <summary>Artples modelini kullanarak görsel üretimi için API URL'sini içerir.</summary>
        public string artplesImage { get; }



        /// <summary>CompVis modelini kullanarak görsel üretimi için API URL'sini içerir.</summary>
        public string compvisImage { get; }



        /// <summary>
        /// Hugging Face yapılandırmasını belirtilen <see cref="IConfiguration"/> nesnesine göre başlatır.
        /// </summary>
        /// <param name="configuration">Uygulamanın yapılandırma ayarlarını içeren <see cref="IConfiguration"/> nesnesi.</param>
        public HuggingFaceConfig(IConfiguration configuration)
        {
            var apiKey = configuration["HuggingFace:apiKey"]!;
            var fluxImageUrl = configuration["HuggingFace:fluxImage"]!;
            var artplesImageUrl = configuration["HuggingFace:artplesImage"]!;
            var compvisImageUrl = configuration["HuggingFace:compvisImage"]!;

            apiKey = apiKey;
            fluxImage = fluxImageUrl;
            artplesImage = artplesImageUrl;
            compvisImage = compvisImageUrl;
        }
    }
}