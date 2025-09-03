using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using RiskCalculator.API.Services;
using RiskCalculator.API.Models.Requests;
using RiskCalculator.API.Models.DTOs;
using RiskCalculator.API.Middleware;

namespace RiskCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RiskCalculatorController : ControllerBase
{
    private readonly IRiskCalculationService _riskCalculationService;
    private readonly IValidator<CalculateRiskRequest> _calculateRiskValidator;
    private readonly IValidator<SaveHistoryRequest> _saveHistoryValidator;
    private readonly ILogger<RiskCalculatorController> _logger;

    public RiskCalculatorController(
        IRiskCalculationService riskCalculationService,
        IValidator<CalculateRiskRequest> calculateRiskValidator,
        IValidator<SaveHistoryRequest> saveHistoryValidator,
        ILogger<RiskCalculatorController> logger)
    {
        _riskCalculationService = riskCalculationService;
        _calculateRiskValidator = calculateRiskValidator;
        _saveHistoryValidator = saveHistoryValidator;
        _logger = logger;
    }

    /// <summary>
    /// Calculate cybersecurity risk based on threat and vulnerability levels
    /// </summary>
    [HttpPost("calculate")]
    public async Task<ActionResult<ApiResponseDto<RiskCalculationResponseDto>>> CalculateRisk([FromBody] CalculateRiskRequest request)
    {
        try
        {
            _logger.LogInformation("Risk calculation requested for threat level {ThreatLevel}, vulnerability level {VulnerabilityLevel}", 
                request.ThreatLevel, request.VulnerabilityLevel);

            var validationResult = await _calculateRiskValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponseDto<RiskCalculationResponseDto>
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    Message = "Validation failed"
                });
            }

            var result = await _riskCalculationService.CalculateRiskAsync(request);
            
            // Registrar métricas de negocio
            MetricsMiddleware.RecordRiskCalculation(
                result.RiskLevel, 
                request.ThreatType ?? "Unknown", 
                request.VulnerabilityCategory ?? "Unknown"
            );
            
            _logger.LogInformation("Risk calculation completed. Risk Score: {RiskScore}, Risk Level: {RiskLevel}", 
                result.RiskScore, result.RiskLevel);

            return Ok(new ApiResponseDto<RiskCalculationResponseDto>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating risk");
            return StatusCode(500, new ApiResponseDto<RiskCalculationResponseDto>
            {
                Success = false,
                Errors = new List<string> { "An unexpected error occurred" },
                Message = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Get the risk assessment matrix
    /// </summary>
    [HttpGet("matrix")]
    public async Task<ActionResult<ApiResponseDto<RiskMatrixResponseDto>>> GetRiskMatrix()
    {
        try
        {
            var matrix = await _riskCalculationService.GetRiskMatrixAsync();
            return Ok(new ApiResponseDto<RiskMatrixResponseDto>
            {
                Success = true,
                Data = matrix
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving risk matrix");
            return StatusCode(500, new ApiResponseDto<RiskMatrixResponseDto>
            {
                Success = false,
                Errors = new List<string> { "Failed to retrieve risk matrix" },
                Message = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Save risk calculation to history
    /// </summary>
    [HttpPost("history")]
    public async Task<ActionResult<ApiResponseDto<RiskHistoryDto>>> SaveToHistory([FromBody] SaveHistoryRequest request)
    {
        try
        {
            var validationResult = await _saveHistoryValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponseDto<RiskHistoryDto>
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    Message = "Validation failed"
                });
            }

            var result = await _riskCalculationService.SaveToHistoryAsync(request);
            return Ok(new ApiResponseDto<RiskHistoryDto>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving to history");
            return StatusCode(500, new ApiResponseDto<RiskHistoryDto>
            {
                Success = false,
                Errors = new List<string> { "Failed to save to history" },
                Message = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Get risk calculation history
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponseDto<PaginatedResponseDto<RiskHistoryDto>>>> GetHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? userIdentifier = null)
    {
        try
        {
            var history = await _riskCalculationService.GetHistoryAsync(pageNumber, pageSize, userIdentifier);
            return Ok(new ApiResponseDto<PaginatedResponseDto<RiskHistoryDto>>
            {
                Success = true,
                Data = history
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving history");
            return StatusCode(500, new ApiResponseDto<PaginatedResponseDto<RiskHistoryDto>>
            {
                Success = false,
                Errors = new List<string> { "Failed to retrieve history" },
                Message = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Get available threat types
    /// </summary>
    [HttpGet("threat-types")]
    public async Task<ActionResult<ApiResponseDto<List<ThreatTypeDto>>>> GetThreatTypes()
    {
        try
        {
            var threatTypes = await _riskCalculationService.GetThreatTypesAsync();
            return Ok(new ApiResponseDto<List<ThreatTypeDto>>
            {
                Success = true,
                Data = threatTypes
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving threat types");
            return StatusCode(500, new ApiResponseDto<List<ThreatTypeDto>>
            {
                Success = false,
                Errors = new List<string> { "Failed to retrieve threat types" },
                Message = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Get available vulnerability categories
    /// </summary>
    [HttpGet("vulnerability-categories")]
    public async Task<ActionResult<ApiResponseDto<List<VulnerabilityCategoryDto>>>> GetVulnerabilityCategories()
    {
        try
        {
            var categories = await _riskCalculationService.GetVulnerabilityCategoriesAsync();
            return Ok(new ApiResponseDto<List<VulnerabilityCategoryDto>>
            {
                Success = true,
                Data = categories
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vulnerability categories");
            return StatusCode(500, new ApiResponseDto<List<VulnerabilityCategoryDto>>
            {
                Success = false,
                Errors = new List<string> { "Failed to retrieve vulnerability categories" },
                Message = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Get risk mitigation recommendations
    /// </summary>
    [HttpGet("recommendations")]
    public async Task<ActionResult<ApiResponseDto<List<RecommendationDto>>>> GetRecommendations(
        [FromQuery] string riskLevel,
        [FromQuery] string? threatType = null,
        [FromQuery] string? vulnerabilityCategory = null)
    {
        try
        {
            if (string.IsNullOrEmpty(riskLevel))
            {
                return BadRequest(new ApiResponseDto<List<RecommendationDto>>
                {
                    Success = false,
                    Errors = new List<string> { "Risk level is required" },
                    Message = "Validation failed"
                });
            }

            var recommendations = await _riskCalculationService.GetRecommendationsAsync(riskLevel, threatType, vulnerabilityCategory);
            return Ok(new ApiResponseDto<List<RecommendationDto>>
            {
                Success = true,
                Data = recommendations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recommendations");
            return StatusCode(500, new ApiResponseDto<List<RecommendationDto>>
            {
                Success = false,
                Errors = new List<string> { "Failed to retrieve recommendations" },
                Message = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public ActionResult<ApiResponseDto<object>> HealthCheck()
    {
        return Ok(new ApiResponseDto<object>
        {
            Success = true,
            Data = new 
            { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                service = "Risk Calculator API"
            }
        });
    }

    /// <summary>
    /// Test endpoint to generate 400 errors for observability testing
    /// </summary>
    [HttpGet("test/error/400")]
    public ActionResult<ApiResponseDto<object>> TestBadRequest()
    {
        _logger.LogWarning("Test endpoint called - generating 400 Bad Request error");
        return BadRequest(new ApiResponseDto<object>
        {
            Success = false,
            Errors = new List<string> { "This is an intentional 400 error for testing observability" },
            Message = "Bad Request Test"
        });
    }

    /// <summary>
    /// Test endpoint to generate 404 errors for observability testing
    /// </summary>
    [HttpGet("test/error/404")]
    public ActionResult<ApiResponseDto<object>> TestNotFound()
    {
        _logger.LogWarning("Test endpoint called - generating 404 Not Found error");
        return NotFound(new ApiResponseDto<object>
        {
            Success = false,
            Errors = new List<string> { "This is an intentional 404 error for testing observability" },
            Message = "Not Found Test"
        });
    }

    /// <summary>
    /// Test endpoint to generate 500 errors for observability testing
    /// </summary>
    [HttpGet("test/error/500")]
    public ActionResult<ApiResponseDto<object>> TestInternalServerError()
    {
        _logger.LogError("Test endpoint called - generating 500 Internal Server Error");
        throw new Exception("This is an intentional 500 error for testing observability and distributed tracing");
    }

    /// <summary>
    /// Test endpoint with slow response for latency testing
    /// </summary>
    [HttpGet("test/slow")]
    public async Task<ActionResult<ApiResponseDto<object>>> TestSlowResponse()
    {
        _logger.LogInformation("Test slow endpoint called - simulating slow response");
        
        var random = new Random();
        var delay = random.Next(2000, 5000); // 2-5 seconds delay
        
        await Task.Delay(delay);
        
        _logger.LogInformation("Slow response completed after {Delay}ms", delay);
        
        return Ok(new ApiResponseDto<object>
        {
            Success = true,
            Data = new
            {
                Message = "Slow response completed",
                DelayMs = delay,
                Timestamp = DateTime.UtcNow
            }
        });
    }

    /// <summary>
    /// Test endpoint to generate multiple risk calculations for metrics
    /// </summary>
    [HttpGet("test/bulk-calculations")]
    public async Task<ActionResult<ApiResponseDto<List<object>>>> TestBulkCalculations()
    {
        _logger.LogInformation("Test bulk calculations endpoint called");
        
        var results = new List<object>();
        var threatTypes = new[] { "Malware", "Phishing", "Data Breach", "SQL Injection", "DDoS" };
        var vulnerabilityCategories = new[] { "Critical", "High", "Medium", "Low" };
        var random = new Random();

        for (int i = 0; i < 10; i++)
        {
            var threatType = threatTypes[random.Next(threatTypes.Length)];
            var vulnCategory = vulnerabilityCategories[random.Next(vulnerabilityCategories.Length)];
            var threatLevel = random.Next(1, 6);
            var vulnLevel = random.Next(1, 6);

            // Simulate a calculation
            var riskScore = threatLevel * vulnLevel;
            var riskLevel = riskScore switch
            {
                >= 20 => "Critical",
                >= 15 => "High", 
                >= 10 => "Medium",
                >= 5 => "Low",
                _ => "Minimal"
            };

            // Record metrics
            MetricsMiddleware.RecordRiskCalculation(riskLevel, threatType, vulnCategory);

            results.Add(new
            {
                ThreatType = threatType,
                VulnerabilityCategory = vulnCategory,
                ThreatLevel = threatLevel,
                VulnerabilityLevel = vulnLevel,
                RiskScore = riskScore,
                RiskLevel = riskLevel
            });

            // Small delay to spread out the metrics
            await Task.Delay(100);
        }

        _logger.LogInformation("Generated {Count} test calculations", results.Count);

        return Ok(new ApiResponseDto<List<object>>
        {
            Success = true,
            Data = results
        });
    }
}
