FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props global.json ./
COPY services/building-blocks/Marketplace.BuildingBlocks/Marketplace.BuildingBlocks.csproj services/building-blocks/Marketplace.BuildingBlocks/
COPY services/gateway/Marketplace.Gateway.Api/Marketplace.Gateway.Api.csproj services/gateway/Marketplace.Gateway.Api/
RUN dotnet restore services/gateway/Marketplace.Gateway.Api/Marketplace.Gateway.Api.csproj
COPY services/building-blocks/ services/building-blocks/
COPY services/gateway/Marketplace.Gateway.Api/ services/gateway/Marketplace.Gateway.Api/
RUN dotnet publish services/gateway/Marketplace.Gateway.Api/Marketplace.Gateway.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
EXPOSE 8080
HEALTHCHECK --interval=15s --timeout=5s --retries=5 --start-period=20s \
  CMD curl -f http://127.0.0.1:8080/health || exit 1
ENTRYPOINT ["dotnet", "Marketplace.Gateway.Api.dll"]
