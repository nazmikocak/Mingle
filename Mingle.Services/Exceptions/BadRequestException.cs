namespace Mingle.Services.Exceptions
{
    /// <summary>
    /// Geçersiz istek durumunda fırlatılan özel hata.
    /// </summary>
    public sealed class BadRequestException : Exception
    {
        /// <param name="message">Hata mesajı.</param>
        public BadRequestException(string message) : base(message) { }
    }
}