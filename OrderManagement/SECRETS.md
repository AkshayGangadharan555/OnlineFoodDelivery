# Managing secrets (local development)

This project ignores `appsettings.json` and `appsettings.Development.json` by default — use `dotnet user-secrets` for local secrets instead of committing credentials.

From the `OrderManagement` folder run:

```bash
# initialize user-secrets (only needed once per project)
cd OrderManagement
dotnet user-secrets init

# set your DB connection string (example)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=OrdersDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"

# set Jwt settings
dotnet user-secrets set "JwtSettings:Key" "REPLACE_WITH_LONG_SECURE_RANDOM_KEY"
dotnet user-secrets set "JwtSettings:Issuer" "YourIssuer"
dotnet user-secrets set "JwtSettings:Audience" "YourAudience"

# set service urls for inter-service HttpClient config (optional)
dotnet user-secrets set "ServiceUrls:RestaurantService" "https://localhost:6001"
dotnet user-secrets set "ServiceUrls:CustomerService" "https://localhost:6002"
dotnet user-secrets set "ServiceUrls:DeliveryService" "https://localhost:6003"
```

Notes:
- `dotnet user-secrets` stores values in your user profile and is only used in the Development environment.
- For production, use a secret store (Azure Key Vault, AWS Secrets Manager, environment variables, etc.).
