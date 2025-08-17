import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type {
  CalculateRiskRequest,
  RiskCalculationResponse,
  SaveHistoryRequest,
  RiskHistoryItem,
  PaginatedResponse,
} from '../types/riskCalculator';
import { RiskCalculatorApiService } from '../services/riskCalculatorApi';

// Query Keys
export const queryKeys = {
  riskCalculation: ['riskCalculation'] as const,
  riskMatrix: ['riskMatrix'] as const,
  history: ['history'] as const,
  threatTypes: ['threatTypes'] as const,
  vulnerabilityCategories: ['vulnerabilityCategories'] as const,
  recommendations: (riskLevel: string, threatType?: string, vulnerabilityCategory?: string) =>
    ['recommendations', riskLevel, threatType, vulnerabilityCategory] as const,
  healthCheck: ['healthCheck'] as const,
};

// Risk Calculation Hook
export const useRiskCalculation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CalculateRiskRequest) => RiskCalculatorApiService.calculateRisk(request),
    onSuccess: (data: RiskCalculationResponse) => {
      // Cache the result for potential reuse
      queryClient.setQueryData(queryKeys.riskCalculation, data);
    },
    onError: (error: Error) => {
      console.error('Risk calculation failed:', error);
    },
  });
};

// Risk Matrix Hook
export const useRiskMatrix = () => {
  return useQuery({
    queryKey: queryKeys.riskMatrix,
    queryFn: () => RiskCalculatorApiService.getRiskMatrix(),
    staleTime: 1000 * 60 * 60, // 1 hour
    gcTime: 1000 * 60 * 60 * 24, // 24 hours
  });
};

// History Hooks
export const useSaveToHistory = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: SaveHistoryRequest) => RiskCalculatorApiService.saveToHistory(request),
    onSuccess: () => {
      // Invalidate history queries to refetch updated data
      queryClient.invalidateQueries({ queryKey: queryKeys.history });
    },
    onError: (error: Error) => {
      console.error('Failed to save to history:', error);
    },
  });
};

export const useRiskHistory = (pageNumber: number = 1, pageSize: number = 10, userIdentifier?: string) => {
  return useQuery({
    queryKey: [...queryKeys.history, pageNumber, pageSize, userIdentifier],
    queryFn: () => RiskCalculatorApiService.getHistory(pageNumber, pageSize, userIdentifier),
    placeholderData: (previousData: PaginatedResponse<RiskHistoryItem> | undefined) => previousData,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

// Configuration Hooks
export const useThreatTypes = () => {
  return useQuery({
    queryKey: queryKeys.threatTypes,
    queryFn: () => RiskCalculatorApiService.getThreatTypes(),
    staleTime: 1000 * 60 * 60, // 1 hour
    gcTime: 1000 * 60 * 60 * 24, // 24 hours
  });
};

export const useVulnerabilityCategories = () => {
  return useQuery({
    queryKey: queryKeys.vulnerabilityCategories,
    queryFn: () => RiskCalculatorApiService.getVulnerabilityCategories(),
    staleTime: 1000 * 60 * 60, // 1 hour
    gcTime: 1000 * 60 * 60 * 24, // 24 hours
  });
};

export const useRecommendations = (
  riskLevel: string,
  threatType?: string,
  vulnerabilityCategory?: string,
  enabled: boolean = true
) => {
  return useQuery({
    queryKey: queryKeys.recommendations(riskLevel, threatType, vulnerabilityCategory),
    queryFn: () => RiskCalculatorApiService.getRecommendations(riskLevel, threatType, vulnerabilityCategory),
    enabled: enabled && !!riskLevel,
    staleTime: 1000 * 60 * 30, // 30 minutes
  });
};

// Health Check Hook
export const useHealthCheck = () => {
  return useQuery({
    queryKey: queryKeys.healthCheck,
    queryFn: () => RiskCalculatorApiService.healthCheck(),
    refetchInterval: 1000 * 60 * 5, // Check every 5 minutes
    staleTime: 1000 * 60, // 1 minute
    retry: 3,
  });
};

// Combined hook for initial data loading
export const useInitialData = () => {
  const threatTypesQuery = useThreatTypes();
  const vulnerabilityCategoriesQuery = useVulnerabilityCategories();
  const riskMatrixQuery = useRiskMatrix();
  const healthCheckQuery = useHealthCheck();

  return {
    threatTypes: threatTypesQuery.data,
    vulnerabilityCategories: vulnerabilityCategoriesQuery.data,
    riskMatrix: riskMatrixQuery.data,
    healthStatus: healthCheckQuery.data,
    isLoading: threatTypesQuery.isLoading || 
               vulnerabilityCategoriesQuery.isLoading || 
               riskMatrixQuery.isLoading,
    isError: threatTypesQuery.isError || 
             vulnerabilityCategoriesQuery.isError || 
             riskMatrixQuery.isError ||
             healthCheckQuery.isError,
    errors: [
      threatTypesQuery.error,
      vulnerabilityCategoriesQuery.error,
      riskMatrixQuery.error,
      healthCheckQuery.error,
    ].filter(Boolean),
  };
};

// Custom hook for risk calculation with automatic history saving
export const useRiskCalculationWithHistory = () => {
  const calculateRisk = useRiskCalculation();
  const saveToHistory = useSaveToHistory();

  const calculateAndSave = async (request: CalculateRiskRequest, saveToHistoryEnabled: boolean = true) => {
    try {
      // Calculate risk first
      const result = await calculateRisk.mutateAsync(request);

      // Save to history if enabled
      if (saveToHistoryEnabled) {
        const historyRequest: SaveHistoryRequest = {
          threatLevel: request.threatLevel,
          vulnerabilityLevel: request.vulnerabilityLevel,
          threatType: request.threatType,
          vulnerabilityCategory: request.vulnerabilityCategory,
          riskScore: result.riskScore,
          riskLevel: result.riskLevel,
          assetValue: request.assetValue,
          description: request.description,
          userIdentifier: 'default-user', // In a real app, this would come from auth
          additionalMetadata: request.additionalMetadata,
        };

        await saveToHistory.mutateAsync(historyRequest);
      }

      return result;
    } catch (error) {
      console.error('Risk calculation with history failed:', error);
      throw error;
    }
  };

  return {
    calculateAndSave,
    isLoading: calculateRisk.isPending || saveToHistory.isPending,
    error: calculateRisk.error || saveToHistory.error,
    data: calculateRisk.data,
  };
};
