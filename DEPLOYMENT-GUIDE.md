# AmbulanceRider Deployment Guide

## Overview

This guide covers deploying the AmbulanceRider application in both development and production environments.

## Environment Configuration

### Development (Current Setup)
- **Web**: `http://localhost:8080` (HTTP), `https://localhost:8443` (HTTPS)
- **API**: `http://localhost:5000`
- **Database**: `localhost:5433`
- **Environment**: Development
- **SSL**: Self-signed or none

### Production
- **Web**: `http://app.globalexpress.co.tz` → `https://app.globalexpress.co.tz`
- **API**: `http://api:8080` (internal)
- **Database**: Internal (port 5432)
- **Environment**: Production
- **SSL**: Let's Encrypt certificates

## Switching to Production

### Step 1: Update docker-compose.yaml

Change the web service ports (lines 46-47):

```yaml
# FROM (Development):
ports:
  - "8080:80"
  - "8443:443"

# TO (Production):
ports:
  - "80:80"
  - "443:443"
```

Change environment (line 49):

```yaml
# FROM:
- ASPNETCORE_ENVIRONMENT=Development

# TO:
- ASPNETCORE_ENVIRONMENT=Production
```

### Step 2: Configure SSL Certificates

1. **Update email** in `init-letsencrypt.ps1`:
   ```powershell
   $email = "your-email@globalexpress.co.tz"
   ```

2. **Verify DNS** points to your server:
   ```powershell
   nslookup app.globalexpress.co.tz
   ```

3. **Run the SSL setup script**:
   ```powershell
   .\init-letsencrypt.ps1
   ```

### Step 3: Deploy

```powershell
# Stop current containers
docker-compose down

# Build and start with production config
docker-compose up -d --build

# Verify all services are healthy
docker ps
```

### Step 4: Verify Deployment

1. **Check web access**: `https://app.globalexpress.co.tz`
2. **Check API health**: `http://localhost:5000/health`
3. **Check SSL certificate**: Browser should show valid certificate
4. **Check logs**:
   ```powershell
   docker logs web
   docker logs api
   docker logs db
   docker logs certbot
   ```

## Port Configuration Summary

| Service | Development | Production | Internal |
|---------|-------------|------------|----------|
| Web HTTP | 8080 | 80 | 80 |
| Web HTTPS | 8443 | 443 | 443 |
| API | 5000 | 5000 | 8080 |
| Database | 5433 | 5433 | 5432 |

## SSL Certificate Management

### Automatic Renewal
- Certificates renew automatically every 12 hours
- Nginx reloads every 6 hours to pick up renewed certificates
- No manual intervention required

### Manual Renewal
```powershell
docker-compose run --rm certbot renew
docker-compose exec web nginx -s reload
```

### Check Certificate Status
```powershell
docker-compose run --rm certbot certificates
```

## Troubleshooting

### Port Already in Use

**Error**: `Bind for 0.0.0.0:80 failed: port is already allocated`

**Solution**:
1. Check what's using the port:
   ```powershell
   netstat -ano | findstr :80
   ```
2. Stop the conflicting service or change ports in docker-compose.yaml

### SSL Certificate Request Failed

**Possible causes**:
1. **DNS not configured**: Ensure domain points to server IP
2. **Firewall blocking**: Open ports 80 and 443
3. **Rate limiting**: Use staging mode first (`$staging = 1`)

**Check**:
```powershell
# DNS
nslookup app.globalexpress.co.tz

# Ports
Test-NetConnection -ComputerName app.globalexpress.co.tz -Port 80
Test-NetConnection -ComputerName app.globalexpress.co.tz -Port 443

# Logs
docker logs certbot
docker logs web
```

### Container Unhealthy

**Check health status**:
```powershell
docker ps
```

**Check logs**:
```powershell
docker logs <container_name>
```

**Restart container**:
```powershell
docker-compose restart <service_name>
```

## Rollback to Development

If you need to rollback to development:

1. **Update docker-compose.yaml**:
   ```yaml
   ports:
     - "8080:80"
     - "8443:443"
   environment:
     - ASPNETCORE_ENVIRONMENT=Development
   ```

2. **Restart services**:
   ```powershell
   docker-compose down
   docker-compose up -d
   ```

## Security Checklist

- [ ] SSL certificates configured and valid
- [ ] Database password is strong and secure
- [ ] API endpoints require authentication
- [ ] CORS configured correctly
- [ ] Firewall rules in place
- [ ] Regular backups configured
- [ ] Monitoring and logging enabled
- [ ] Environment variables not committed to Git

## Backup Strategy

### Database Backup
```powershell
docker exec db pg_dump -U ambulance_rider ambulance_rider > backup_$(Get-Date -Format "yyyyMMdd_HHmmss").sql
```

### SSL Certificates Backup
```powershell
Copy-Item -Recurse ./certbot/conf ./backups/certbot_$(Get-Date -Format "yyyyMMdd")
```

### Restore Database
```powershell
Get-Content backup_20250106_052000.sql | docker exec -i db psql -U ambulance_rider -d ambulance_rider
```

## Monitoring

### Check Container Status
```powershell
docker ps
docker stats
```

### View Logs
```powershell
# Real-time logs
docker-compose logs -f

# Specific service
docker logs -f web
docker logs -f api

# Last 100 lines
docker logs --tail 100 api
```

### Health Checks
- **Web**: `http://localhost:8080` (dev) or `https://app.globalexpress.co.tz` (prod)
- **API**: `http://localhost:5000/health`
- **Database**: `docker exec db pg_isready -U ambulance_rider`

## Performance Optimization

### Nginx Caching
Already configured in `nginx.conf`:
- Static assets: 1 year cache
- HTML files: no cache
- Gzip compression enabled

### Database
- Connection pooling configured in API
- Indexes on frequently queried fields
- Regular VACUUM and ANALYZE

### Docker
- Multi-stage builds reduce image size
- Health checks ensure service availability
- Restart policies handle failures

## Support

For issues:
1. Check logs: `docker logs <container>`
2. Review documentation: `SSL-SETUP.md`, `SSL-QUICKSTART.md`
3. Verify configuration: `docker-compose config`
4. Test connectivity: `docker exec <container> <command>`

## Additional Resources

- **SSL Setup**: See `SSL-SETUP.md` for detailed SSL configuration
- **Quick Start**: See `SSL-QUICKSTART.md` for quick reference
- **API Documentation**: See `API_DOCUMENTATION.md`
- **Features**: See `FEATURES_OVERVIEW.md`
