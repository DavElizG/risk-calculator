// API Types
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors: string[];
  timestamp: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// Risk Calculation Types
export interface CalculateRiskRequest {
  threatLevel: number;
  vulnerabilityLevel: number;
  threatType: string;
  vulnerabilityCategory: string;
  assetValue?: string;
  description?: string;
  additionalMetadata?: Record<string, any>;
}

export interface RiskCalculationResponse {
  riskScore: number;
  riskLevel: string;
  riskColor: string;
  recommendations: string[];
  calculatedAt: string;
  threatAssessment: ThreatAssessment;
  vulnerabilityAssessment: VulnerabilityAssessment;
  matrixPosition: RiskMatrixPosition;
  severity: string;
  confidenceScore: number;
}

export interface ThreatAssessment {
  level: number;
  type: string;
  description: string;
  indicators: string[];
  likelihood: string;
  mitigationStrategies: string[];
}

export interface VulnerabilityAssessment {
  level: number;
  category: string;
  description: string;
  weaknesses: string[];
  exploitability: string;
  remediationSteps: string[];
}

export interface RiskMatrixPosition {
  x: number;
  y: number;
  cell: string;
  zone: string;
}

// Risk Matrix Types
export interface RiskMatrixResponse {
  matrix: RiskMatrixCell[][];
  riskLevels: RiskLevelDefinition[];
  configuration: MatrixConfiguration;
}

export interface RiskMatrixCell {
  threatLevel: number;
  vulnerabilityLevel: number;
  riskScore: number;
  riskLevel: string;
  color: string;
  position: Position;
}

export interface RiskLevelDefinition {
  level: string;
  minScore: number;
  maxScore: number;
  color: string;
  description: string;
}

export interface MatrixConfiguration {
  threatAxis: AxisConfiguration;
  vulnerabilityAxis: AxisConfiguration;
}

export interface AxisConfiguration {
  label: string;
  levels: string[];
}

export interface Position {
  x: number;
  y: number;
}

// History Types
export interface SaveHistoryRequest {
  threatLevel: number;
  vulnerabilityLevel: number;
  threatType: string;
  vulnerabilityCategory: string;
  riskScore: number;
  riskLevel: string;
  assetValue?: string;
  description?: string;
  userIdentifier?: string;
  additionalMetadata?: Record<string, any>;
}

export interface RiskHistoryItem {
  id: number;
  threatLevel: number;
  vulnerabilityLevel: number;
  threatType: string;
  vulnerabilityCategory: string;
  riskScore: number;
  riskLevel: string;
  assetValue?: string;
  description?: string;
  createdAt: string;
  userIdentifier?: string;
}

// Configuration Types
export interface ThreatType {
  name: string;
  description: string;
  category: string;
  examples: string[];
  icon: string;
}

export interface VulnerabilityCategory {
  name: string;
  description: string;
  type: string;
  commonWeaknesses: string[];
  icon: string;
}

export interface Recommendation {
  title: string;
  description: string;
  priority: string;
  category: string;
  steps: string[];
  effectivenessScore: number;
}

// UI Types
export type RiskLevel = 'VeryLow' | 'Low' | 'Medium' | 'High' | 'VeryHigh';

export interface FormData {
  threatLevel: number;
  vulnerabilityLevel: number;
  threatType: string;
  vulnerabilityCategory: string;
  assetValue: string;
  description: string;
}

export interface ExportOptions {
  format: 'pdf' | 'json' | 'csv';
  includeMatrix: boolean;
  includeRecommendations: boolean;
  includeHistory: boolean;
}

// Chart Types
export interface ChartData {
  labels: string[];
  datasets: ChartDataset[];
}

export interface ChartDataset {
  label: string;
  data: number[];
  backgroundColor: string | string[];
  borderColor: string | string[];
  borderWidth?: number;
}

// Component Props Types
export interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'danger' | 'success' | 'warning';
  size?: 'sm' | 'md' | 'lg';
  disabled?: boolean;
  loading?: boolean;
  children: React.ReactNode;
  onClick?: () => void;
  type?: 'button' | 'submit' | 'reset';
  className?: string;
}

export interface InputProps {
  label?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  placeholder?: string;
  value?: string | number;
  onChange?: (value: string | number) => void;
  type?: 'text' | 'number' | 'email' | 'password' | 'textarea';
  className?: string;
}

export interface SelectProps {
  label?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  placeholder?: string;
  value?: string | number;
  onChange?: (value: string | number) => void;
  options: SelectOption[];
  className?: string;
}

export interface SelectOption {
  value: string | number;
  label: string;
  disabled?: boolean;
}

export interface CardProps {
  title?: string;
  subtitle?: string;
  children: React.ReactNode;
  className?: string;
  actions?: React.ReactNode;
}

// Error Types
export interface AppError {
  message: string;
  code?: string;
  details?: string;
  timestamp: string;
}

// State Types
export interface AppState {
  currentCalculation?: RiskCalculationResponse;
  history: RiskHistoryItem[];
  loading: boolean;
  error?: AppError;
  threatTypes: ThreatType[];
  vulnerabilityCategories: VulnerabilityCategory[];
  riskMatrix?: RiskMatrixResponse;
}

// Hook Types
export interface UseRiskCalculationResult {
  calculate: (request: CalculateRiskRequest) => Promise<RiskCalculationResponse>;
  isLoading: boolean;
  error?: Error;
  data?: RiskCalculationResponse;
}

export interface UseRiskHistoryResult {
  saveToHistory: (request: SaveHistoryRequest) => Promise<RiskHistoryItem>;
  getHistory: (page?: number, pageSize?: number) => Promise<PaginatedResponse<RiskHistoryItem>>;
  isLoading: boolean;
  error?: Error;
  data?: PaginatedResponse<RiskHistoryItem>;
}

export interface UseRiskMatrixResult {
  getMatrix: () => Promise<RiskMatrixResponse>;
  isLoading: boolean;
  error?: Error;
  data?: RiskMatrixResponse;
}
