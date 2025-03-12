using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    /// <summary>
    /// Gemini AI API entegrasyonu için yapılandırma sınıfıdır.
    /// </summary>
    public class GeminiConfig
    {
        /// <summary>Metin üretimi için kullanılan API URL'sini içerir.</summary>
        public string TextGeneration { get; }



        /// <summary>
        /// Gemini AI yapılandırmasını belirtilen <see cref="IConfiguration"/> nesnesine göre başlatır.
        /// </summary>
        /// <param name="configuration">Uygulamanın yapılandırma ayarlarını içeren <see cref="IConfiguration"/> nesnesi.</param>
        public GeminiConfig(IConfiguration configuration)
        {
            var apiKey = configuration["GeminiSettings:ApiKey"];
            var textUrl = configuration["GeminiSettings:TextUrl"];

            TextGeneration = textUrl + apiKey;
        }
    }
}