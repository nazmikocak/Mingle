namespace Mingle.Entities.Enums
{
    /// <summary>
    /// Çağrı durumlarını temsil eden enum.
    /// Bir çağrının mevcut durumunu belirtir (Beklemede, Kabul Edildi, Reddedildi, İptal Edildi, Cevapsız Kaldı).
    /// </summary>
    public enum CallStatus
    {
        Pending,
        Accepted,
        Declined,
        Canceled,
        Missed
    }
}