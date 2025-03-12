using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;


namespace Mingle.DataAccess.Configurations
{
    /// <summary>
    /// Cloudinary servisi için yapılandırma ayarlarını yönetir.
    /// </summary>
    public class CloudinaryConfig
    {
        /// <summary>Cloudinary servisiyle iletişim kurmak için kullanılan nesnedir.</summary>
        public Cloudinary Cloudinary { get; }



        /// <summary>
        /// Cloudinary yapılandırmasını belirtilen <see cref="IConfiguration"/> nesnesine göre oluşturur.
        /// </summary>
        /// <param name="configuration">Uygulamanın yapılandırma ayarlarını içeren <see cref="IConfiguration"/> nesnesi.</param>
        public CloudinaryConfig(IConfiguration configuration)
        {
            var cloudName = configuration["CloudinarySettings:CloudName"];
            var apiKey = configuration["CloudinarySettings:ApiKey"];
            var apiSecret = configuration["CloudinarySettings:ApiSecret"];

            Account account = new Account(
                cloudName,
                apiKey,
                apiSecret
            );

            Cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }
    }
}