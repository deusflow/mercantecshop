FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY WebShopMercantec.sln ./
COPY WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj WebShopMercantec/WebShopMercantec/
COPY WebShopMercantec/WebShopMercantec.Client/WebShopMercantec.Client.csproj WebShopMercantec/WebShopMercantec.Client/
COPY WebShopMercantec.Shared/WebShopMercantec.Shared.csproj WebShopMercantec.Shared/
RUN dotnet restore

COPY . .
RUN dotnet publish WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/logs

COPY --from=build /app/publish .

RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WebShopMercantec.dll"]

