import axios from 'axios';
import type { AxiosResponse, AxiosError } from 'axios';
import type {
  ApiResponse,
  PaginatedResponse,
  CalculateRiskRequest,
  RiskCalculationResponse,
  SaveHistoryRequest,
  RiskHistoryItem,
  RiskMatrixResponse,
  ThreatType,
  VulnerabilityCategory,
  Recommendation,
} from '../types/riskCalculator';

// Create axios instance with base configuration
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor for logging and auth
api.interceptors.request.use(
  (config) => {
    console.log(`Making ${config.method?.toUpperCase()} request to ${config.url}`);
    return config;
  },
  (error) => {
    console.error('Request error:', error);
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
api.interceptors.response.use(
  (response: AxiosResponse<ApiResponse<any>>) => {
    if (!response.data.success && response.data.errors?.length > 0) {
      throw new Error(response.data.errors.join(', '));
    }
    return response;
  },
  (error: AxiosError<any>) => {
    let message = 'An unexpected error occurred';
    let details: string[] = [];
    
    // Handle validation errors (400 Bad Request)
    if (error.response?.status === 400) {
      const errorData = error.response.data;
      
      // Handle ASP.NET Core ValidationProblemDetails
      if (errorData?.errors) {
        const validationErrors: string[] = [];
        Object.entries(errorData.errors).forEach(([, messages]) => {
          if (Array.isArray(messages)) {
            validationErrors.push(...messages);
          } else {
            validationErrors.push(String(messages));
          }
        });
        if (validationErrors.length > 0) {
          message = 'Validation Error';
          details = validationErrors;
        }
      }
      // Handle custom API response format
      else if (errorData?.errors && Array.isArray(errorData.errors)) {
        message = 'Validation Error';
        details = errorData.errors;
      }
      // Handle simple title/detail format
      else if (errorData?.title || errorData?.detail) {
        message = errorData.title || 'Validation Error';
        if (errorData.detail) details.push(errorData.detail);
      }
    }
    // Handle other API response formats
    else if (error.response?.data?.errors && Array.isArray(error.response.data.errors)) {
      message = error.response.data.message || 'API Error';
      details = error.response.data.errors;
    } else if (error.response?.data?.message) {
      message = error.response.data.message;
    } else if (error.message) {
      message = error.message;
    }

    console.error('API Error:', {
      status: error.response?.status,
      message,
      details,
      url: error.config?.url,
      requestData: error.config?.data,
      responseData: error.response?.data
    });

    // Create a detailed error message
    const fullMessage = details.length > 0 ? `${message}: ${details.join(', ')}` : message;
    throw new Error(fullMessage);
  }
);

export class RiskCalculatorApiService {
  // Risk Calculation Endpoints
  static async calculateRisk(request: CalculateRiskRequest): Promise<RiskCalculationResponse> {
    try {
      const response = await api.post<ApiResponse<RiskCalculationResponse>>('/RiskCalculator/calculate', request);
      return response.data.data!;
    } catch (error) {
      console.error('Error calculating risk:', error);
      throw error;
    }
  }

  static async getRiskMatrix(): Promise<RiskMatrixResponse> {
    try {
      const response = await api.get<ApiResponse<RiskMatrixResponse>>('/RiskCalculator/matrix');
      return response.data.data!;
    } catch (error) {
      console.error('Error fetching risk matrix:', error);
      throw error;
    }
  }

  // History Endpoints
  static async saveToHistory(request: SaveHistoryRequest): Promise<RiskHistoryItem> {
    try {
      const response = await api.post<ApiResponse<RiskHistoryItem>>('/RiskCalculator/history', request);
      return response.data.data!;
    } catch (error) {
      console.error('Error saving to history:', error);
      throw error;
    }
  }

  static async getHistory(
    pageNumber: number = 1,
    pageSize: number = 10,
    userIdentifier?: string
  ): Promise<PaginatedResponse<RiskHistoryItem>> {
    try {
      const params = new URLSearchParams({
        pageNumber: pageNumber.toString(),
        pageSize: pageSize.toString(),
      });

      if (userIdentifier) {
        params.append('userIdentifier', userIdentifier);
      }

      const response = await api.get<ApiResponse<PaginatedResponse<RiskHistoryItem>>>(
        `/RiskCalculator/history?${params}`
      );
      return response.data.data!;
    } catch (error) {
      console.error('Error fetching history:', error);
      throw error;
    }
  }

  // Configuration Endpoints
  static async getThreatTypes(): Promise<ThreatType[]> {
    try {
      const response = await api.get<ApiResponse<ThreatType[]>>('/RiskCalculator/threat-types');
      return response.data.data!;
    } catch (error) {
      console.error('Error fetching threat types:', error);
      throw error;
    }
  }

  static async getVulnerabilityCategories(): Promise<VulnerabilityCategory[]> {
    try {
      const response = await api.get<ApiResponse<VulnerabilityCategory[]>>('/RiskCalculator/vulnerability-categories');
      return response.data.data!;
    } catch (error) {
      console.error('Error fetching vulnerability categories:', error);
      throw error;
    }
  }

  static async getRecommendations(
    riskLevel: string,
    threatType?: string,
    vulnerabilityCategory?: string
  ): Promise<Recommendation[]> {
    try {
      const params = new URLSearchParams({ riskLevel });
      
      if (threatType) {
        params.append('threatType', threatType);
      }
      
      if (vulnerabilityCategory) {
        params.append('vulnerabilityCategory', vulnerabilityCategory);
      }

      const response = await api.get<ApiResponse<Recommendation[]>>(
        `/RiskCalculator/recommendations?${params}`
      );
      return response.data.data!;
    } catch (error) {
      console.error('Error fetching recommendations:', error);
      throw error;
    }
  }

  // Health Check
  static async healthCheck(): Promise<{ status: string; timestamp: string; version: string }> {
    try {
      const response = await api.get<ApiResponse<{ status: string; timestamp: string; version: string }>>(
        '/RiskCalculator/health'
      );
      return response.data.data!;
    } catch (error) {
      console.error('Error checking API health:', error);
      throw error;
    }
  }
}

// Export default instance for convenience
export default RiskCalculatorApiService;
