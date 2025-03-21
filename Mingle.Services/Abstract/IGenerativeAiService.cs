using Mingle.Shared.DTOs.Request;

namespace Mingle.Services.Abstract
{
    /// <summary>
    /// Generative AI servislerini sağlayan arayüz.
    /// </summary>
    public interface IGenerativeAiService
    {
        /// <summary>
        /// Gemini modeli kullanarak metin oluşturur.
        /// </summary>
        /// <param name="request">AI modeline gönderilecek isteği içeren veri.</param>
        /// <returns>Oluşturulan metni döner.</returns>
        /// <exception cref="BadRequestException">Geçersiz AI modeli durumunda hata fırlatılır.</exception>
        Task<string> GeminiGenerateTextAsync(AiRequest request);



        /// <summary>
        /// Hugging Face AI modelleri kullanarak resim oluşturur.
        /// </summary>
        /// <param name="request">AI modeline gönderilecek isteği içeren veri.</param>
        /// <returns>Oluşturulan resmi base64 formatında döner.</returns>
        /// <exception cref="BadRequestException">Geçersiz AI modeli durumunda hata fırlatılır.</exception>
        Task<string> HfGenerateImageAsync(AiRequest request);
    }
}