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

# Install required runtime dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    libpq-dev \
    && rm -rf /var/lib/apt/lists/*

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port 80
EXPOSE 8080

# Copy the published app
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "AmbulanceRider.dll"]
