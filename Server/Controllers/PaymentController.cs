using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase

{
    [HttpGet("checkout")]
    public IActionResult Checkout()
    {
        // Serve the HTML page
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "payment.html"), "text/html");
    }
}
