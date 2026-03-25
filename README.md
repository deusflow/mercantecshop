# WebShopMercantec

Backend/API is built on **ASP.NET Core net9.0** with Pomelo MySQL provider.

## Local development (SSH tunnel required)

1. Start SSH tunnel to MariaDB:

```bash
ssh -L 3307:127.0.0.1:3306 -N root@192.168.115.187
```

2. Set local secrets (do not store in repo):

```bash
cd /Users/deuswork/Documents/Programmering/WebShopMercantec/WebShopMercantec/WebShopMercantec

dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1;Port=3307;Database=snipeit;User=snipeit;Password=YOUR_PASSWORD;"
dotnet user-secrets set "Jwt:Key" "YOUR_LONG_DEV_SECRET_AT_LEAST_32_CHARS"
```

3. Run API:

```bash
dotnet run --project /Users/deuswork/Documents/Programmering/WebShopMercantec/WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj
```

Swagger endpoint:
- `http://localhost:5107/swagger`

## Database migrations

Custom SQL migrations are in `migrations/`:
- `001_webshop_tables.sql`
- `002_webshop_refresh_token_indexes.sql`

