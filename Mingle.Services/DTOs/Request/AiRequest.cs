namespace Mingle.Services.DTOs.Request
{
    public class TextRequest
    {
        // TODO: DataAnnetions 
        public string Prompt { get; set; }
    }

    public class ImageRequest
    {
        public string Prompt { get; set; }

        public int NumberOfImages { get; set; } = 1;

        public string AspectRatio { get; set; } = "1:1";
    }
}