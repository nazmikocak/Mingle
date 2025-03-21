namespace Mingle.Services.Exceptions
{
    /// <summary>
    /// Erişim engellenmiş durumunda fırlatılan özel hata.
    /// </summary>
    public sealed class ForbiddenException : Exception
    {
        /// <param name="message">Hata mesajı.</param>
        public ForbiddenException(string message) : base(message) { }
    }
}