import React from 'react';
import { useTheme } from '../contexts/ThemeContext';

interface RiskMatrixProps {
  currentThreat: number;
  currentVulnerability: number;
  onCellClick?: (threat: number, vulnerability: number) => void;
}

const RiskMatrix: React.FC<RiskMatrixProps> = ({ 
  currentThreat, 
  currentVulnerability, 
  onCellClick 
}) => {
  const { isDarkMode } = useTheme();

  const getRiskLevel = (threat: number, vulnerability: number) => {
    const score = threat * vulnerability;
    if (score <= 10) return 'LOW';
    if (score <= 30) return 'MEDIUM';
    if (score <= 60) return 'HIGH';
    return 'CRITICAL';
  };

  const getRiskColor = (threat: number, vulnerability: number) => {
    const level = getRiskLevel(threat, vulnerability);
    const baseColors = {
      'LOW': 'bg-green-400',
      'MEDIUM': 'bg-yellow-400',
      'HIGH': 'bg-orange-400',
      'CRITICAL': 'bg-red-500'
    };
    
    const darkColors = {
      'LOW': 'bg-green-500',
      'MEDIUM': 'bg-yellow-500',
      'HIGH': 'bg-orange-500',
      'CRITICAL': 'bg-red-600'
    };

    return isDarkMode ? darkColors[level] : baseColors[level];
  };

  const getTextColor = (threat: number, vulnerability: number) => {
    const level = getRiskLevel(threat, vulnerability);
    return level === 'LOW' || level === 'MEDIUM' ? 'text-gray-900' : 'text-white';
  };

  const isCurrentCell = (threat: number, vulnerability: number) => {
    return currentThreat === threat && currentVulnerability === vulnerability;
  };

  return (
    <div className={`p-6 rounded-xl ${isDarkMode ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'} border shadow-lg`}>
      <h3 className={`text-xl font-bold mb-4 ${isDarkMode ? 'text-white' : 'text-gray-900'}`}>
        Risk Assessment Matrix
      </h3>
      
      <div className="grid grid-cols-11 gap-1 max-w-4xl">
        {/* Empty corner cell */}
        <div className={`p-2 text-center font-semibold ${isDarkMode ? 'text-gray-300' : 'text-gray-600'}`}>
        </div>
        
        {/* Vulnerability headers */}
        {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map(vuln => (
          <div 
            key={`vuln-${vuln}`}
            className={`p-2 text-center text-sm font-semibold ${isDarkMode ? 'text-gray-300 bg-gray-700' : 'text-gray-600 bg-gray-100'} rounded`}
          >
            V{vuln}
          </div>
        ))}
        
        {/* Threat rows */}
        {[10, 9, 8, 7, 6, 5, 4, 3, 2, 1].map(threat => (
          <React.Fragment key={`threat-row-${threat}`}>
            {/* Threat header */}
            <div className={`p-2 text-center text-sm font-semibold ${isDarkMode ? 'text-gray-300 bg-gray-700' : 'text-gray-600 bg-gray-100'} rounded`}>
              T{threat}
            </div>
            
            {/* Risk cells */}
            {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map(vuln => (
              <div
                key={`cell-${threat}-${vuln}`}
                onClick={() => onCellClick?.(threat, vuln)}
                className={`
                  p-2 text-center text-xs font-bold rounded cursor-pointer
                  transition-all duration-200 hover:scale-105 hover:shadow-md
                  ${getRiskColor(threat, vuln)}
                  ${getTextColor(threat, vuln)}
                  ${isCurrentCell(threat, vuln) ? 'ring-4 ring-blue-500 ring-opacity-50 scale-110' : ''}
                  ${onCellClick ? 'hover:ring-2 hover:ring-blue-400' : ''}
                `}
              >
                {threat * vuln}
              </div>
            ))}
          </React.Fragment>
        ))}
      </div>
      
      {/* Legend */}
      <div className="mt-6 grid grid-cols-2 md:grid-cols-4 gap-2">
        {[
          { level: 'LOW', range: '1-10', color: 'bg-green-400' },
          { level: 'MEDIUM', range: '11-30', color: 'bg-yellow-400' },
          { level: 'HIGH', range: '31-60', color: 'bg-orange-400' },
          { level: 'CRITICAL', range: '61-100', color: 'bg-red-500' }
        ].map(({ level, range, color }) => (
          <div key={level} className="flex items-center space-x-2">
            <div className={`w-4 h-4 rounded ${color}`}></div>
            <span className={`text-sm ${isDarkMode ? 'text-gray-300' : 'text-gray-700'}`}>
              {level} ({range})
            </span>
          </div>
        ))}
      </div>
      
      <div className={`mt-4 text-sm ${isDarkMode ? 'text-gray-400' : 'text-gray-500'}`}>
        <p>Vulnerability Level (V) → Horizontal Axis</p>
        <p>Threat Level (T) → Vertical Axis</p>
        <p>Risk Score = Threat × Vulnerability</p>
      </div>
    </div>
  );
};

export default RiskMatrix;
