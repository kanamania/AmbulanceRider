# Initialize Let's Encrypt certificates for app.globalexpress.co.tz

$domains = @("app.globalexpress.co.tz")
$rsa_key_size = 4096
$data_path = "./certbot"
$email = "admin@globalexpress.co.tz" # Replace with your email
$staging = 0 # Set to 1 if you're testing your setup to avoid hitting request limits

if (Test-Path $data_path) {
    $decision = Read-Host "Existing data found for $domains. Continue and replace existing certificate? (y/N)"
    if ($decision -ne "Y" -and $decision -ne "y") {
        exit
    }
}

if (!(Test-Path "$data_path/conf/options-ssl-nginx.conf") -or !(Test-Path "$data_path/conf/ssl-dhparams.pem")) {
    Write-Host "### Downloading recommended TLS parameters ..."
    New-Item -ItemType Directory -Force -Path "$data_path/conf" | Out-Null
    Invoke-WebRequest -Uri "https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/_internal/tls_configs/options-ssl-nginx.conf" -OutFile "$data_path/conf/options-ssl-nginx.conf"
    Invoke-WebRequest -Uri "https://raw.githubusercontent.com/certbot/certbot/master/certbot/certbot/ssl-dhparams.pem" -OutFile "$data_path/conf/ssl-dhparams.pem"
    Write-Host ""
}

Write-Host "### Creating dummy certificate for $domains ..."
$path = "/etc/letsencrypt/live/$($domains[0])"
New-Item -ItemType Directory -Force -Path "$data_path/conf/live/$($domains[0])" | Out-Null
docker-compose run --rm --entrypoint "openssl req -x509 -nodes -newkey rsa:$rsa_key_size -days 1 -keyout '$path/privkey.pem' -out '$path/fullchain.pem' -subj '/CN=localhost'" certbot
Write-Host ""

Write-Host "### Starting nginx ..."
docker-compose up --force-recreate -d web
Write-Host ""

Write-Host "### Deleting dummy certificate for $domains ..."
docker-compose run --rm --entrypoint "rm -Rf /etc/letsencrypt/live/$($domains[0]) && rm -Rf /etc/letsencrypt/archive/$($domains[0]) && rm -Rf /etc/letsencrypt/renewal/$($domains[0]).conf" certbot
Write-Host ""

Write-Host "### Requesting Let's Encrypt certificate for $domains ..."
# Join $domains to -d args
$domain_args = ""
foreach ($domain in $domains) {
    $domain_args += " -d $domain"
}

# Select appropriate email arg
if ($email -eq "") {
    $email_arg = "--register-unsafely-without-email"
} else {
    $email_arg = "--email $email"
}

# Enable staging mode if needed
$staging_arg = ""
if ($staging -ne 0) {
    $staging_arg = "--staging"
}

docker-compose run --rm --entrypoint "certbot certonly --webroot -w /var/www/certbot $staging_arg $email_arg $domain_args --rsa-key-size $rsa_key_size --agree-tos --force-renewal" certbot
Write-Host ""

Write-Host "### Reloading nginx ..."
docker-compose exec web nginx -s reload
