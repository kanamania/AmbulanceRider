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

# Corrected RUN command
RUN set -eux; \
    # 1. Update package lists, allowing unauthenticated packages temporarily.
    #    This is the only way to get the list to find the keyring package.
    apt-get update --allow-unauthenticated; \
    \
    # 2. Install the official Debian keyrings. This fixes the root problem.
    apt-get install -y --no-install-recommends debian-archive-keyring; \
    \
    # 3. Perform a clean, secure update (now that the keys are installed).
    apt-get update; \
    \
    # 4. Install your desired package (e.g., curl, which you needed in the previous step).
    #    (You didn't ask for a specific package here, but you would add it here)
    # apt-get install -y --no-install-recommends curl; \
    \
    # 5. Clean up the cache.
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
