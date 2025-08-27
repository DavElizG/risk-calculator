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
}
