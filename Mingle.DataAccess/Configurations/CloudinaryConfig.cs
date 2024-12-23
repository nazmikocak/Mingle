using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;


namespace Mingle.DataAccess.Configurations
{
    public class CloudinaryConfig
    {
        public Cloudinary Cloudinary { get; }

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