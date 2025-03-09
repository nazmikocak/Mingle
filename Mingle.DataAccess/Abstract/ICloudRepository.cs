namespace Mingle.DataAccess.Abstract
{
    public interface ICloudRepository
    {
        Task<Uri> UploadPhotoAsync(string publicId, string folder, string tags, MemoryStream photo);

        Task<Uri> UploadVideoAsync(string publicId, string folder, string tags, MemoryStream video);

        Task<Uri> UploadAudioAsync(string publicId, string folder, string tags, MemoryStream audio);

        Task<(Uri, long)> UploadFileAsync(string publicId, string folder, string tags, MemoryStream file);
    }
}