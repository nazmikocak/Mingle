using Newtonsoft.Json;
using System.Text;

namespace Mingle.Services.Utilities
{
    public class PackageSizeCalculationHelper
    {
        public void CalculateMessageSizeAndPacketEstimate(object dto)
        {
            string serializedMessage = JsonConvert.SerializeObject(dto);

            int messageSizeInBytes = Encoding.UTF8.GetByteCount(serializedMessage);
            Console.WriteLine($"\nMesaj Boyutu: {messageSizeInBytes} byte");

            int mss = 1460;
            int tcpHeaderSize = 20;
            int ipHeaderSize = 20;
            int websocketHeaderSize = 2;

            int totalHeaderSize = tcpHeaderSize + ipHeaderSize + websocketHeaderSize;

            int packetCount = (int)Math.Ceiling((double)messageSizeInBytes / mss);
            Console.WriteLine($"Tahmini Paket Sayısı: {packetCount} paket");

            int firstPacketSize = mss + totalHeaderSize;
            int lastPacketSize = messageSizeInBytes % mss == 0 ? firstPacketSize : (messageSizeInBytes % mss) + totalHeaderSize;

            Console.WriteLine($"İlk Paket Boyutu: {firstPacketSize} byte (Başlıklar dahil)");
            Console.WriteLine($"Son Paket Boyutu: {lastPacketSize} byte (Başlıklar dahil)");

            if (packetCount == 1)
            {
                Console.WriteLine("Mesaj tek bir pakette iletiliyor.");
            }
        }
    }
}