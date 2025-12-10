# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["AmbulanceRider/AmbulanceRider.csproj", "AmbulanceRider/"]
RUN dotnet restore "AmbulanceRider/AmbulanceRider.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/AmbulanceRider"

# Build and publish the Blazor WebAssembly app
RUN dotnet publish "AmbulanceRider.csproj" -c Release -o /app/publish \
    /p:DeployOnBuild=true \
    /p:WebAssemblyEnableCompression=false \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install required runtime dependencies (switch to HTTPS sources)
RUN set -eux; \
    if [ -f /etc/apt/sources.list ]; then \
        sed -i 's|http://deb.debian.org|https://deb.debian.org|g' /etc/apt/sources.list; \
    fi; \
    if [ -d /etc/apt/sources.list.d ]; then \
        for source in /etc/apt/sources.list.d/*; do \
            if [ -f "$source" ]; then \
                sed -i 's|http://deb.debian.org|https://deb.debian.org|g' "$source"; \
            fi; \
        done; \
    fi; \
    apt-get update; \
    apt-get install -y --no-install-recommends libpq-dev; \
    rm -rf /var/lib/apt/lists/*

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port 80
EXPOSE 8080

# Copy the published app
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "AmbulanceRider.dll"]
