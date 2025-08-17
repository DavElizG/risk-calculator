import React from 'react';
import { useTheme } from '../contexts/ThemeContext';
import type { RiskCalculationResponse } from '../types/riskCalculator';

interface RiskResultsProps {
  result: RiskCalculationResponse;
  isLoading?: boolean;
}

const RiskResults: React.FC<RiskResultsProps> = ({ result, isLoading = false }) => {
  const { isDarkMode } = useTheme();

  const getRiskColor = (level: string) => {
    const colors = {
      'LOW': isDarkMode ? 'bg-green-700 border-green-500' : 'bg-green-50 border-green-200',
      'MEDIUM': isDarkMode ? 'bg-yellow-700 border-yellow-500' : 'bg-yellow-50 border-yellow-200',
      'HIGH': isDarkMode ? 'bg-orange-700 border-orange-500' : 'bg-orange-50 border-orange-200',
      'CRITICAL': isDarkMode ? 'bg-red-700 border-red-500' : 'bg-red-50 border-red-200'
    };
    return colors[level as keyof typeof colors] || colors.LOW;
  };

  const getRiskTextColor = (level: string) => {
    const colors = {
      'LOW': isDarkMode ? 'text-green-200' : 'text-green-800',
      'MEDIUM': isDarkMode ? 'text-yellow-200' : 'text-yellow-800',
      'HIGH': isDarkMode ? 'text-orange-200' : 'text-orange-800',
      'CRITICAL': isDarkMode ? 'text-red-200' : 'text-red-800'
    };
    return colors[level as keyof typeof colors] || colors.LOW;
  };

  const getRiskIcon = (level: string) => {
    const icons = {
      'LOW': '🟢',
      'MEDIUM': '🟡',
      'HIGH': '🟠',
      'CRITICAL': '🔴'
    };
    return icons[level as keyof typeof icons] || '🟢';
  };

  const getSeverityDescription = (severity: string) => {
    const descriptions = {
      'MINIMAL': 'Risk is within acceptable parameters',
      'LOW': 'Risk requires monitoring but is manageable',
      'MODERATE': 'Risk requires attention and mitigation planning',
      'HIGH': 'Risk requires immediate attention and action',
      'SEVERE': 'Risk requires urgent intervention and management',
      'CRITICAL': 'Risk poses significant threat requiring immediate action'
    };
    return descriptions[severity as keyof typeof descriptions] || 'Risk assessment completed';
  };

  if (isLoading) {
    return (
      <div className={`p-6 rounded-xl border ${isDarkMode ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'} shadow-lg`}>
        <div className="animate-pulse">
          <div className={`h-6 ${isDarkMode ? 'bg-gray-700' : 'bg-gray-200'} rounded mb-4`}></div>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {[1, 2, 3].map(i => (
              <div key={i} className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700' : 'bg-gray-100'}`}>
                <div className={`h-4 ${isDarkMode ? 'bg-gray-600' : 'bg-gray-300'} rounded mb-2`}></div>
                <div className={`h-8 ${isDarkMode ? 'bg-gray-600' : 'bg-gray-300'} rounded`}></div>
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className={`p-6 rounded-xl border ${getRiskColor(result.riskLevel)} shadow-lg transition-all duration-300`}>
      <div className="flex items-center justify-between mb-6">
        <h3 className={`text-2xl font-bold ${getRiskTextColor(result.riskLevel)}`}>
          {getRiskIcon(result.riskLevel)} Risk Assessment Results
        </h3>
        <div className={`px-3 py-1 rounded-full text-sm font-semibold ${getRiskTextColor(result.riskLevel)}`}>
          {result.riskLevel} RISK
        </div>
      </div>

      {/* Main Risk Metrics */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <div className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700/50' : 'bg-white/50'} border ${isDarkMode ? 'border-gray-600' : 'border-gray-200'}`}>
          <p className={`text-sm font-medium ${isDarkMode ? 'text-gray-300' : 'text-gray-600'}`}>Risk Score</p>
          <p className={`text-3xl font-bold ${getRiskTextColor(result.riskLevel)}`}>
            {result.riskScore}
          </p>
        </div>

        <div className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700/50' : 'bg-white/50'} border ${isDarkMode ? 'border-gray-600' : 'border-gray-200'}`}>
          <p className={`text-sm font-medium ${isDarkMode ? 'text-gray-300' : 'text-gray-600'}`}>Risk Level</p>
          <p className={`text-xl font-bold ${getRiskTextColor(result.riskLevel)}`}>
            {result.riskLevel}
          </p>
        </div>

        <div className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700/50' : 'bg-white/50'} border ${isDarkMode ? 'border-gray-600' : 'border-gray-200'}`}>
          <p className={`text-sm font-medium ${isDarkMode ? 'text-gray-300' : 'text-gray-600'}`}>Severity</p>
          <p className={`text-xl font-bold ${getRiskTextColor(result.riskLevel)}`}>
            {result.severity}
          </p>
        </div>

        <div className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700/50' : 'bg-white/50'} border ${isDarkMode ? 'border-gray-600' : 'border-gray-200'}`}>
          <p className={`text-sm font-medium ${isDarkMode ? 'text-gray-300' : 'text-gray-600'}`}>Confidence</p>
          <p className={`text-xl font-bold ${getRiskTextColor(result.riskLevel)}`}>
            {Math.round(result.confidenceScore * 100)}%
          </p>
        </div>
      </div>

      {/* Assessment Details */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        {/* Threat Assessment */}
        <div className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700/30' : 'bg-white/30'} border ${isDarkMode ? 'border-gray-600' : 'border-gray-200'}`}>
          <h4 className={`text-lg font-semibold mb-3 ${getRiskTextColor(result.riskLevel)}`}>
            🎯 Threat Assessment
          </h4>
          <div className="space-y-2">
            <p className={`${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
              <span className="font-medium">Level:</span> {result.threatAssessment.level}/10
            </p>
            <p className={`${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
              <span className="font-medium">Type:</span> {result.threatAssessment.type || 'General'}
            </p>
            <p className={`${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
              <span className="font-medium">Likelihood:</span> {result.threatAssessment.likelihood}
            </p>
          </div>
        </div>

        {/* Vulnerability Assessment */}
        <div className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700/30' : 'bg-white/30'} border ${isDarkMode ? 'border-gray-600' : 'border-gray-200'}`}>
          <h4 className={`text-lg font-semibold mb-3 ${getRiskTextColor(result.riskLevel)}`}>
            🛡️ Vulnerability Assessment
          </h4>
          <div className="space-y-2">
            <p className={`${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
              <span className="font-medium">Level:</span> {result.vulnerabilityAssessment.level}/10
            </p>
            <p className={`${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
              <span className="font-medium">Category:</span> {result.vulnerabilityAssessment.category || 'General'}
            </p>
            <p className={`${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
              <span className="font-medium">Exploitability:</span> {result.vulnerabilityAssessment.exploitability}
            </p>
          </div>
        </div>
      </div>

      {/* Matrix Position */}
      <div className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700/30' : 'bg-white/30'} border ${isDarkMode ? 'border-gray-600' : 'border-gray-200'} mb-6`}>
        <h4 className={`text-lg font-semibold mb-3 ${getRiskTextColor(result.riskLevel)}`}>
          📍 Matrix Position
        </h4>
        <p className={`${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
          <span className="font-medium">Position:</span> T{result.matrixPosition.y} × V{result.matrixPosition.x} 
          <span className="ml-2 text-sm">({result.matrixPosition.zone} Zone)</span>
        </p>
        <p className={`text-sm ${isDarkMode ? 'text-gray-400' : 'text-gray-600'} mt-1`}>
          {getSeverityDescription(result.severity)}
        </p>
      </div>

      {/* Recommendations */}
      {result.recommendations && result.recommendations.length > 0 && (
        <div className={`p-4 rounded-lg ${isDarkMode ? 'bg-gray-700/30' : 'bg-white/30'} border ${isDarkMode ? 'border-gray-600' : 'border-gray-200'}`}>
          <h4 className={`text-lg font-semibold mb-3 ${getRiskTextColor(result.riskLevel)}`}>
            💡 Recommendations
          </h4>
          <ul className="space-y-2">
            {result.recommendations.map((rec, index) => (
              <li key={index} className={`flex items-start ${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
                <span className="text-green-500 mr-2 mt-1">•</span>
                <span>{rec}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Timestamp */}
      <div className={`mt-4 text-sm ${isDarkMode ? 'text-gray-400' : 'text-gray-500'} text-center`}>
        Assessment completed on {new Date(result.calculatedAt).toLocaleString()}
      </div>
    </div>
  );
};

export default RiskResults;
