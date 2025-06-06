﻿using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthServer.Data
{
    public class DbSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DbSeeder(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            // Opret clienten 'client-console' hvis den ikke findes
            if (await appManager.FindByClientIdAsync("client-console") is null)
            {
                await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "client-console",
                    ClientSecret = "console1234",

                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.Prefixes.Scope + "message_api"
                    }
                });
            }

            // Opret scopet 'message_api' hvis det ikke findes
            if (await scopeManager.FindByNameAsync("message_api") is null)
            {
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = "message_api",
                    DisplayName = "Adgang til Message API",
                    Resources = { "resource_server" } 
                });
            }

            // Opret clienten 'spa-client' hvis den ikke findes
            if (await appManager.FindByClientIdAsync("spa-client") is null)
            {
                await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "spa-client",
                    DisplayName = "SPA Client",
                    ClientType = ClientTypes.Public, 
                    RedirectUris = { new Uri("https://localhost:7296/callback") },
                    PostLogoutRedirectUris = { new Uri("https://localhost:7296/") },

                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.EndSession,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Prefixes.Scope + Scopes.OpenId,
                        Permissions.Prefixes.Scope + Scopes.OfflineAccess,
                        Permissions.Prefixes.Scope + "note_api"
                    },

                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }

            // Opret scopet 'note_api' hvis det ikke findes
            if (await scopeManager.FindByNameAsync("note_api") is null)
            {
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = "note_api",
                    DisplayName = "Adgang til Note API",
                    Resources = { "resource_server" }
                });
            }

        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
