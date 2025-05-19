using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenIddict validation services
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // URL'en til din AuthServer
        options.SetIssuer("https://localhost:7143/");

        // Delte krypteringsnøgle
        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        // Brug HttpClient til discovery
        options.UseSystemNetHttp();

        // Registrér ASP.NET Core-integration
        options.UseAspNetCore();
    });

// Hvis du vil bruge [Authorize] attributten globalt (valgfrit)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
