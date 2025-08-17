namespace RiskCalculator.API.Models.DTOs;

public class RiskCalculationResponseDto
{
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string RiskColor { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
    public string CalculatedAt { get; set; } = string.Empty;
    public ThreatAssessmentDto ThreatAssessment { get; set; } = new();
    public VulnerabilityAssessmentDto VulnerabilityAssessment { get; set; } = new();
    public RiskMatrixPositionDto MatrixPosition { get; set; } = new();
    public string Severity { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
}

public class ThreatAssessmentDto
{
    public int Level { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Indicators { get; set; } = new();
    public string Likelihood { get; set; } = string.Empty;
    public List<string> MitigationStrategies { get; set; } = new();
}

public class VulnerabilityAssessmentDto
{
    public int Level { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Weaknesses { get; set; } = new();
    public string Exploitability { get; set; } = string.Empty;
    public List<string> RemediationSteps { get; set; } = new();
}

public class RiskMatrixPositionDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Zone { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class RiskMatrixResponseDto
{
    public List<List<string>> Matrix { get; set; } = new();
    public List<string> ThreatLabels { get; set; } = new();
    public List<string> VulnerabilityLabels { get; set; } = new();
    public RiskMatrixConfigDto Configuration { get; set; } = new();
}

public class RiskMatrixConfigDto
{
    public Dictionary<string, string> ColorMap { get; set; } = new();
    public List<RiskZoneDto> Zones { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
}

public class RiskZoneDto
{
    public string Level { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MinScore { get; set; }
    public int MaxScore { get; set; }
}
