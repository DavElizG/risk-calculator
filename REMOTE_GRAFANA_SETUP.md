# Configuración para Grafana Cloud

## 🌐 Acceso Remoto a Métricas desde Grafana Cloud

### 📋 Pasos para configurar acceso remoto:

#### 1. **Exponer Prometheus públicamente con ngrok:**

```bash
# Instalar ngrok si no lo tienes
# https://ngrok.com/download

# Exponer Prometheus en puerto 9090
ngrok http 9090
```

**Resultado:**
- URL pública: `https://xxxxx-xx-xx-xxx-xxx.ngrok.io`
- Esta URL apunta a tu Prometheus local en puerto 9090

#### 2. **Configurar datasource en Grafana Cloud:**

**Datasource Settings:**
- **Name**: Risk Calculator Prometheus (Local)
- **Type**: Prometheus
- **URL**: `https://xxxxx-xx-xx-xxx-xxx.ngrok.io` (URL de ngrok)
- **Access**: Server (default)
- **HTTP Method**: GET

#### 3. **Alternativa: Reverse Proxy con CloudFlare Tunnel**

```bash
# Instalar cloudflared
# https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/

# Crear túnel para Prometheus
cloudflared tunnel --url http://localhost:9090
```

#### 4. **Configuración de CORS en Prometheus**

Si tienes problemas de CORS, agregar a prometheus.yml:

```yaml
global:
  external_labels:
    monitor: 'risk-calculator-monitor'

# Habilitar CORS
rule_files: []

# Configuración web para CORS
web:
  enable-admin-api: true
  cors:
    allowed-origins: 
      - "https://*.grafana.net"
      - "https://grafana.com"
```

### 🔐 **Configuración de Seguridad:**

#### **Autenticación básica para Prometheus:**

```yaml
# Agregar a prometheus.yml
basic_auth_users:
  admin: $2y$10$...hash...  # bcrypt hash de la password
```

#### **Variables de entorno para Docker:**

```yaml
prometheus:
  image: prom/prometheus:latest
  environment:
    - PROMETHEUS_WEB_EXTERNAL_URL=https://your-ngrok-url.ngrok.io
  ports:
    - "9090:9090"
```

### 📊 **URLs de Acceso:**

#### **Local Development:**
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3001
- Jaeger: http://localhost:16686
- API: http://localhost:8080

#### **Acceso Remoto (via ngrok):**
- Prometheus: https://xxxxx.ngrok.io
- Métricas endpoint: https://risk-calculator-api.onrender.com/metrics

### 🎯 **Configuración Completa Grafana Cloud:**

#### **Datasources a configurar:**

1. **Production API (Render):**
   - URL: `https://risk-calculator-api.onrender.com/metrics`
   - Tipo: Prometheus (via push)

2. **Local Prometheus (via ngrok):**
   - URL: Tu ngrok URL
   - Tipo: Prometheus

3. **Jaeger (si lo expones):**
   - URL: Tu ngrok URL para puerto 16686
   - Tipo: Jaeger

#### **Dashboard Import:**
- Usar el JSON del dashboard que tienes en monitoring/
- Configurar variables para switchear entre datasources

### ⚡ **Comandos rápidos:**

```bash
# Levantar stack completo
docker-compose -f docker-compose.monitoring.yml -f docker-compose.tracing.yml up -d

# Verificar servicios
docker ps

# Exponer Prometheus
ngrok http 9090

# Verificar métricas
curl http://localhost:9090/api/v1/query?query=up
```

### 🎯 **Beneficios del acceso remoto:**

✅ **Monitoring desde cualquier lugar**
✅ **Dashboards profesionales en Grafana Cloud**
✅ **No dependes de infraestructura local**
✅ **Mejor para demos y presentaciones**
✅ **Integración con alerting de Grafana Cloud**

### 🔍 **Troubleshooting:**

**Problema**: No se conecta Grafana Cloud
**Solución**: Verificar que ngrok esté corriendo y la URL sea accesible

**Problema**: CORS errors
**Solución**: Configurar headers CORS en Prometheus

**Problema**: Métricas vacías
**Solución**: Verificar que el endpoint /metrics esté funcionando

---

**Next Steps:**
1. Instalar ngrok
2. Exponer Prometheus con ngrok http 9090
3. Configurar datasource en Grafana Cloud
4. Importar dashboards
5. ¡Disfrutar del monitoring remoto! 🚀
