# Screenshots para Medium Article

## 📸 Screenshots Requeridos para el Artículo

### 🎯 **Screenshots Principales:**

1. **grafana-main-dashboard.png**
   - Dashboard principal con métricas RED
   - Muestra: Request rate, Error rate, Response time, Active requests
   - **Status**: ⏳ Pendiente de capturar

2. **grafana-business-metrics.png**
   - Dashboard de métricas de negocio
   - Muestra: Risk calculations por tipo, distribución de niveles
   - **Status**: ⏳ Pendiente de capturar

3. **grafana-traces-integration.png**
   - Integración de Jaeger con Grafana
   - Muestra: Traces en tiempo real
   - **Status**: ⏳ Pendiente de capturar

4. **grafana-alerts-dashboard.png**
   - Dashboard de alertas
   - Muestra: Configuración de alertas y notificaciones
   - **Status**: ⏳ Pendiente de capturar

### 🛠️ **Screenshots de Proceso:**

5. **grafana-dashboard-creation.png**
   - Proceso de creación de dashboard
   - Muestra: Panel de configuración
   - **Status**: ⏳ Pendiente de capturar

6. **grafana-business-queries.png**
   - Queries de PromQL para métricas de negocio
   - Muestra: Consultas personalizadas
   - **Status**: ⏳ Pendiente de capturar

### 📋 **Instrucciones para Capturar:**

1. **Configurar entorno local:**
   ```bash
   docker-compose -f docker-compose.observability.yml up -d
   ```

2. **Generar datos de prueba:**
   ```bash
   # Ejecutar el script de tráfico que ya corrimos
   ```

3. **Acceder a Grafana:**
   - URL: http://localhost:3001
   - Usuario: admin
   - Password: admin123

4. **Configuraciones de captura:**
   - Resolución: 1920x1080 mínimo
   - Tema: Dark theme de Grafana
   - Tiempo: "Last 30 minutes" con refresh 5s
   - Browser: Pantalla completa, sin bookmarks

### 🎨 **Mejoras visuales:**

- Usar callouts para destacar métricas clave
- Incluir timestamps para mostrar datos en tiempo real
- Highlighting de valores críticos
- Annotations explicativas

### 📈 **Impacto esperado:**

✅ **Credibilidad**: Prueba que el sistema funciona  
✅ **Engagement**: Contenido visual atrae más lectores  
✅ **Learning**: Facilita comprensión de conceptos  
✅ **Validation**: Lectores pueden comparar resultados  

---

**Nota**: Una vez capturadas las screenshots, actualizar el status a ✅ Completado
