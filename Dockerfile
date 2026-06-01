# === STAGE 1: Build ===
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first, then restore (leverages NuGet layer cache)
COPY WebShopMercantec.sln ./
COPY WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj WebShopMercantec/WebShopMercantec/
COPY WebShopMercantec/WebShopMercantec.Client/WebShopMercantec.Client.csproj WebShopMercantec/WebShopMercantec.Client/
COPY WebShopMercantec.Shared/WebShopMercantec.Shared.csproj WebShopMercantec.Shared/
RUN dotnet restore

# Copy the full source and publish
COPY . .
RUN dotnet publish WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj \
    -c Release -o /app/publish --no-restore

# === STAGE 2: Runtime ===
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Install adduser to create a non-root user on Debian slim images
RUN apt-get update && apt-get install -y --no-install-recommends adduser \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Run as non-root
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WebShopMercantec.dll"]
