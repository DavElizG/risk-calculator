import React, { useState, useEffect } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ThemeProvider, useTheme } from './contexts/ThemeContext';
import { useRiskCalculation, useHealthCheck, useThreatTypes, useVulnerabilityCategories } from './hooks/useRiskCalculation';
import { useToast } from './hooks/useToast';
import RiskMatrix from './components/RiskMatrix';
import RiskResults from './components/RiskResults';
import ToastContainer from './components/ToastContainer';
import type { CalculateRiskRequest } from './types/riskCalculator';
import './index.css';

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
    },
  },
});

function RiskCalculatorApp() {
  const { isDarkMode, toggleDarkMode } = useTheme();
  const { toasts, showSuccess, showError, showWarning, removeToast } = useToast();
  const [formData, setFormData] = useState<CalculateRiskRequest>({
    threatLevel: 1,
    vulnerabilityLevel: 1,
    threatType: '',
    vulnerabilityCategory: ''
  });
  const [customThreatType, setCustomThreatType] = useState('');
  const [customVulnerabilityCategory, setCustomVulnerabilityCategory] = useState('');

  const riskCalculation = useRiskCalculation();
  const healthCheck = useHealthCheck();
  const threatTypes = useThreatTypes();
  const vulnerabilityCategories = useVulnerabilityCategories();

  // Handle success and error states
  useEffect(() => {
    if (riskCalculation.isSuccess && riskCalculation.data) {
      showSuccess(
        'Risk Calculation Complete!',
        `Risk Level: ${riskCalculation.data.riskLevel} (Score: ${riskCalculation.data.riskScore})`
      );
    }
  }, [riskCalculation.isSuccess, riskCalculation.data, showSuccess]);

  useEffect(() => {
    if (riskCalculation.isError) {
      const error = riskCalculation.error as any;
      showError(
        'Risk Calculation Failed',
        error?.message || 'An unexpected error occurred while calculating risk'
      );
    }
  }, [riskCalculation.isError, riskCalculation.error, showError]);

  useEffect(() => {
    if (threatTypes.isError) {
      showWarning(
        'Failed to Load Threat Types',
        'Using default options. Please refresh the page.'
      );
    }
  }, [threatTypes.isError, showWarning]);

  useEffect(() => {
    if (vulnerabilityCategories.isError) {
      showWarning(
        'Failed to Load Vulnerability Categories',
        'Using default options. Please refresh the page.'
      );
    }
  }, [vulnerabilityCategories.isError, showWarning]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const submitData = {
      ...formData,
      threatType: formData.threatType === 'Custom/Other' ? customThreatType : formData.threatType,
      vulnerabilityCategory: formData.vulnerabilityCategory === 'Custom/Other' ? customVulnerabilityCategory : formData.vulnerabilityCategory
    };
    riskCalculation.mutate(submitData);
  };

  const handleInputChange = (field: keyof CalculateRiskRequest, value: any) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleMatrixCellClick = (threat: number, vulnerability: number) => {
    setFormData(prev => ({ 
      ...prev, 
      threatLevel: threat, 
      vulnerabilityLevel: vulnerability 
    }));
  };

  return (
    <div className={`min-h-screen transition-all duration-300 ${
      isDarkMode 
        ? 'bg-gradient-to-br from-gray-900 via-gray-800 to-gray-900' 
        : 'bg-gradient-to-br from-blue-50 via-white to-indigo-50'
    }`}>
      {/* Header */}
      <header className={`backdrop-blur-md border-b transition-all duration-300 ${
        isDarkMode 
          ? 'bg-gray-900/95 border-gray-700 shadow-xl' 
          : 'bg-white/95 border-gray-200 shadow-lg'
      }`}>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <h1 className={`text-2xl font-bold transition-colors duration-300 ${
                  isDarkMode ? 'text-blue-400' : 'text-blue-600'
                }`}>
                  🛡️ Risk Calculator
                </h1>
              </div>
              <div className="hidden md:block">
                <div className="ml-10 flex items-baseline space-x-4">
                  <span className={`text-sm transition-colors duration-300 ${
                    isDarkMode ? 'text-gray-300' : 'text-gray-500'
                  }`}>
                    Professional Cybersecurity Tool
                  </span>
                </div>
              </div>
            </div>
            <div className="flex items-center space-x-4">
              {/* Health Status */}
              <div className="text-sm">
                {healthCheck.data ? (
                  <span className="flex items-center text-green-500">
                    <div className="w-2 h-2 bg-green-500 rounded-full mr-2 animate-pulse"></div>
                    Connected
                  </span>
                ) : (
                  <span className="flex items-center text-red-500">
                    <div className="w-2 h-2 bg-red-500 rounded-full mr-2"></div>
                    Offline
                  </span>
                )}
              </div>
              
              {/* Dark Mode Toggle */}
              <button
                onClick={toggleDarkMode}
                className={`p-2 rounded-lg transition-all duration-300 hover:scale-110 ${
                  isDarkMode 
                    ? 'bg-gray-700 text-yellow-400 hover:bg-gray-600' 
                    : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                }`}
                title={isDarkMode ? 'Switch to Light Mode' : 'Switch to Dark Mode'}
              >
                {isDarkMode ? '☀️' : '🌙'}
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0 space-y-8">
          
          {/* Risk Calculator Form */}
          <div className={`p-8 rounded-2xl shadow-2xl transition-all duration-300 backdrop-blur-sm ${
            isDarkMode 
              ? 'bg-gray-800/90 border border-gray-700' 
              : 'bg-white/90 border border-gray-200'
          }`}>
            <div className="text-center mb-8">
              <h2 className={`text-3xl font-bold mb-4 transition-colors duration-300 ${
                isDarkMode ? 'text-white' : 'text-gray-900'
              }`}>
                Cybersecurity Risk Assessment
              </h2>
              <p className={`text-lg transition-colors duration-300 ${
                isDarkMode ? 'text-gray-300' : 'text-gray-600'
              }`}>
                Calculate risk using the formula: <span className="font-bold text-blue-500">RISK = THREAT × VULNERABILITY</span>
              </p>
            </div>

            <form onSubmit={handleSubmit} className="space-y-8">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                {/* Threat Configuration */}
                <div className={`p-6 rounded-xl border transition-all duration-300 ${
                  isDarkMode 
                    ? 'bg-gray-700/50 border-gray-600' 
                    : 'bg-gray-50 border-gray-200'
                }`}>
                  <h3 className={`text-xl font-semibold mb-4 transition-colors duration-300 ${
                    isDarkMode ? 'text-white' : 'text-gray-900'
                  }`}>
                    🎯 Threat Configuration
                  </h3>
                  
                  <div className="space-y-4">
                    <div>
                      <label htmlFor="threatLevel" className={`block text-sm font-medium mb-2 transition-colors duration-300 ${
                        isDarkMode ? 'text-gray-300' : 'text-gray-700'
                      }`}>
                        Threat Level: {formData.threatLevel}/10
                      </label>
                      <input
                        type="range"
                        id="threatLevel"
                        min="1"
                        max="10"
                        value={formData.threatLevel}
                        onChange={(e) => handleInputChange('threatLevel', parseInt(e.target.value))}
                        className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer slider"
                      />
                      <div className="flex justify-between text-xs text-gray-500 mt-1">
                        <span>Low</span>
                        <span>Medium</span>
                        <span>High</span>
                      </div>
                    </div>

                    <div>
                      <label htmlFor="threatType" className={`block text-sm font-medium mb-2 transition-colors duration-300 ${
                        isDarkMode ? 'text-gray-300' : 'text-gray-700'
                      }`}>
                        Threat Type
                      </label>
                      <select
                        id="threatType"
                        value={formData.threatType}
                        onChange={(e) => handleInputChange('threatType', e.target.value)}
                        className={`w-full p-3 rounded-lg border transition-all duration-300 focus:ring-2 focus:ring-blue-500 ${
                          isDarkMode 
                            ? 'bg-gray-600 border-gray-500 text-white' 
                            : 'bg-white border-gray-300 text-gray-900'
                        }`}
                      >
                        <option value="">Select threat type...</option>
                        {threatTypes.data?.map((threat) => (
                          <option key={threat.name} value={threat.name}>
                            {threat.name}
                          </option>
                        ))}
                      </select>
                      {formData.threatType === 'Custom/Other' && (
                        <input
                          type="text"
                          placeholder="Enter custom threat type..."
                          value={customThreatType}
                          onChange={(e) => setCustomThreatType(e.target.value)}
                          className={`mt-2 w-full p-3 rounded-lg border transition-all duration-300 focus:ring-2 focus:ring-blue-500 ${
                            isDarkMode 
                              ? 'bg-gray-600 border-gray-500 text-white placeholder-gray-400' 
                              : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500'
                          }`}
                        />
                      )}
                    </div>
                  </div>
                </div>

                {/* Vulnerability Configuration */}
                <div className={`p-6 rounded-xl border transition-all duration-300 ${
                  isDarkMode 
                    ? 'bg-gray-700/50 border-gray-600' 
                    : 'bg-gray-50 border-gray-200'
                }`}>
                  <h3 className={`text-xl font-semibold mb-4 transition-colors duration-300 ${
                    isDarkMode ? 'text-white' : 'text-gray-900'
                  }`}>
                    🛡️ Vulnerability Configuration
                  </h3>
                  
                  <div className="space-y-4">
                    <div>
                      <label htmlFor="vulnerabilityLevel" className={`block text-sm font-medium mb-2 transition-colors duration-300 ${
                        isDarkMode ? 'text-gray-300' : 'text-gray-700'
                      }`}>
                        Vulnerability Level: {formData.vulnerabilityLevel}/10
                      </label>
                      <input
                        type="range"
                        id="vulnerabilityLevel"
                        min="1"
                        max="10"
                        value={formData.vulnerabilityLevel}
                        onChange={(e) => handleInputChange('vulnerabilityLevel', parseInt(e.target.value))}
                        className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer slider"
                      />
                      <div className="flex justify-between text-xs text-gray-500 mt-1">
                        <span>Low</span>
                        <span>Medium</span>
                        <span>High</span>
                      </div>
                    </div>

                    <div>
                      <label htmlFor="vulnerabilityCategory" className={`block text-sm font-medium mb-2 transition-colors duration-300 ${
                        isDarkMode ? 'text-gray-300' : 'text-gray-700'
                      }`}>
                        Vulnerability Category
                      </label>
                      <select
                        id="vulnerabilityCategory"
                        value={formData.vulnerabilityCategory}
                        onChange={(e) => handleInputChange('vulnerabilityCategory', e.target.value)}
                        className={`w-full p-3 rounded-lg border transition-all duration-300 focus:ring-2 focus:ring-blue-500 ${
                          isDarkMode 
                            ? 'bg-gray-600 border-gray-500 text-white' 
                            : 'bg-white border-gray-300 text-gray-900'
                        }`}
                      >
                        <option value="">Select vulnerability category...</option>
                        {vulnerabilityCategories.data?.map((vuln) => (
                          <option key={vuln.name} value={vuln.name}>
                            {vuln.name}
                          </option>
                        ))}
                      </select>
                      {formData.vulnerabilityCategory === 'Custom/Other' && (
                        <input
                          type="text"
                          placeholder="Enter custom vulnerability category..."
                          value={customVulnerabilityCategory}
                          onChange={(e) => setCustomVulnerabilityCategory(e.target.value)}
                          className={`mt-2 w-full p-3 rounded-lg border transition-all duration-300 focus:ring-2 focus:ring-blue-500 ${
                            isDarkMode 
                              ? 'bg-gray-600 border-gray-500 text-white placeholder-gray-400' 
                              : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500'
                          }`}
                        />
                      )}
                    </div>
                  </div>
                </div>
              </div>

              {/* Submit Button */}
              <div className="text-center">
                <button
                  type="submit"
                  disabled={riskCalculation.isPending}
                  className={`px-8 py-4 rounded-xl font-semibold text-lg transition-all duration-300 transform hover:scale-105 focus:outline-none focus:ring-4 disabled:opacity-50 disabled:cursor-not-allowed ${
                    riskCalculation.isPending
                      ? 'bg-gray-400 text-gray-200'
                      : 'bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white shadow-lg hover:shadow-xl focus:ring-blue-500'
                  }`}
                >
                  {riskCalculation.isPending ? (
                    <div className="flex items-center">
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white mr-2"></div>
                      Calculating...
                    </div>
                  ) : (
                    '🔍 Calculate Risk'
                  )}
                </button>
              </div>
            </form>
          </div>

          {/* Risk Matrix */}
          <RiskMatrix
            currentThreat={formData.threatLevel}
            currentVulnerability={formData.vulnerabilityLevel}
            onCellClick={handleMatrixCellClick}
          />

          {/* Results */}
          {riskCalculation.isSuccess && riskCalculation.data && (
            <div className="animate-fade-in">
              <RiskResults 
                result={riskCalculation.data} 
                isLoading={riskCalculation.isPending}
              />
            </div>
          )}
        </div>
      </main>

      {/* Toast Container */}
      <ToastContainer toasts={toasts} onRemoveToast={removeToast} />

      {/* Footer */}
      <footer className={`border-t transition-all duration-300 backdrop-blur-md ${
        isDarkMode 
          ? 'bg-gray-900/95 border-gray-700' 
          : 'bg-white/95 border-gray-200'
      }`}>
        <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <p className={`text-sm transition-colors duration-300 ${
              isDarkMode ? 'text-gray-400' : 'text-gray-500'
            }`}>
              © 2025 Professional Cybersecurity Risk Assessment Tool
            </p>
            <p className={`text-xs mt-2 transition-colors duration-300 ${
              isDarkMode ? 'text-gray-500' : 'text-gray-400'
            }`}>
              Calculate risk using the formula: RISK = THREAT × VULNERABILITY
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider>
        <RiskCalculatorApp />
      </ThemeProvider>
    </QueryClientProvider>
  );
}

export default App;
