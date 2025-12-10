# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef --version 9.0.10
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy csproj and restore dependencies
COPY ["AmbulanceRider.API.csproj", "./"]
RUN dotnet restore "AmbulanceRider.API.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "AmbulanceRider.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AmbulanceRider.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# This should be your first RUN command after FROM
RUN apt-get update && apt-get install -y debian-archive-keyring && rm -rf /var/lib/apt/lists/*

# Install curl for health checks (ensure HTTPS sources to avoid signature issues)
RUN set -eux; \
    # ... sed commands ... \
    apt-get update; \
    # Try installing the keyring and curl in one go
    apt-get install -y --no-install-recommends debian-archive-keyring curl; \
    # Final cleanup
    rm -rf /var/lib/apt/lists/*

# Copy the published app and dotnet tools
COPY --from=publish /app/publish .
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

# Health check
HEALTHCHECK --interval=10s --timeout=5s --start-period=20s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "AmbulanceRider.API.dll"]
