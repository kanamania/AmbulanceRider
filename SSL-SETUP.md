# SSL Certificate Setup with Let's Encrypt

This guide will help you set up SSL certificates for your AmbulanceRider application using Let's Encrypt and Certbot.

## Prerequisites

1. Your domain `app.globalexpress.co.tz` must be pointing to your server's IP address
2. Ports 80 and 443 must be open on your firewall
3. Docker and Docker Compose must be installed

## Initial Setup

### Step 1: Update Email Address

Edit the `init-letsencrypt.ps1` file and replace the email address:

```powershell
$email = "your-email@globalexpress.co.tz"
```

### Step 2: Test with Staging (Optional but Recommended)

For your first run, it's recommended to test with Let's Encrypt staging environment to avoid hitting rate limits:

```powershell
$staging = 1  # Set to 1 for testing
```

### Step 3: Run the Initialization Script

On Windows (PowerShell):
```powershell
.\init-letsencrypt.ps1
```

On Linux/Mac:
```bash
chmod +x init-letsencrypt.sh
./init-letsencrypt.sh
```

### Step 4: Switch to Production

Once the staging certificates work, update the script:

```powershell
$staging = 0  # Set to 0 for production
```

Then run the script again to get production certificates.

## What the Script Does

1. **Downloads TLS parameters**: Gets recommended SSL configuration from Certbot
2. **Creates dummy certificate**: Temporary certificate to start Nginx
3. **Starts Nginx**: Brings up the web server
4. **Requests real certificate**: Uses Certbot to get Let's Encrypt certificate
5. **Reloads Nginx**: Applies the new certificate

## Certificate Renewal

Certificates are automatically renewed by the `certbot` container, which checks for renewal every 12 hours. The web container reloads Nginx every 6 hours to pick up renewed certificates.

## Manual Renewal

To manually renew certificates:

```powershell
docker-compose run --rm certbot renew
docker-compose exec web nginx -s reload
```

## Troubleshooting

### Certificate Request Fails

1. **Check DNS**: Ensure `app.globalexpress.co.tz` resolves to your server IP
   ```powershell
   nslookup app.globalexpress.co.tz
   ```

2. **Check Firewall**: Ensure ports 80 and 443 are open
   ```powershell
   Test-NetConnection -ComputerName app.globalexpress.co.tz -Port 80
   Test-NetConnection -ComputerName app.globalexpress.co.tz -Port 443
   ```

3. **Check Nginx Logs**:
   ```powershell
   docker logs web
   ```

4. **Check Certbot Logs**:
   ```powershell
   docker logs certbot
   ```

### Rate Limiting

Let's Encrypt has rate limits:
- 50 certificates per registered domain per week
- 5 duplicate certificates per week

Use staging environment (`$staging = 1`) for testing to avoid hitting these limits.

## File Structure

After setup, you'll have:

```
AmbulanceRider/
├── certbot/
│   ├── conf/
│   │   ├── live/
│   │   │   └── app.globalexpress.co.tz/
│   │   │       ├── fullchain.pem
│   │   │       ├── privkey.pem
│   │   │       └── ...
│   │   ├── options-ssl-nginx.conf
│   │   └── ssl-dhparams.pem
│   └── www/
│       └── .well-known/
│           └── acme-challenge/
```

## Security Notes

1. **Never commit certificates to Git**: The `certbot/` directory should be in `.gitignore`
2. **Backup certificates**: Store backups of `certbot/conf/` securely
3. **Monitor expiration**: Certificates expire after 90 days (auto-renewal handles this)

## Additional Configuration

### For API Subdomain

If you want a separate SSL certificate for an API subdomain (e.g., `api.globalexpress.co.tz`):

1. Update the `$domains` array in the script:
   ```powershell
   $domains = @("app.globalexpress.co.tz", "api.globalexpress.co.tz")
   ```

2. Update your Nginx configuration to handle the API subdomain

3. Run the initialization script again

## Support

For issues with:
- **Let's Encrypt**: https://letsencrypt.org/docs/
- **Certbot**: https://certbot.eff.org/docs/
- **Nginx SSL**: https://nginx.org/en/docs/http/configuring_https_servers.html
