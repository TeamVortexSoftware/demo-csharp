# Vortex C# SDK Demo

A demo application showcasing the Vortex C# SDK integration with ASP.NET Core.

## Features

- 🔐 **Authentication System**: Cookie-based auth with JWT tokens
- ⚡ **Vortex Integration**: Full Vortex API integration for invitation management
- 🎯 **JWT Generation**: Generate Vortex JWTs for authenticated users
- 📧 **Invitation Management**: Get, accept, revoke, and reinvite functionality
- 👥 **Group Management**: Handle invitations by group type and ID
- 🌐 **Interactive Frontend**: Complete HTML interface to test all features

## Prerequisites

- .NET 6.0 SDK or later
- The Vortex C# SDK (automatically linked via project reference)

## Installation

1. Navigate to the demo directory:
   ```bash
   cd apps/demo-csharp
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

## Running the Demo

1. Set your Vortex API key (optional - defaults to demo key):
   ```bash
   export VORTEX_API_KEY=your-api-key-here
   ```

2. Run the server:
   ```bash
   dotnet run
   ```

3. Open your browser and visit: `http://localhost:5000`

## Demo Users

The demo includes two pre-configured users:

| Email | Password | Role | Groups |
|-------|----------|------|--------|
| alice@example.com | password123 | admin | Main Workspace, Engineering Team |
| bob@example.com | password123 | member | Main Workspace |

## API Endpoints

### Authentication
- `POST /api/auth/login` - Login with email/password
- `POST /api/auth/logout` - Logout current user
- `GET /api/auth/me` - Get current user info
- `GET /api/auth/users` - Get demo users list

### Vortex Integration
- `POST /api/vortex/jwt` - Generate Vortex JWT for current user
- `GET /api/vortex/invitations` - Get invitations by target
- `GET /api/vortex/invitations/:id` - Get specific invitation
- `DELETE /api/vortex/invitations/:id` - Revoke invitation
- `POST /api/vortex/invitations/accept` - Accept invitations
- `GET /api/vortex/invitations/by-group/:type/:id` - Get group invitations
- `DELETE /api/vortex/invitations/by-group/:type/:id` - Delete group invitations
- `POST /api/vortex/invitations/:id/reinvite` - Reinvite user

## Project Structure

```
demo-csharp/
├── Controllers/
│   ├── AuthController.cs       # Authentication endpoints
│   └── VortexController.cs     # Vortex API endpoints
├── wwwroot/
│   └── index.html              # Demo frontend
├── Program.cs                  # Application entry point
├── VortexDemo.csproj          # Project file
└── README.md                  # This file
```

## Development

To run in development mode with hot reload:

```bash
dotnet watch run
```

## License

MIT
