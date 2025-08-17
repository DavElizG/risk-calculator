using System.ComponentModel.DataAnnotations;

namespace RiskCalculator.API.Models.Requests;

public class CalculateRiskRequest
{
    [Required]
    [Range(1, 10, ErrorMessage = "Threat level must be between 1 and 10")]
    public int ThreatLevel { get; set; }

    [Required]
    [Range(1, 10, ErrorMessage = "Vulnerability level must be between 1 and 10")]
    public int VulnerabilityLevel { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Threat type cannot exceed 100 characters")]
    public string ThreatType { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "Vulnerability category cannot exceed 100 characters")]
    public string VulnerabilityCategory { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Asset value cannot exceed 200 characters")]
    public string? AssetValue { get; set; }

    public string? Description { get; set; }

    public Dictionary<string, object>? AdditionalMetadata { get; set; }
}
