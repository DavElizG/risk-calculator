using Prometheus;
using System.Diagnostics;

namespace RiskCalculator.API.Middleware
{
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MetricsMiddleware> _logger;

        // Métricas de Prometheus
        private static readonly Counter TotalRequests = Metrics
            .CreateCounter("http_requests_total", "Total number of HTTP requests", 
                new[] { "method", "endpoint", "status_code" });

        private static readonly Histogram RequestDuration = Metrics
            .CreateHistogram("http_request_duration_seconds", "Duration of HTTP requests in seconds",
                new[] { "method", "endpoint" });

        private static readonly Counter ErrorsTotal = Metrics
            .CreateCounter("http_errors_total", "Total number of HTTP errors",
                new[] { "method", "endpoint", "status_code", "error_type" });

        private static readonly Gauge ActiveRequests = Metrics
            .CreateGauge("http_requests_active", "Number of active HTTP requests",
                new[] { "method", "endpoint" });

        private static readonly Counter RiskCalculationsTotal = Metrics
            .CreateCounter("risk_calculations_total", "Total number of risk calculations",
                new[] { "risk_level", "threat_type", "vulnerability_category" });

        public MetricsMiddleware(RequestDelegate next, ILogger<MetricsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var method = context.Request.Method;
            var endpoint = GetEndpointName(context.Request.Path);

            // Incrementar requests activos
            ActiveRequests.WithLabels(method, endpoint).Inc();

            try
            {
                await _next(context);

                // Métricas de respuesta exitosa
                var statusCode = context.Response.StatusCode.ToString();
                TotalRequests.WithLabels(method, endpoint, statusCode).Inc();
                
                // Log de request exitoso
                _logger.LogInformation("Request completed: {Method} {Endpoint} - {StatusCode} in {Duration}ms",
                    method, endpoint, statusCode, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                // Métricas de error
                var statusCode = "500";
                var errorType = ex.GetType().Name;
                
                TotalRequests.WithLabels(method, endpoint, statusCode).Inc();
                ErrorsTotal.WithLabels(method, endpoint, statusCode, errorType).Inc();

                // Log detallado del error
                _logger.LogError(ex, "Request failed: {Method} {Endpoint} - Error: {ErrorType} in {Duration}ms",
                    method, endpoint, errorType, sw.ElapsedMilliseconds);

                throw; // Re-throw para que el GlobalExceptionMiddleware lo maneje
            }
            finally
            {
                sw.Stop();
                
                // Métricas de duración y requests activos
                RequestDuration.WithLabels(method, endpoint).Observe(sw.Elapsed.TotalSeconds);
                ActiveRequests.WithLabels(method, endpoint).Dec();
            }
        }

        private static string GetEndpointName(PathString path)
        {
            var pathString = path.ToString().ToLowerInvariant();
            
            // Normalizar endpoints para métricas
            if (pathString.StartsWith("/api/riskcalculator"))
                return "/api/riskcalculator";
            if (pathString.StartsWith("/swagger"))
                return "/swagger";
            if (pathString.StartsWith("/metrics"))
                return "/metrics";
            if (pathString == "/")
                return "/";
            
            return "unknown";
        }

        // Método para registrar métricas de cálculo de riesgo
        public static void RecordRiskCalculation(string riskLevel, string threatType, string vulnerabilityCategory)
        {
            RiskCalculationsTotal.WithLabels(riskLevel, threatType, vulnerabilityCategory).Inc();
        }
    }
}
