namespace Mingle.Services.Exceptions
{
    /// <summary>
    /// Bulunamayan kaynak durumunda fırlatılan özel hata.
    /// </summary>
    public sealed class NotFoundException : Exception
    {
        /// <param name="message">Hata mesajı.</param>
        public NotFoundException(string message) : base(message) { }
    }
}