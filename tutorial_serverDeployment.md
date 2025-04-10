# **Azure DevOps: Docker Stack Deployment mit Auto-Update**

## **1. Server Vorbereitung (Ubuntu)**
```bash
# SSH Zugriff
ssh username@dein-server-ip

# Docker Installation
sudo apt-get update
sudo apt-get install -y apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update && sudo apt-get install -y docker-ce docker-ce-cli containerd.io

# Swarm initialisieren
sudo docker swarm init
```

## **2. GitHub Container Registry Login (only if repo is private)**
```bash
# Mit GitHub PAT authentifizieren (only if repo is private)
echo "dein_github_pat_token" | sudo docker login ghcr.io -u dein_github_username --password-stdin

# Docker container pullen
docker pull ghcr.io/mirci212/vcc_projekt:latest
```

## **3. Secrets erstellen**
```bash
# Alle Secrets anlegen
echo "***" | sudo docker secret create DB_SERVER -
echo "***" | sudo docker secret create DB_NAME -
echo "***" | sudo docker secret create DB_USER -
echo "***" | sudo docker secret create DB_PASSWORD -
echo "http://frontend.example.com" | sudo docker secret create FRONTEND_URL -
echo "***@htlvb.at" | sudo docker secret create MAIL_EMAIL -
echo "***" | sudo docker secret create MAIL_PASSWORD -
echo "***" | sudo docker secret create TENANT_ID -
echo "***" | sudo docker secret create CLIENT_ID -
echo "***" | sudo docker secret create CLIENT_SECRET -
echo "smtp.office365.com" | sudo docker secret create SMTP_SERVER -
echo "outlook.office365.com" | sudo docker secret create IMAP_SERVER -
```

## **4. docker-compose.yml erstellen**
```bash
cat << 'EOF' > docker-compose.yml
version: '3.7'

services:
  vcc-app:
    image: ghcr.io/mirci212/vcc_projekt:latest
    ports:
      - "80:8080"
      - "443:8081"
    deploy:
      replicas: 1
      update_config:
        parallelism: 1
        delay: 10s
    secrets:
      - DB_SERVER
      - DB_NAME
      - DB_USER
      - DB_PASSWORD
      - FRONTEND_URL
      - MAIL_EMAIL
      - MAIL_PASSWORD
      - TENANT_ID
      - CLIENT_ID
      - CLIENT_SECRET
      - SMTP_SERVER
      - IMAP_SERVER

secrets:
  DB_SERVER:
    external: true
  DB_NAME:
    external: true
  DB_USER:
    external: true
  DB_PASSWORD:
    external: true
  FRONTEND_URL:
    external: true
  MAIL_EMAIL:
    external: true
  MAIL_PASSWORD:
    external: true
  TENANT_ID:
    external: true
  CLIENT_ID:
    external: true
  CLIENT_SECRET:
    external: true
  SMTP_SERVER:
    external: true
  IMAP_SERVER:
    external: true
EOF
```

## **5. Stack deployen**
```bash
sudo docker stack deploy -c docker-compose.yml vcc
```

## **6. (Optional) Automatisches Weekly Update**
```bash
# Cronjob für wöchentliches Update (Montags 2 Uhr)
(crontab -l 2>/dev/null; echo "0 2 * * 1 /usr/bin/docker pull ghcr.io/mirci212/vcc_projekt:latest && /usr/bin/docker service update --image ghcr.io/mirci212/vcc_projekt:latest vcc_vcc-app") | crontab -
```

## **Überprüfung**
```bash
# Stack Status
sudo docker service ls

# Logs anzeigen
sudo docker service logs vcc_vcc-app

# Secrets prüfen
sudo docker secret ls
```
## **Admin Benutzer**
Username: admin1 <br/>
Passwort: !Passw0rd

## **Troubleshooting**
```bash
# Falls Update hängt:
sudo docker service update --force vcc_vcc-app

# Bei Port-Konflikten:
sudo netstat -tulnp | grep ':80\|:443'
```

## **Wichtige Hinweise**
1. Ersetze `dein_github_pat_token` mit einem GitHub Personal Access Token (mit `read:packages` Berechtigung)
2. Der Cronjob läuft als root - sicherstellen dass der Docker Daemon für root läuft
3. Für Produktionsumgebungen HTTPS mit Traefik/Nginx hinzufügen
4. Backups der Secrets erstellen mit:
```bash
sudo docker secret inspect SECRET_NAME
```

Diese Lösung bietet:
- Sichere Secrets-Verwaltung via Swarm
- Automatische wöchentliche Updates
- Einfaches Monitoring via `docker service` Befehlen
- Skalierbarkeit für zukünftige Erweiterungen
