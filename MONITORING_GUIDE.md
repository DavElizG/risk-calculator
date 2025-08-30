# 📊 Implementando Monitoreo Avanzado con Prometheus y Grafana en .NET 8

## 🎯 Introducción

En este artículo te mostraré cómo implementar un sistema de monitoreo completo para una aplicación .NET 8 Web API utilizando **Prometheus** para la recolección de métricas y **Grafana** para la visualización, todo orquestado con **Docker Compose**.

> **¿Por qué es importante el monitoreo?** En aplicaciones modernas, especialmente aquellas relacionadas con seguridad informática como nuestro Risk Calculator, es crucial tener visibilidad completa del comportamiento de la aplicación, errores, rendimiento y métricas de negocio.

## 🏗️ Arquitectura del Sistema

Nuestro sistema de monitoreo consta de 4 componentes principales:

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend       │    │   Prometheus    │    │   Grafana       │
│   (React)       │────│   (.NET 8 API)  │────│   (Metrics)     │────│   (Dashboard)   │
│   Port: 3000    │    │   Port: 8080    │    │   Port: 9090    │    │   Port: 3001    │
└─────────────────┘    └─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🚀 Paso 1: Configuración del Backend (.NET 8)

### 1.1 Instalación de Dependencias

Primero, agregamos el paquete de Prometheus para .NET:

```xml
<PackageReference Include="prometheus-net.AspNetCore" Version="8.2.1" />
```

### 1.2 Middleware Personalizado para Métricas

Creamos un middleware personalizado que captura métricas HTTP y de negocio:

```csharp
using Prometheus;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MetricsMiddleware> _logger;
    
    // Métricas HTTP
    private static readonly Counter TotalRequests = Metrics
        .CreateCounter("http_requests_total", "Total HTTP requests", 
            new[] { "method", "endpoint", "status_code" });
    
    private static readonly Counter ErrorsTotal = Metrics
        .CreateCounter("http_errors_total", "Total HTTP errors", 
            new[] { "method", "endpoint", "status_code", "error_type" });
    
    private static readonly Histogram RequestDuration = Metrics
        .CreateHistogram("http_request_duration_seconds", "HTTP request duration",
            new[] { "method", "endpoint" });
    
    private static readonly Gauge ActiveRequests = Metrics
        .CreateGauge("http_requests_active", "Active HTTP requests");
    
    // Métricas de negocio
    private static readonly Counter RiskCalculationsTotal = Metrics
        .CreateCounter("risk_calculations_total", "Total risk calculations performed",
            new[] { "risk_level", "threat_type", "vulnerability_category" });

    public MetricsMiddleware(RequestDelegate next, ILogger<MetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "unknown";
        var method = context.Request.Method;
        
        ActiveRequests.Inc();
        
        using var timer = RequestDuration.WithLabels(method, path).NewTimer();
        
        try
        {
            await _next(context);
            
            var statusCode = context.Response.StatusCode.ToString();
            TotalRequests.WithLabels(method, path, statusCode).Inc();
            
            if (statusCode.StartsWith("4") || statusCode.StartsWith("5"))
            {
                ErrorsTotal.WithLabels(method, path, statusCode, "http_error").Inc();
            }
        }
        catch (Exception ex)
        {
            ErrorsTotal.WithLabels(method, path, "500", ex.GetType().Name).Inc();
            _logger.LogError(ex, "Unhandled exception in request {Method} {Path}", method, path);
            throw;
        }
        finally
        {
            ActiveRequests.Dec();
        }
    }

    // Método para registrar métricas de negocio
    public static void RecordRiskCalculation(string riskLevel, string threatType, string vulnerabilityCategory)
    {
        RiskCalculationsTotal.WithLabels(riskLevel, threatType, vulnerabilityCategory).Inc();
    }
}
```

### 1.3 Configuración en Program.cs

```csharp
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// ... configuración existente ...

var app = builder.Build();

// Configuración de Prometheus
app.UseHttpMetrics(); // Métricas HTTP automáticas
app.UseMiddleware<MetricsMiddleware>(); // Nuestro middleware personalizado

// ... configuración existente ...

// Endpoint de métricas para Prometheus
app.MapMetrics(); // Expone /metrics

app.Run();
```

### 1.4 Integración en el Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class RiskCalculatorController : ControllerBase
{
    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate([FromBody] CalculateRiskRequest request)
    {
        try
        {
            var result = await _riskCalculationService.CalculateRiskAsync(request);
            
            // Registrar métrica de negocio
            MetricsMiddleware.RecordRiskCalculation(
                result.RiskLevel.ToString(),
                request.ThreatType,
                request.VulnerabilityCategory
            );
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating risk");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
```

## 🐳 Paso 2: Configuración con Docker Compose

### 2.1 Estructura de Archivos

```
monitoring/
├── prometheus.yml          # Configuración de Prometheus
├── grafana-datasources.yml # Configuración de datasources
└── risk-calculator-dashboard.json # Dashboard predefinido
```

### 2.2 Docker Compose para Monitoreo

```yaml
version: '3.8'

services:
  # Backend API
  risk-calculator-backend:
    build:
      context: ./backend/risk-calculator-api
      dockerfile: risk-calculator-api/Dockerfile
    container_name: risk-calculator-backend
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    networks:
      - risk-calculator-network

  # Frontend
  risk-calculator-frontend:
    build:
      context: ./frontend/my-app
      dockerfile: Dockerfile
    container_name: risk-calculator-frontend
    ports:
      - "3000:3000"
    networks:
      - risk-calculator-network

  # Prometheus
  prometheus:
    image: prom/prometheus:latest
    container_name: prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./monitoring/prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus_data:/prometheus
    command:
      - '--config.file=/etc/prometheus/prometheus.yml'
      - '--storage.tsdb.path=/prometheus'
      - '--web.console.libraries=/etc/prometheus/console_libraries'
      - '--web.console.templates=/etc/prometheus/consoles'
      - '--storage.tsdb.retention.time=200h'
      - '--web.enable-lifecycle'
    restart: unless-stopped
    networks:
      - risk-calculator-network

  # Grafana
  grafana:
    image: grafana/grafana:latest
    container_name: grafana
    ports:
      - "3001:3000"
    environment:
      - GF_SECURITY_ADMIN_USER=admin
      - GF_SECURITY_ADMIN_PASSWORD=admin123
      - GF_USERS_ALLOW_SIGN_UP=false
    volumes:
      - grafana_data:/var/lib/grafana
      - ./monitoring/grafana-datasources.yml:/etc/grafana/provisioning/datasources/datasources.yml
    restart: unless-stopped
    networks:
      - risk-calculator-network

volumes:
  prometheus_data:
  grafana_data:

networks:
  risk-calculator-network:
    driver: bridge
```

## ⚙️ Paso 3: Configuración de Prometheus

### 3.1 prometheus.yml

```yaml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

rule_files:
  # - "first_rules.yml"
  # - "second_rules.yml"

scrape_configs:
  # Prometheus se monitorea a sí mismo
  - job_name: 'prometheus'
    static_configs:
      - targets: ['localhost:9090']

  # Nuestro Risk Calculator API
  - job_name: 'risk-calculator-api'
    static_configs:
      - targets: ['risk-calculator-backend:80']
    metrics_path: '/metrics'
    scrape_interval: 5s
    scrape_timeout: 5s
```

### 3.2 Configuración del Datasource de Grafana

```yaml
apiVersion: 1

datasources:
  - name: 'Risk Calculator Prometheus'
    type: prometheus
    access: proxy
    url: http://prometheus:9090
    isDefault: true
    editable: true
    basicAuth: false
    orgId: 1
    jsonData:
      timeInterval: "5s"
      httpMethod: "GET"
```

## 📈 Paso 4: Métricas y Queries Importantes

### 4.1 Métricas de Infraestructura

```promql
# Verificar que los servicios están activos
up

# Requests HTTP totales
http_requests_total

# Duración de requests
rate(http_request_duration_seconds_sum[5m]) / rate(http_request_duration_seconds_count[5m])

# Requests activos
http_requests_active
```

### 4.2 Métricas de Negocio

```promql
# Total de cálculos de riesgo
risk_calculations_total

# Cálculos por tipo de amenaza
sum(rate(risk_calculations_total[5m])) by (threat_type)

# Distribución de niveles de riesgo
sum(rate(risk_calculations_total[5m])) by (risk_level)
```

### 4.3 Métricas de Error

```promql
# Tasa de errores
rate(http_errors_total[5m])

# Errores por endpoint
sum(rate(http_errors_total[5m])) by (endpoint)

# Porcentaje de errores
(rate(http_errors_total[5m]) / rate(http_requests_total[5m])) * 100
```

## 🎨 Paso 5: Creando Dashboards en Grafana

### 5.1 Dashboard para API Monitoring

**Paneles recomendados:**

1. **Request Rate**: `rate(http_requests_total[5m])`
2. **Error Rate**: `rate(http_errors_total[5m])`
3. **Response Time**: `http_request_duration_seconds`
4. **Active Requests**: `http_requests_active`
5. **Risk Calculations**: `rate(risk_calculations_total[5m])`

### 5.2 Alertas Importantes

```promql
# Alerta: API no disponible
up{job="risk-calculator-api"} == 0

# Alerta: Tasa de errores alta
rate(http_errors_total[5m]) > 0.1

# Alerta: Tiempo de respuesta alto
http_request_duration_seconds{quantile="0.95"} > 2
```

## 🚀 Paso 6: Despliegue y Monitoreo

### 6.1 Comandos para Ejecutar

```bash
# Construir y ejecutar todo el stack
docker-compose -f docker-compose.monitoring.yml up -d

# Verificar que todos los servicios están corriendo
docker ps

# Ver logs específicos
docker logs prometheus
docker logs grafana
docker logs risk-calculator-backend
```

### 6.2 Acceso a los Servicios

- **Frontend**: http://localhost:3000
- **API**: http://localhost:8080
- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3001 (admin/admin123)

## 📊 Resultados y Beneficios

### ✅ Lo que logramos:

1. **Visibilidad completa** del comportamiento de la aplicación
2. **Métricas de infraestructura** (CPU, memoria, requests)
3. **Métricas de negocio** (cálculos de riesgo por tipo)
4. **Detección temprana** de errores y problemas
5. **Dashboards profesionales** para stakeholders
6. **Alertas automáticas** para incidentes críticos

### 📈 Métricas Clave que Monitoreamos:

- **Performance**: Tiempo de respuesta, throughput
- **Disponibilidad**: Uptime, health checks
- **Errores**: Tasa de errores, tipos de excepciones
- **Negocio**: Cálculos por tipo de amenaza, niveles de riesgo
- **Recursos**: Memoria, CPU, requests activos

## 🎯 Conclusiones

Implementar un sistema de monitoreo robusto con Prometheus y Grafana nos proporciona:

1. **Proactividad**: Detectamos problemas antes de que afecten a los usuarios
2. **Insights de negocio**: Entendemos cómo se usa nuestra aplicación
3. **Mejora continua**: Datos para optimizar performance
4. **Confiabilidad**: Mayor estabilidad del sistema
5. **Transparencia**: Visibilidad para todo el equipo

## 🔗 Recursos Adicionales

- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [.NET Prometheus Integration](https://github.com/prometheus-net/prometheus-net)
- [PromQL Query Language](https://prometheus.io/docs/prometheus/latest/querying/)

---

**¿Te gustó este artículo?** 👏 Si implementaste este sistema de monitoreo, comparte tu experiencia en los comentarios. ¡Las métricas no mienten! 📊

**Tags**: #dotnet #prometheus #grafana #monitoring #devops #docker #cybersecurity #observability
