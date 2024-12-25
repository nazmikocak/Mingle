using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Mingle.DataAccess.Abstract;
using System.IO;

namespace Mingle.DataAccess.Concrete
{
    public sealed class CloudRepository : ICloudRepository
    {
        private readonly Cloudinary _cloudinary;


        public CloudRepository(Configurations.CloudinaryConfig cloudinaryConfig)
        {
            _cloudinary = cloudinaryConfig.Cloudinary;
        }

        
        public async Task<Uri> UploadPhotoAsync(string publicId, string folder, string tags, MemoryStream photo) 
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(publicId, photo),
                PublicId = publicId,
                Overwrite = true,
                Folder = folder,
                Tags = tags,
                Format = "webp"
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));
            return uploadResult.SecureUrl;
        }
    }
}