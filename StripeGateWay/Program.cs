using Stripe;
using StripeGateWay.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
// Accessing the Configuration from builder
var stripeApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];
StripeConfiguration.ApiKey = stripeApiKey;

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
