using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.DataAccess.Abstract
{
    public interface ICloudRepository
    {
        Task<Uri> UploadProfilePhotoAsync(string userId, IFormFile profilePhoto);

        Task<Uri> UploadPhotoAsync(string publicId, string folder, string tags, IFormFile profilePhoto);
    }
}