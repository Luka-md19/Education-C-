using QRCoder;
using System;

public class QRCodeService
{
    public string GenerateQRCode(string content)
    {
        Console.WriteLine($"Encoding content in QR Code: {content}"); // Debugging log

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
            {
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);
                    string base64QRCode = Convert.ToBase64String(qrCodeBytes);
                    Console.WriteLine($"Generated Base64 QR Code: {base64QRCode}"); // Debugging log
                    return base64QRCode;
                }
            }
        }
    }
}
