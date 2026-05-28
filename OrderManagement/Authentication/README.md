# JWT Authentication

Minimal JWT setup for Order Management service.

## Files
- **JwtSettings.cs** - JWT configuration model
- **JwtServiceExtensions.cs** - JWT authentication setup

## Configuration

Set these environment variables:
- `JWT_SECRET` - Secret key (min 32 characters)
- `JWT_ISSUER` - Token issuer
- `JWT_AUDIENCE` - Token audience

## Usage

### In Program.cs
```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);
```

### Protect endpoints
```csharp
[Authorize]
public class OrdersController : ControllerBase { }
```
