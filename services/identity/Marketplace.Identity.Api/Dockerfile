FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props global.json ./
COPY services/building-blocks/Marketplace.BuildingBlocks/Marketplace.BuildingBlocks.csproj services/building-blocks/Marketplace.BuildingBlocks/
COPY services/identity/Marketplace.Identity.Api/Marketplace.Identity.Api.csproj services/identity/Marketplace.Identity.Api/
RUN dotnet restore services/identity/Marketplace.Identity.Api/Marketplace.Identity.Api.csproj
COPY services/building-blocks/ services/building-blocks/
COPY services/identity/Marketplace.Identity.Api/ services/identity/Marketplace.Identity.Api/
RUN dotnet publish services/identity/Marketplace.Identity.Api/Marketplace.Identity.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
EXPOSE 8080
HEALTHCHECK --interval=15s --timeout=5s --retries=5 --start-period=30s \
  CMD curl -f http://127.0.0.1:8080/health || exit 1
ENTRYPOINT ["dotnet", "Marketplace.Identity.Api.dll"]
