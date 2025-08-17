using RiskCalculator.API.Models.DTOs;
using RiskCalculator.API.Models.Requests;

namespace RiskCalculator.API.Services;

public interface IRiskCalculationService
{
    /// <summary>
    /// Calculates risk based on threat and vulnerability levels
    /// </summary>
    Task<RiskCalculationResponseDto> CalculateRiskAsync(CalculateRiskRequest request);

    /// <summary>
    /// Gets the complete risk matrix
    /// </summary>
    Task<RiskMatrixResponseDto> GetRiskMatrixAsync();

    /// <summary>
    /// Saves a risk calculation to history
    /// </summary>
    Task<RiskHistoryDto> SaveToHistoryAsync(SaveHistoryRequest request);

    /// <summary>
    /// Gets risk calculation history with pagination
    /// </summary>
    Task<PaginatedResponseDto<RiskHistoryDto>> GetHistoryAsync(int pageNumber = 1, int pageSize = 10, string? userIdentifier = null);

    /// <summary>
    /// Gets available threat types
    /// </summary>
    Task<List<ThreatTypeDto>> GetThreatTypesAsync();

    /// <summary>
    /// Gets available vulnerability categories
    /// </summary>
    Task<List<VulnerabilityCategoryDto>> GetVulnerabilityCategoriesAsync();

    /// <summary>
    /// Gets recommendations based on risk level and types
    /// </summary>
    Task<List<RecommendationDto>> GetRecommendationsAsync(string riskLevel, string? threatType = null, string? vulnerabilityCategory = null);
}
