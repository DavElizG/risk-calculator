/**
 * ESLint Configuration - Risk Calculator Frontend
 * 
 * Fecha: 28/10/2025
 * Versión de ESLint: 8.57.1
 * Tipo de configuración: Flat Config (ESLint v9 compatible)
 * 
 * Esta configuración implementa análisis estático de código para detectar:
 * - Violaciones de seguridad de tipos (TypeScript)
 * - Errores lógicos y código inaccesible
 * - Problemas de React Hooks
 * - Issues de Fast Refresh en desarrollo
 * 
 * Equipo: Marco Jimenez, Norman Alvarado, Jose Guadamuz, Melina Cruz
 */

import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'

export default tseslint.config(
  // Ignorar archivos compilados
  { ignores: ['dist'] },
  
  {
    // Extender configuraciones recomendadas
    extends: [
      js.configs.recommended,           // Reglas base de JavaScript
      ...tseslint.configs.recommended   // Reglas recomendadas de TypeScript
    ],
    
    // Aplicar a archivos TypeScript y TSX
    files: ['**/*.{ts,tsx}'],
    
    // Configuración del lenguaje
    languageOptions: {
      ecmaVersion: 2020,      // Soporte ES2020
      globals: globals.browser // Variables globales del navegador
    },
    
    // Plugins activos
    plugins: {
      'react-hooks': reactHooks,       // Reglas de React Hooks
      'react-refresh': reactRefresh    // Reglas de Vite Fast Refresh
    },
    
    // Reglas personalizadas
    rules: {
      // React Hooks: Validar dependencias exhaustivas
      ...reactHooks.configs.recommended.rules,
      
      // React Refresh: Advertir sobre exports mixtos
      'react-refresh/only-export-components': [
        'warn',
        { allowConstantExport: true }
      ],
      
      // NOTA: Las siguientes reglas están activas por defecto en tseslint.configs.recommended:
      // - @typescript-eslint/no-explicit-any: 'error'
      // - @typescript-eslint/no-unused-vars: 'error'
      // - no-dupe-else-if: 'error'
    },
  },
)

/**
 * REGLAS ACTIVAS Y SU PROPÓSITO DE SEGURIDAD:
 * 
 * 1. @typescript-eslint/no-explicit-any [ERROR]
 *    - Previene uso de tipo 'any' que elimina type safety
 *    - Reduce riesgo de inyección y validación débil
 *    - Enforce tipos estrictos en toda la aplicación
 * 
 * 2. no-dupe-else-if [ERROR]
 *    - Detecta condiciones duplicadas en if-else-if
 *    - Previene código inaccesible
 *    - Evita omisión accidental de validaciones
 * 
 * 3. react-hooks/exhaustive-deps [WARN]
 *    - Valida que useEffect incluya todas las dependencias
 *    - Previene stale closures y bugs de estado
 *    - Mejora predecibilidad del código
 * 
 * 4. react-refresh/only-export-components [WARN]
 *    - Asegura compatibilidad con Vite HMR
 *    - Mejora experiencia de desarrollo
 *    - Sin impacto de seguridad directo
 * 
 * COBERTURA DE OWASP:
 * - A03:2021 Injection: Parcial (via type safety)
 * - A04:2021 Insecure Design: Detecta patrones problemáticos
 * - A08:2021 Software and Data Integrity: Via linting estricto
 * 
 * INTEGRACIÓN CON SSDLC:
 * ✅ Static Analysis: Implementado
 * ⏳ Pre-commit Hooks: Pendiente (Husky)
 * ⏳ CI/CD Pipeline: Pendiente
 * ⏳ IDE Integration: Activo (VS Code ESLint Extension)
 */
