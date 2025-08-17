using FluentValidation;
using RiskCalculator.API.Models.Requests;

namespace RiskCalculator.API.Validators;

public class CalculateRiskRequestValidator : AbstractValidator<CalculateRiskRequest>
{
    public CalculateRiskRequestValidator()
    {
        RuleFor(x => x.ThreatLevel)
            .InclusiveBetween(1, 10)
            .WithMessage("Threat level must be between 1 and 10");

        RuleFor(x => x.VulnerabilityLevel)
            .InclusiveBetween(1, 10)
            .WithMessage("Vulnerability level must be between 1 and 10");

        RuleFor(x => x.ThreatType)
            .NotEmpty()
            .WithMessage("Threat type is required")
            .MaximumLength(100)
            .WithMessage("Threat type cannot exceed 100 characters");

        RuleFor(x => x.VulnerabilityCategory)
            .NotEmpty()
            .WithMessage("Vulnerability category is required")
            .MaximumLength(100)
            .WithMessage("Vulnerability category cannot exceed 100 characters");

        RuleFor(x => x.AssetValue)
            .MaximumLength(200)
            .WithMessage("Asset value cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.AssetValue));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class SaveHistoryRequestValidator : AbstractValidator<SaveHistoryRequest>
{
    public SaveHistoryRequestValidator()
    {
        RuleFor(x => x.ThreatLevel)
            .InclusiveBetween(1, 10)
            .WithMessage("Threat level must be between 1 and 10");

        RuleFor(x => x.VulnerabilityLevel)
            .InclusiveBetween(1, 10)
            .WithMessage("Vulnerability level must be between 1 and 10");

        RuleFor(x => x.RiskScore)
            .InclusiveBetween(1, 100)
            .WithMessage("Risk score must be between 1 and 100");

        RuleFor(x => x.ThreatType)
            .NotEmpty()
            .WithMessage("Threat type is required")
            .MaximumLength(100)
            .WithMessage("Threat type cannot exceed 100 characters");

        RuleFor(x => x.VulnerabilityCategory)
            .NotEmpty()
            .WithMessage("Vulnerability category is required")
            .MaximumLength(100)
            .WithMessage("Vulnerability category cannot exceed 100 characters");

        RuleFor(x => x.RiskLevel)
            .NotEmpty()
            .WithMessage("Risk level is required")
            .Must(BeAValidRiskLevel)
            .WithMessage("Risk level must be one of: VeryLow, Low, Medium, High, VeryHigh");

        RuleFor(x => x.AssetValue)
            .MaximumLength(200)
            .WithMessage("Asset value cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.AssetValue));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    private static bool BeAValidRiskLevel(string riskLevel)
    {
        var validLevels = new[] { "VeryLow", "Low", "Medium", "High", "VeryHigh" };
        return validLevels.Contains(riskLevel);
    }
}
