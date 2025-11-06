# SSL Certificate Quick Start Guide

## Quick Setup (5 Minutes)

### 1. Update Email in Script

Edit `init-letsencrypt.ps1`:
```powershell
$email = "your-email@globalexpress.co.tz"  # Line 5
```

### 2. Verify DNS

Make sure `app.globalexpress.co.tz` points to your server:
```powershell
nslookup app.globalexpress.co.tz
```

### 3. Run the Setup Script

```powershell
.\init-letsencrypt.ps1
```

### 4. Wait for Completion

The script will:
- ✓ Download SSL configuration files
- ✓ Create temporary certificate
- ✓ Start Nginx
- ✓ Request Let's Encrypt certificate
- ✓ Reload Nginx with real certificate

### 5. Test Your Site

Visit: `https://app.globalexpress.co.tz`

## What Changed

### docker-compose.yaml
- Added port 443 for HTTPS
- Added `certbot` service for automatic renewal
- Added volumes for SSL certificates
- Changed environment to Production

### nginx.conf
- Added HTTP to HTTPS redirect
- Added SSL configuration
- Added certificate paths
- Added Let's Encrypt challenge location

### Dockerfile
- Exposed port 443

## Automatic Renewal

Certificates renew automatically every 12 hours. No action needed!

## Troubleshooting

### "Certificate request failed"

**Check DNS:**
```powershell
nslookup app.globalexpress.co.tz
```
Should return your server's IP address.

**Check Ports:**
```powershell
Test-NetConnection -ComputerName app.globalexpress.co.tz -Port 80
Test-NetConnection -ComputerName app.globalexpress.co.tz -Port 443
```
Both should succeed.

**Check Logs:**
```powershell
docker logs web
docker logs certbot
```

### "Rate limit exceeded"

Use staging mode first:
```powershell
# In init-letsencrypt.ps1, line 6:
$staging = 1
```

Run the script, then switch back to production:
```powershell
$staging = 0
```

### "Nginx won't start"

Check Nginx configuration:
```powershell
docker-compose exec web nginx -t
```

## Manual Commands

**Renew certificates manually:**
```powershell
docker-compose run --rm certbot renew
docker-compose exec web nginx -s reload
```

**Check certificate expiration:**
```powershell
docker-compose run --rm certbot certificates
```

**Restart services:**
```powershell
docker-compose restart web certbot
```

## Production Checklist

- [ ] DNS points to server
- [ ] Ports 80 and 443 are open
- [ ] Email address updated in script
- [ ] Tested with staging first
- [ ] Production certificates obtained
- [ ] HTTPS site accessible
- [ ] HTTP redirects to HTTPS
- [ ] Certificate auto-renewal working

## Next Steps

1. **Update API calls**: Change API URLs from `http://` to `https://`
2. **Update CORS**: Update API CORS settings if needed
3. **Monitor certificates**: Check expiration dates regularly
4. **Backup certificates**: Store `certbot/conf/` backups securely

## Support

For detailed information, see `SSL-SETUP.md`
