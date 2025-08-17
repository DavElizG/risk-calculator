using System.ComponentModel.DataAnnotations;

namespace RiskCalculator.API.Models.Requests;

public class SaveHistoryRequest
{
    [Required]
    public int ThreatLevel { get; set; }

    [Required]
    public int VulnerabilityLevel { get; set; }

    [Required]
    public string ThreatType { get; set; } = string.Empty;

    [Required]
    public string VulnerabilityCategory { get; set; } = string.Empty;

    [Required]
    public int RiskScore { get; set; }

    [Required]
    public string RiskLevel { get; set; } = string.Empty;

    public string? AssetValue { get; set; }

    public string? Description { get; set; }

    public string? UserIdentifier { get; set; }

    public Dictionary<string, object>? AdditionalMetadata { get; set; }
}
