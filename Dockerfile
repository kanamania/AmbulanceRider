# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Install Node.js for npm packages
RUN apt-get update && apt-get install -y \
    curl \
    && curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*

# Copy csproj and restore as distinct layers
COPY ["AmbulanceRider/AmbulanceRider.csproj", "AmbulanceRider/"]
RUN dotnet restore "AmbulanceRider/AmbulanceRider.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/AmbulanceRider"
RUN dotnet publish "AmbulanceRider.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Serve the application from Nginx
FROM nginx:alpine

# Install curl for healthcheck
RUN apk add --no-cache curl

# Set working directory
WORKDIR /usr/share/nginx/html

# Copy the built app to Nginx
RUN rm -rf ./*
COPY --from=build /app/publish/wwwroot .

# Copy the Nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Set proper permissions
RUN chown -R nginx:nginx /var/cache/nginx && \
    chown -R nginx:nginx /var/log/nginx && \
    chown -R nginx:nginx /etc/nginx/conf.d && \
    touch /var/run/nginx.pid && \
    chown -R nginx:nginx /var/run/nginx.pid && \
    chown -R nginx:nginx /usr/share/nginx/html && \
    chmod -R 755 /usr/share/nginx/html

# Ensure the nginx user has write access to the temp directories
RUN mkdir -p /var/lib/nginx/tmp \
    && chown -R nginx:nginx /var/lib/nginx \
    && chmod 700 /var/lib/nginx/tmp

# Switch to non-root user
USER nginx

# Expose ports 80 and 443
EXPOSE 80
EXPOSE 443

# Health check
HEALTHCHECK --interval=10s --timeout=5s --start-period=20s --retries=3 \
    CMD curl -f http://localhost/ || exit 1

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
