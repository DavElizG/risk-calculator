using RiskCalculator.API.Models.DTOs;
using RiskCalculator.API.Models.Requests;

namespace RiskCalculator.API.Services;

public class RiskCalculationService : IRiskCalculationService
{
    private readonly ILogger<RiskCalculationService> _logger;
    private static readonly List<RiskHistoryDto> _history = new();
    private static int _nextId = 1;

    public RiskCalculationService(ILogger<RiskCalculationService> logger)
    {
        _logger = logger;
    }

    public async Task<RiskCalculationResponseDto> CalculateRiskAsync(CalculateRiskRequest request)
    {
        _logger.LogInformation("Calculating risk for threat level {ThreatLevel} and vulnerability level {VulnerabilityLevel}", 
            request.ThreatLevel, request.VulnerabilityLevel);

        var riskScore = request.ThreatLevel * request.VulnerabilityLevel;
        var riskLevel = DetermineRiskLevel(riskScore);
        var riskColor = GetRiskColor(riskLevel);

        var response = new RiskCalculationResponseDto
        {
            RiskScore = riskScore,
            RiskLevel = riskLevel,
            RiskColor = riskColor,
            Recommendations = await GetRecommendationStringsAsync(riskLevel, request.ThreatType, request.VulnerabilityCategory),
            CalculatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ThreatAssessment = BuildThreatAssessment(request.ThreatLevel, request.ThreatType),
            VulnerabilityAssessment = BuildVulnerabilityAssessment(request.VulnerabilityLevel, request.VulnerabilityCategory),
            MatrixPosition = new RiskMatrixPositionDto
            {
                X = request.VulnerabilityLevel,
                Y = request.ThreatLevel,
                Description = $"{request.ThreatLevel},{request.VulnerabilityLevel}",
                Zone = DetermineRiskZone(riskLevel)
            },
            Severity = DetermineSeverity(riskScore),
            ConfidenceScore = CalculateConfidenceScore(request.ThreatLevel, request.VulnerabilityLevel)
        };

        _logger.LogInformation("Risk calculation completed. Score: {RiskScore}, Level: {RiskLevel}", 
            riskScore, riskLevel);

        return response;
    }

    public async Task<RiskMatrixResponseDto> GetRiskMatrixAsync()
    {
        _logger.LogInformation("Retrieving risk matrix");
        return await Task.FromResult(new RiskMatrixResponseDto());
    }

    public async Task<RiskHistoryDto> SaveToHistoryAsync(SaveHistoryRequest request)
    {
        _logger.LogInformation("Saving risk calculation to history");

        var historyItem = new RiskHistoryDto
        {
            Id = _nextId++,
            ThreatLevel = request.ThreatLevel,
            VulnerabilityLevel = request.VulnerabilityLevel,
            ThreatType = request.ThreatType,
            VulnerabilityCategory = request.VulnerabilityCategory,
            RiskScore = request.RiskScore,
            RiskLevel = request.RiskLevel,
            AssetValue = request.AssetValue,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UserIdentifier = request.UserIdentifier
        };

        _history.Add(historyItem);
        
        _logger.LogInformation("Risk calculation saved to history with ID {Id}", historyItem.Id);
        return await Task.FromResult(historyItem);
    }

    public async Task<PaginatedResponseDto<RiskHistoryDto>> GetHistoryAsync(int pageNumber = 1, int pageSize = 10, string? userIdentifier = null)
    {
        _logger.LogInformation("Retrieving risk calculation history. Page: {PageNumber}, Size: {PageSize}", 
            pageNumber, pageSize);

        var query = _history.AsQueryable();

        if (!string.IsNullOrEmpty(userIdentifier))
        {
            query = query.Where(h => h.UserIdentifier == userIdentifier);
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(h => h.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var response = new PaginatedResponseDto<RiskHistoryDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await Task.FromResult(response);
    }

    public async Task<List<ThreatTypeDto>> GetThreatTypesAsync()
    {
        _logger.LogInformation("Retrieving threat types");

        var threatTypes = new List<ThreatTypeDto>
        {
            new() { Name = "Malware", Description = "Malicious software including viruses, trojans, and ransomware", Category = "Technical", Examples = new() { "Ransomware", "Trojans", "Viruses", "Spyware" }, Icon = "🦠" },
            new() { Name = "Phishing", Description = "Fraudulent attempts to obtain sensitive information", Category = "Social", Examples = new() { "Email phishing", "Spear phishing", "Whaling", "Smishing" }, Icon = "🎣" },
            new() { Name = "Physical Theft", Description = "Unauthorized physical access to systems or data", Category = "Physical", Examples = new() { "Device theft", "Data theft", "Unauthorized access", "Tailgating" }, Icon = "🔓" },
            new() { Name = "Insider Threat", Description = "Threats from within the organization", Category = "Human", Examples = new() { "Malicious insider", "Negligent employee", "Compromised account", "Privilege abuse" }, Icon = "👤" },
            new() { Name = "Network Attack", Description = "Attacks targeting network infrastructure", Category = "Technical", Examples = new() { "DDoS", "Man-in-the-middle", "DNS poisoning", "Network intrusion" }, Icon = "🌐" },
            new() { Name = "Data Breach", Description = "Unauthorized access to confidential data", Category = "Technical", Examples = new() { "Database breach", "File exposure", "Cloud misconfiguration", "API vulnerability" }, Icon = "💾" },
            new() { Name = "Social Engineering", Description = "Manipulation of people to divulge confidential information", Category = "Social", Examples = new() { "Pretexting", "Baiting", "Quid pro quo", "Tailgating" }, Icon = "🎭" },
            new() { Name = "Advanced Persistent Threat", Description = "Prolonged and targeted cyberattack", Category = "Technical", Examples = new() { "State-sponsored attacks", "Industrial espionage", "Long-term infiltration", "Zero-day exploits" }, Icon = "🕵️" },
            new() { Name = "Ransomware", Description = "Malware that encrypts data and demands payment", Category = "Technical", Examples = new() { "CryptoLocker", "WannaCry", "Petya", "REvil" }, Icon = "🔒" },
            new() { Name = "Supply Chain Attack", Description = "Attacks targeting third-party vendors or suppliers", Category = "External", Examples = new() { "Software supply chain", "Hardware tampering", "Vendor compromise", "Third-party risk" }, Icon = "🔗" },
            new() { Name = "Cloud Security Threats", Description = "Threats specific to cloud environments", Category = "Technical", Examples = new() { "Misconfigured cloud storage", "Account hijacking", "Shared technology vulnerabilities", "Data loss" }, Icon = "☁️" },
            new() { Name = "IoT Threats", Description = "Threats targeting Internet of Things devices", Category = "Technical", Examples = new() { "Device hijacking", "Botnet recruitment", "Data interception", "Firmware attacks" }, Icon = "📱" },
            new() { Name = "Zero-Day Exploits", Description = "Attacks exploiting unknown vulnerabilities", Category = "Technical", Examples = new() { "Software vulnerabilities", "Hardware exploits", "Firmware attacks", "Protocol flaws" }, Icon = "⚡" },
            new() { Name = "Business Email Compromise", Description = "Email-based attacks targeting business operations", Category = "Social", Examples = new() { "CEO fraud", "Invoice fraud", "Payroll redirection", "Wire transfer fraud" }, Icon = "📧" },
            new() { Name = "Cryptojacking", Description = "Unauthorized use of systems to mine cryptocurrency", Category = "Technical", Examples = new() { "Browser-based mining", "Malware mining", "Cloud hijacking", "Mobile mining" }, Icon = "💰" },
            new() { Name = "API Attacks", Description = "Attacks targeting application programming interfaces", Category = "Technical", Examples = new() { "API abuse", "Injection attacks", "Broken authentication", "Rate limiting bypass" }, Icon = "🔌" },
            new() { Name = "Custom/Other", Description = "Custom threat type not listed above", Category = "Custom", Examples = new() { "Specify your own threat type" }, Icon = "✏️" }
        };

        return await Task.FromResult(threatTypes);
    }

    public async Task<List<VulnerabilityCategoryDto>> GetVulnerabilityCategoriesAsync()
    {
        _logger.LogInformation("Retrieving vulnerability categories");

        var categories = new List<VulnerabilityCategoryDto>
        {
            new() { Name = "Software Vulnerabilities", Description = "Weaknesses in software applications and systems", Type = "Technical", CommonWeaknesses = new() { "Buffer overflow", "SQL injection", "Cross-site scripting", "Unpatched software" }, Icon = "💻" },
            new() { Name = "Network Security", Description = "Weaknesses in network configuration and protection", Type = "Technical", CommonWeaknesses = new() { "Open ports", "Weak encryption", "Unsecured protocols", "Network segmentation" }, Icon = "🌐" },
            new() { Name = "Access Control", Description = "Weaknesses in user authentication and authorization", Type = "Technical", CommonWeaknesses = new() { "Weak passwords", "Default credentials", "Excessive privileges", "Poor identity management" }, Icon = "🔐" },
            new() { Name = "Data Protection", Description = "Weaknesses in data handling and storage", Type = "Technical", CommonWeaknesses = new() { "Unencrypted data", "Poor backup procedures", "Data leakage", "Inadequate disposal" }, Icon = "🛡️" },
            new() { Name = "Physical Security", Description = "Weaknesses in physical access controls", Type = "Physical", CommonWeaknesses = new() { "Unsecured facilities", "Poor visitor management", "Inadequate surveillance", "Device security" }, Icon = "🏢" },
            new() { Name = "Human Factors", Description = "Weaknesses related to human behavior and training", Type = "Human", CommonWeaknesses = new() { "Lack of awareness", "Poor training", "Social engineering susceptibility", "Policy non-compliance" }, Icon = "👥" },
            new() { Name = "Operational Procedures", Description = "Weaknesses in business processes and procedures", Type = "Operational", CommonWeaknesses = new() { "Inadequate incident response", "Poor change management", "Lack of documentation", "Insufficient monitoring" }, Icon = "⚙️" },
            new() { Name = "Compliance & Governance", Description = "Weaknesses in regulatory compliance and governance", Type = "Governance", CommonWeaknesses = new() { "Non-compliance", "Poor risk management", "Inadequate policies", "Lack of oversight" }, Icon = "📋" },
            new() { Name = "Cloud Security", Description = "Weaknesses in cloud infrastructure and services", Type = "Technical", CommonWeaknesses = new() { "Misconfigured cloud storage", "Weak IAM policies", "Insecure APIs", "Shared responsibility gaps" }, Icon = "☁️" },
            new() { Name = "Mobile Security", Description = "Weaknesses in mobile devices and applications", Type = "Technical", CommonWeaknesses = new() { "Insecure mobile apps", "Device management", "Mobile malware", "Data leakage" }, Icon = "📱" },
            new() { Name = "Web Application Security", Description = "Weaknesses in web applications and services", Type = "Technical", CommonWeaknesses = new() { "OWASP Top 10", "Input validation", "Session management", "Authentication flaws" }, Icon = "🌍" },
            new() { Name = "Database Security", Description = "Weaknesses in database systems and data storage", Type = "Technical", CommonWeaknesses = new() { "SQL injection", "Privilege escalation", "Data encryption", "Backup security" }, Icon = "🗄️" },
            new() { Name = "Email Security", Description = "Weaknesses in email systems and communications", Type = "Technical", CommonWeaknesses = new() { "Spam filtering", "Email encryption", "Phishing protection", "Email authentication" }, Icon = "📧" },
            new() { Name = "Endpoint Security", Description = "Weaknesses in end-user devices and systems", Type = "Technical", CommonWeaknesses = new() { "Antivirus gaps", "Patch management", "Device encryption", "USB security" }, Icon = "💻" },
            new() { Name = "IoT Security", Description = "Weaknesses in Internet of Things devices", Type = "Technical", CommonWeaknesses = new() { "Default passwords", "Firmware updates", "Network isolation", "Device monitoring" }, Icon = "📱" },
            new() { Name = "Third-Party Risk", Description = "Weaknesses from external vendors and partners", Type = "External", CommonWeaknesses = new() { "Vendor assessment", "Supply chain security", "Contract security", "Monitoring controls" }, Icon = "🤝" },
            new() { Name = "Incident Response", Description = "Weaknesses in incident detection and response", Type = "Operational", CommonWeaknesses = new() { "Detection capabilities", "Response procedures", "Communication plans", "Recovery processes" }, Icon = "🚨" },
            new() { Name = "Custom/Other", Description = "Custom vulnerability category not listed above", Type = "Custom", CommonWeaknesses = new() { "Specify your own vulnerability category" }, Icon = "✏️" }
        };

        return await Task.FromResult(categories);
    }

    public async Task<List<RecommendationDto>> GetRecommendationsAsync(string riskLevel, string? threatType = null, string? vulnerabilityCategory = null)
    {
        _logger.LogInformation("Retrieving recommendations for risk level {RiskLevel}", riskLevel);

        var recommendations = new List<RecommendationDto>();

        // Base recommendations by risk level
        switch (riskLevel)
        {
            case "VeryLow":
                recommendations.AddRange(GetVeryLowRiskRecommendations());
                break;
            case "Low":
                recommendations.AddRange(GetLowRiskRecommendations());
                break;
            case "Medium":
                recommendations.AddRange(GetMediumRiskRecommendations());
                break;
            case "High":
                recommendations.AddRange(GetHighRiskRecommendations());
                break;
            case "VeryHigh":
                recommendations.AddRange(GetVeryHighRiskRecommendations());
                break;
        }

        // Add specific recommendations based on threat type and vulnerability category
        if (!string.IsNullOrEmpty(threatType))
        {
            recommendations.AddRange(GetThreatSpecificRecommendations(threatType));
        }

        if (!string.IsNullOrEmpty(vulnerabilityCategory))
        {
            recommendations.AddRange(GetVulnerabilitySpecificRecommendations(vulnerabilityCategory));
        }

        return await Task.FromResult(recommendations.DistinctBy(r => r.Title).ToList());
    }

    private static string DetermineRiskLevel(int riskScore)
    {
        return riskScore switch
        {
            >= 1 and <= 20 => "VeryLow",     // 1-20 (1-2 x 1-10 or similar low combinations)
            >= 21 and <= 35 => "Low",        // 21-35 
            >= 36 and <= 55 => "Medium",     // 36-55
            >= 56 and <= 75 => "High",       // 56-75
            >= 76 and <= 100 => "VeryHigh",  // 76-100 (high threat x high vulnerability)
            _ => "Medium"
        };
    }

    private static string GetRiskColor(string riskLevel)
    {
        return riskLevel switch
        {
            "VeryLow" => "#22c55e",
            "Low" => "#eab308",
            "Medium" => "#f97316",
            "High" => "#ef4444",
            "VeryHigh" => "#dc2626",
            _ => "#f97316"
        };
    }

    private static string DetermineRiskZone(string riskLevel)
    {
        return riskLevel switch
        {
            "VeryLow" or "Low" => "Acceptable",
            "Medium" => "Manageable",
            "High" or "VeryHigh" => "Unacceptable",
            _ => "Manageable"
        };
    }

    private static string DetermineSeverity(int riskScore)
    {
        return riskScore switch
        {
            <= 20 => "Minimal",     // 1-20
            <= 35 => "Minor",       // 21-35  
            <= 55 => "Moderate",    // 36-55
            <= 75 => "Major",       // 56-75
            _ => "Critical"         // 76-100
        };
    }

    private static double CalculateConfidenceScore(int threatLevel, int vulnerabilityLevel)
    {
        // Higher levels generally have higher confidence in assessment
        var averageLevel = (threatLevel + vulnerabilityLevel) / 2.0;
        return Math.Round((averageLevel / 10.0) * 100, 1);  // Changed from 5.0 to 10.0
    }

    private ThreatAssessmentDto BuildThreatAssessment(int threatLevel, string threatType)
    {
        var descriptions = new Dictionary<int, string>
        {
            { 1, "Very low probability of threat occurrence with minimal impact potential" },
            { 2, "Low probability of threat occurrence with limited impact potential" },
            { 3, "Below moderate probability of threat occurrence with slight impact potential" },
            { 4, "Moderate probability of threat occurrence with noticeable impact potential" },
            { 5, "Above moderate probability of threat occurrence with moderate impact potential" },
            { 6, "Elevated probability of threat occurrence with significant impact potential" },
            { 7, "High probability of threat occurrence with substantial impact potential" },
            { 8, "Very high probability of threat occurrence with severe impact potential" },
            { 9, "Critical probability of threat occurrence with major impact potential" },
            { 10, "Extreme probability of threat occurrence with catastrophic impact potential" }
        };

        var likelihoods = new[] { 
            "Very Unlikely", "Unlikely", "Somewhat Unlikely", "Possible", "Somewhat Likely",
            "Likely", "Very Likely", "Highly Likely", "Almost Certain", "Certain"
        };

        return new ThreatAssessmentDto
        {
            Level = threatLevel,
            Type = threatType,
            Description = descriptions[threatLevel],
            Indicators = GetThreatIndicators(threatType),
            Likelihood = likelihoods[threatLevel - 1],
            MitigationStrategies = GetThreatMitigationStrategies(threatType)
        };
    }

    private VulnerabilityAssessmentDto BuildVulnerabilityAssessment(int vulnerabilityLevel, string vulnerabilityCategory)
    {
        var descriptions = new Dictionary<int, string>
        {
            { 1, "Very low vulnerability with strong protective measures in place" },
            { 2, "Low vulnerability with adequate protective measures" },
            { 3, "Below moderate vulnerability with decent protective measures" },
            { 4, "Moderate vulnerability with some protective measures" },
            { 5, "Above moderate vulnerability with limited protective measures" },
            { 6, "Elevated vulnerability with insufficient protective measures" },
            { 7, "High vulnerability with weak protective measures" },
            { 8, "Very high vulnerability with minimal protective measures" },
            { 9, "Critical vulnerability with very poor protective measures" },
            { 10, "Extreme vulnerability with no effective protective measures" }
        };

        var exploitabilities = new[] { 
            "Very Difficult", "Difficult", "Somewhat Difficult", "Moderate", "Somewhat Easy",
            "Easy", "Very Easy", "Extremely Easy", "Trivial", "No Protection"
        };

        return new VulnerabilityAssessmentDto
        {
            Level = vulnerabilityLevel,
            Category = vulnerabilityCategory,
            Description = descriptions[vulnerabilityLevel],
            Weaknesses = GetVulnerabilityWeaknesses(vulnerabilityCategory),
            Exploitability = exploitabilities[vulnerabilityLevel - 1],
            RemediationSteps = GetVulnerabilityRemediationSteps(vulnerabilityCategory)
        };
    }

    private async Task<List<string>> GetRecommendationStringsAsync(string riskLevel, string threatType, string vulnerabilityCategory)
    {
        var recommendations = await GetRecommendationsAsync(riskLevel, threatType, vulnerabilityCategory);
        return recommendations.Select(r => r.Title).ToList();
    }

    // Helper methods for specific recommendations and assessments
    private static List<string> GetThreatIndicators(string threatType)
    {
        return threatType.ToLower() switch
        {
            "malware" => new() { "Unusual network activity", "System performance degradation", "Suspicious file modifications", "Unexpected pop-ups" },
            "phishing" => new() { "Suspicious emails", "Unusual login attempts", "Unexpected password reset requests", "Social engineering attempts" },
            "physical theft" => new() { "Missing devices", "Unauthorized facility access", "Security camera alerts", "Badge access anomalies" },
            "insider threat" => new() { "Unusual data access patterns", "After-hours activity", "Large data downloads", "Policy violations" },
            "network attack" => new() { "Unusual network traffic", "Failed connection attempts", "DNS anomalies", "Bandwidth spikes" },
            _ => new() { "Anomalous behavior", "Security alerts", "System irregularities", "Access violations" }
        };
    }

    private static List<string> GetThreatMitigationStrategies(string threatType)
    {
        return threatType.ToLower() switch
        {
            "malware" => new() { "Deploy endpoint protection", "Regular system updates", "Email filtering", "User education" },
            "phishing" => new() { "Email security training", "Multi-factor authentication", "Email filtering", "Incident reporting procedures" },
            "physical theft" => new() { "Physical access controls", "Device encryption", "Asset tracking", "Security awareness" },
            "insider threat" => new() { "Access monitoring", "Background checks", "Principle of least privilege", "Regular reviews" },
            "network attack" => new() { "Network segmentation", "Intrusion detection", "Firewall configuration", "Traffic monitoring" },
            _ => new() { "Defense in depth", "Regular monitoring", "Incident response", "Security awareness" }
        };
    }

    private static List<string> GetVulnerabilityWeaknesses(string vulnerabilityCategory)
    {
        return vulnerabilityCategory.ToLower() switch
        {
            "software vulnerabilities" => new() { "Unpatched systems", "Legacy software", "Insecure coding practices", "Configuration errors" },
            "network security" => new() { "Open ports", "Weak protocols", "Poor segmentation", "Inadequate monitoring" },
            "access control" => new() { "Weak passwords", "Excessive privileges", "Poor identity management", "Shared accounts" },
            "data protection" => new() { "Unencrypted data", "Poor backup procedures", "Inadequate disposal", "Data leakage" },
            "physical security" => new() { "Unsecured facilities", "Poor visitor management", "Inadequate surveillance", "Device security" },
            _ => new() { "Configuration issues", "Policy gaps", "Training deficiencies", "Process weaknesses" }
        };
    }

    private static List<string> GetVulnerabilityRemediationSteps(string vulnerabilityCategory)
    {
        return vulnerabilityCategory.ToLower() switch
        {
            "software vulnerabilities" => new() { "Implement patch management", "Conduct security testing", "Update legacy systems", "Review configurations" },
            "network security" => new() { "Close unnecessary ports", "Implement network segmentation", "Deploy monitoring tools", "Update protocols" },
            "access control" => new() { "Enforce strong passwords", "Implement least privilege", "Deploy identity management", "Regular access reviews" },
            "data protection" => new() { "Implement encryption", "Improve backup procedures", "Secure data disposal", "Deploy DLP solutions" },
            "physical security" => new() { "Secure facilities", "Implement visitor management", "Deploy surveillance", "Secure devices" },
            _ => new() { "Review policies", "Provide training", "Improve processes", "Implement controls" }
        };
    }

    // Risk level specific recommendations
    private static List<RecommendationDto> GetVeryLowRiskRecommendations()
    {
        return new List<RecommendationDto>
        {
            new() { Title = "Maintain Current Security Posture", Description = "Continue existing security measures", Priority = "Low", Category = "Maintenance", Steps = new() { "Regular monitoring", "Periodic reviews" }, EffectivenessScore = 85 },
            new() { Title = "Schedule Regular Reviews", Description = "Implement periodic security assessments", Priority = "Low", Category = "Governance", Steps = new() { "Quarterly reviews", "Annual assessments" }, EffectivenessScore = 80 }
        };
    }

    private static List<RecommendationDto> GetLowRiskRecommendations()
    {
        return new List<RecommendationDto>
        {
            new() { Title = "Implement Basic Controls", Description = "Deploy fundamental security controls", Priority = "Medium", Category = "Technical", Steps = new() { "Install antivirus", "Enable firewalls", "Regular updates" }, EffectivenessScore = 75 },
            new() { Title = "Security Awareness Training", Description = "Provide basic security training to users", Priority = "Medium", Category = "Training", Steps = new() { "Conduct training sessions", "Distribute materials", "Test knowledge" }, EffectivenessScore = 70 }
        };
    }

    private static List<RecommendationDto> GetMediumRiskRecommendations()
    {
        return new List<RecommendationDto>
        {
            new() { Title = "Enhanced Security Controls", Description = "Implement additional security measures", Priority = "High", Category = "Technical", Steps = new() { "Deploy advanced threat protection", "Implement monitoring", "Enhance access controls" }, EffectivenessScore = 85 },
            new() { Title = "Incident Response Planning", Description = "Develop incident response procedures", Priority = "High", Category = "Governance", Steps = new() { "Create response plan", "Train response team", "Test procedures" }, EffectivenessScore = 80 }
        };
    }

    private static List<RecommendationDto> GetHighRiskRecommendations()
    {
        return new List<RecommendationDto>
        {
            new() { Title = "Immediate Risk Mitigation", Description = "Implement immediate protective measures", Priority = "Critical", Category = "Technical", Steps = new() { "Deploy emergency controls", "Increase monitoring", "Restrict access" }, EffectivenessScore = 90 },
            new() { Title = "Emergency Response Activation", Description = "Activate emergency response procedures", Priority = "Critical", Category = "Operational", Steps = new() { "Alert security team", "Implement containment", "Begin investigation" }, EffectivenessScore = 85 }
        };
    }

    private static List<RecommendationDto> GetVeryHighRiskRecommendations()
    {
        return new List<RecommendationDto>
        {
            new() { Title = "Critical Risk Response", Description = "Immediate action required to address critical risk", Priority = "Critical", Category = "Emergency", Steps = new() { "Isolate affected systems", "Activate crisis team", "Implement emergency procedures" }, EffectivenessScore = 95 },
            new() { Title = "Executive Notification", Description = "Notify executive leadership immediately", Priority = "Critical", Category = "Governance", Steps = new() { "Contact executives", "Prepare briefing", "Coordinate response" }, EffectivenessScore = 90 }
        };
    }

    private static List<RecommendationDto> GetThreatSpecificRecommendations(string threatType)
    {
        return threatType.ToLower() switch
        {
            "malware" => new()
            {
                new() { Title = "Deploy Advanced Endpoint Protection", Description = "Implement next-generation antivirus", Priority = "High", Category = "Technical", Steps = new() { "Select EPP solution", "Deploy to endpoints", "Configure policies" }, EffectivenessScore = 90 }
            },
            "phishing" => new()
            {
                new() { Title = "Email Security Enhancement", Description = "Strengthen email security controls", Priority = "High", Category = "Technical", Steps = new() { "Deploy email filtering", "Enable link protection", "Implement DMARC" }, EffectivenessScore = 85 }
            },
            _ => new List<RecommendationDto>()
        };
    }

    private static List<RecommendationDto> GetVulnerabilitySpecificRecommendations(string vulnerabilityCategory)
    {
        return vulnerabilityCategory.ToLower() switch
        {
            "software vulnerabilities" => new()
            {
                new() { Title = "Vulnerability Management Program", Description = "Implement systematic vulnerability management", Priority = "High", Category = "Technical", Steps = new() { "Deploy scanning tools", "Establish patching procedures", "Regular assessments" }, EffectivenessScore = 88 }
            },
            "access control" => new()
            {
                new() { Title = "Identity and Access Management", Description = "Strengthen access controls", Priority = "High", Category = "Technical", Steps = new() { "Implement IAM solution", "Enable MFA", "Regular access reviews" }, EffectivenessScore = 85 }
            },
            _ => new List<RecommendationDto>()
        };
    }
}
