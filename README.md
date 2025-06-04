# HovedProjekt – OAuth 2.0 & OpenID Connect with OpenIddict

This solution demonstrates a secure authentication and authorization architecture using **OAuth 2.0** and **OpenID Connect** with **OpenIddict** in ASP.NET Core.

The project is structured into four separate applications, each with a distinct role:

### AuthServer
Acts as the **Identity Provider** (Authorization Server):
- Issues JWT tokens using Authorization Code Flow (with PKCE) and Client Credentials Flow.
- Registers and manages clients and scopes.
- Provides login functionality using ASP.NET Identity.

### ClientConsole
A **console application** simulating a machine-to-machine client:
- Uses the Client Credentials Flow to obtain access tokens and call protected APIs.

### ClientSpa
A **Single Page Application (SPA)**:
- Demonstrates an interactive login using Authorization Code Flow with PKCE.
- Redirects to the AuthServer for authentication.

### ResourceAPI
A **protected REST API**:
- Exposes endpoints that require a valid access token.
- Validates scopes and user identity from the incoming JWT token.

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (or update the connection string)

### Setup

1. **Clone the repo**:
   ```bash
   git clone https://github.com/TLLoevkilde/HovedProjekt.git
   cd HovedProjekt
## Getting Started

### Run database migrations and seed data
`AuthServer` will automatically **create and seed** the database (clients, scopes, etc.) on startup.

###  Start the projects in the following order
Start each project in separate terminals or via Visual Studio:

1. **AuthServer**
2. **ResourceAPI**
3. **ClientConsole** (for non-interactive client flow)
4. **ClientSpa** (for interactive browser login)

### Test the system

- Use **ClientConsole** to call the `message_api`.
  - It should write the acces token and a succes message in the console.
- Use **ClientSpa** in the browser to authenticate and interact with the `note_api`:
  - On the **first run**, you will need to **register a new user** via the AuthServer's registration page(you should be redirected automatically).
  - After registration, return to the client and login with the user you just created.
  - You should then be able to write notes and see them on the page.


---

## Technologies

- **ASP.NET Core Identity**
- **OpenIddict** (OIDC Provider)
- **OAuth 2.0 Flows**: Authorization Code + PKCE, Client Credentials
- **Entity Framework Core** (SQL Server LocalDB)
- **JWT Bearer Tokens**
