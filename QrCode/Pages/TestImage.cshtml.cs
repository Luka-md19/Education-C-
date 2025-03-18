using Microsoft.AspNetCore.Mvc.RazorPages;

public class TestImageModel : PageModel
{
    private readonly QRCodeService _qrCodeService;

    // Make ImageData nullable
    public string? ImageData { get; set; }

    // Inject QRCodeService through the constructor
    public TestImageModel(QRCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    public void OnGet()
    {
        // Set the content you want to encode in the QR code
        string contentToEncode = "https://serverapi-trev.azurewebsites.net/api/AccountConroller/registerWithUniqueQrCode"; // This should be the actual content for the QR code
        ImageData = _qrCodeService.GenerateQRCode(contentToEncode);
    }
}
