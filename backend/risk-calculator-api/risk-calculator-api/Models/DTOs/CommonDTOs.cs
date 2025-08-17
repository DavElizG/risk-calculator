namespace RiskCalculator.API.Models.DTOs;

public class RiskHistoryDto
{
    public int Id { get; set; }
    public int ThreatLevel { get; set; }
    public int VulnerabilityLevel { get; set; }
    public string ThreatType { get; set; } = string.Empty;
    public string VulnerabilityCategory { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string? AssetValue { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserIdentifier { get; set; }
}

public class ThreatTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Examples { get; set; } = new();
    public string Icon { get; set; } = string.Empty;
}

public class VulnerabilityCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> CommonWeaknesses { get; set; } = new();
    public string Icon { get; set; } = string.Empty;
}

public class RecommendationDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Steps { get; set; } = new();
    public int EffectivenessScore { get; set; }
}

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PaginatedResponseDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
