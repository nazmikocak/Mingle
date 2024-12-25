using Microsoft.AspNetCore.Http;

namespace Mingle.DataAccess.Abstract
{
    public interface ICloudRepository
    {
        Task<Uri> UploadPhotoAsync(string publicId, string folder, string tags, MemoryStream photo);
    }
}