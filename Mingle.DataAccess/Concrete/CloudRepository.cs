﻿using CloudinaryDotNet;
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
                Format = "webp",
                UseFilenameAsDisplayName = true,
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));
            return uploadResult.SecureUrl;
        }


        public async Task<Uri> UploadVideoAsync(string publicId, string folder, string tags, MemoryStream video)
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(publicId, video),
                PublicId = publicId,
                Overwrite = false,
                Folder = folder,
                Tags = tags,
                Format = "mp4",
                UseFilenameAsDisplayName = true,
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));
            return uploadResult.SecureUrl;
        }


        public async Task<Uri> UploadFileAsync(string publicId, string folder, string tags, MemoryStream file)
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(publicId, file),
                PublicId = publicId,
                Overwrite = false,
                Folder = folder,
                Tags = tags,
                AllowedFormats = ["pdf", "doc", "docx", "xls", "xlsx", "ppt", "pptx", "txt", "zip", "rar"],
                UseFilenameAsDisplayName = true,
            };
            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));
            return uploadResult.SecureUrl;
        }
    }
}