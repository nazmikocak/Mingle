using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Mingle.DataAccess.Abstract;

namespace Mingle.DataAccess.Concrete
{
    public sealed class CloudRepository : ICloudRepository
    {
        private readonly Cloudinary _cloudinary;


        public CloudRepository(Configurations.CloudinaryConfig cloudinaryConfig)
        {
            _cloudinary = cloudinaryConfig.Cloudinary;
        }


        public async Task<Uri> UploadProfilePhotoAsync(string userId, IFormFile profilePhoto)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(profilePhoto.FileName, profilePhoto.OpenReadStream()),
                PublicId = userId,
                Overwrite = true,
                Folder = "Users",
                Tags = "profile_photo",
                Format = "jpg"
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));
            return uploadResult.SecureUrl;
        }


        public async Task<Uri> UploadPhotoAsync(string publicId, string folder, string tags, IFormFile profilePhoto) 
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(profilePhoto.FileName, profilePhoto.OpenReadStream()),
                PublicId = publicId,
                Overwrite = true,
                Folder = folder,
                Tags = tags,
                Format = "jpg"
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));
            return uploadResult.SecureUrl;
        }
    }
}