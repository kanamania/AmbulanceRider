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
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Copy the published app
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Copy nginx config
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
