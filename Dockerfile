# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (cache restore layer)
COPY MediPrax.slnx .
COPY src/MediPrax.Core/MediPrax.Core.csproj src/MediPrax.Core/
COPY src/MediPrax.Application/MediPrax.Application.csproj src/MediPrax.Application/
COPY src/MediPrax.Infrastructure/MediPrax.Infrastructure.csproj src/MediPrax.Infrastructure/
COPY src/MediPrax.Server/MediPrax.Server.csproj src/MediPrax.Server/
COPY src/MediPrax.Reporting/MediPrax.Reporting.csproj src/MediPrax.Reporting/
COPY src/MediPrax.UI/MediPrax.UI.csproj src/MediPrax.UI/
RUN dotnet restore src/MediPrax.Server/MediPrax.Server.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish src/MediPrax.Server/MediPrax.Server.csproj -c Release -o /app --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install QuestPDF system dependencies (libSkiaSharp)
RUN apt-get update && apt-get install -y --no-install-recommends \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/login || exit 1

ENTRYPOINT ["dotnet", "MediPrax.Server.dll"]
