using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;
using System.Net.Http.Headers;
using static OpenIddict.Client.OpenIddictClientModels;

// 1. Opret service-samling
var services = new ServiceCollection();

// 2. Registrer HttpClient og OpenIddict-klienten
services.AddHttpClient(); // Til at kalde API senere

services.AddOpenIddict()
    .AddClient(options =>
    {
        options.AllowClientCredentialsFlow();
        options.DisableTokenStorage();
        options.UseSystemNetHttp();

        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri("https://localhost:7143/", UriKind.Absolute),
            ClientId = "client-console",
            ClientSecret = "console1234"
        });
    });

// 3. Build service provider
using var serviceProvider = services.BuildServiceProvider();

// 4. Opret scope og hent OpenIddictClientService
using var scope = serviceProvider.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<OpenIddictClientService>();

// 5. Hent access token via client credentials
var result = await service.AuthenticateWithClientCredentialsAsync(new ClientCredentialsAuthenticationRequest
{
    Scopes = new List<string> { "message_api" }
});

Console.WriteLine($"Access token:\n {result.AccessToken}");

Console.WriteLine("\nRequesting message from ResourceApi");
// 6. Brug token til at kalde en beskyttet API
var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

var response = await httpClient.GetAsync("https://localhost:7126/api/message/");
var content = await response.Content.ReadAsStringAsync();

Console.WriteLine("API response:\n");
Console.WriteLine(content);
Console.WriteLine("\nPress enter to exit...");
Console.ReadLine();
