# Security Guidelines & Development Standards
## Risk Calculator - Frontend Development

**Version:** 1.0.0  
**Last Updated:** October 28, 2025  
**Project Stack:** React + TypeScript + Vite  
**SSDLC Phase:** Part 1 - Security by Design

---

## 📋 Tabla de Contenidos

1. [Introducción](#introducción)
2. [Estándar de Desarrollo Seguro (SSDLC Principles)](#1-estándar-de-desarrollo-seguro-ssdlc-principles)
3. [Convenciones de Código y Nomenclatura](#2-convenciones-de-código-y-nomenclatura)
4. [Reglas de Commits y Ramas](#3-reglas-de-commits-y-ramas)
5. [Buenas Prácticas de Seguridad en Frontend](#4-buenas-prácticas-de-seguridad-en-frontend)
6. [Herramientas de Linting y Escaneo Local](#5-herramientas-de-linting-y-escaneo-local)
7. [Uso Responsable de Agentes de IA](#6-uso-responsable-de-agentes-de-ia)
8. [Validación y Pruebas (BDD y Unit Tests)](#7-validación-y-pruebas-bdd-y-unit-tests)
9. [Revisión de Código y Control de Calidad](#8-revisión-de-código-y-control-de-calidad)
10. [Firmas y Aprobaciones](#firmas-y-aprobaciones-del-equipo-de-desarrollo)

---

## Introducción

Este documento establece el **estándar de desarrollo seguro** para el proyecto Risk Calculator, alineado con los principios del **Secure Software Development Lifecycle (SSDLC)**. Su objetivo es garantizar que todo el código desarrollado cumpla con criterios de:

- **Seguridad desde el diseño** (Security by Design)
- **Calidad y mantenibilidad** del código
- **Consistencia** en la estructura y nomenclatura
- **Trazabilidad** en commits y versionado
- **Validación continua** mediante pruebas automatizadas
- **Responsabilidad** en el uso de herramientas de IA generativa

Todos los miembros del equipo deben conocer, comprender y aplicar estas directrices en su trabajo diario.

---

## 1. Estándar de Desarrollo Seguro (SSDLC Principles)

### 1.1 Principios Fundamentales

El desarrollo del frontend Risk Calculator sigue los siguientes principios de seguridad:

#### **Security by Design**
- La seguridad debe considerarse desde la fase de diseño, no como un añadido posterior
- Validar toda entrada de usuario antes de procesarla o enviarla al backend
- Implementar manejo de errores que no exponga información sensible al usuario final

#### **Least Privilege Principle**
- Los componentes deben tener acceso únicamente a los datos y funciones que necesitan
- Evitar exponer APIs o funciones internas innecesariamente
- Limitar el alcance de variables y funciones mediante encapsulación adecuada

#### **Defense in Depth**
- Implementar validaciones tanto en frontend como en backend
- Sanitizar datos antes de renderizarlos (protección contra XSS)
- Usar configuraciones de seguridad en encabezados HTTP (CSP, HSTS, etc.)

#### **Fail Securely**
- En caso de error, la aplicación debe fallar de manera segura
- No mostrar stack traces, mensajes técnicos o información de debugging en producción
- Registrar errores internamente sin exponer detalles al usuario

### 1.2 Ciclo de Vida Seguro

Cada feature o cambio debe pasar por las siguientes etapas:

1. **Diseño:** Identificar requisitos de seguridad y posibles amenazas
2. **Desarrollo:** Aplicar convenciones y buenas prácticas documentadas aquí
3. **Escaneo:** Ejecutar análisis estático (ESLint, Snyk, etc.)
4. **Pruebas:** Validar mediante tests unitarios y de integración
5. **Revisión:** Code review obligatorio antes de merge
6. **Despliegue:** Validar en entorno staging antes de producción
7. **Monitoreo:** Revisar logs y métricas post-deployment

---

## 2. Convenciones de Código y Nomenclatura

### 2.1 Estructura de Carpetas

El proyecto sigue una arquitectura modular y clara:

```
src/
├── assets/          # Recursos estáticos (imágenes, iconos)
├── components/      # Componentes reutilizables de UI
├── contexts/        # Context API para estado global
├── hooks/           # Custom hooks reutilizables
├── services/        # Lógica de comunicación con APIs
├── types/           # Definiciones de tipos TypeScript
├── utils/           # Funciones utilitarias puras (si aplica)
└── App.tsx          # Componente raíz
```

**Reglas:**
- Cada carpeta debe contener **un propósito claro y único**
- No mezclar lógica de negocio en componentes de UI
- Los servicios deben ser independientes de los componentes

### 2.2 Nomenclatura de Archivos

#### **Componentes React**
- Usar **PascalCase**: `RiskMatrix.tsx`, `ToastContainer.tsx`
- Un componente por archivo
- Archivo y componente deben tener el mismo nombre

#### **Hooks Personalizados**
- Prefijo `use` + **camelCase**: `useRiskCalculation.ts`, `useToast.ts`
- Deben estar en la carpeta `hooks/`

#### **Servicios y APIs**
- Sufijo descriptivo + **camelCase**: `riskCalculatorApi.ts`
- Deben estar en la carpeta `services/`

#### **Tipos e Interfaces**
- Usar **PascalCase** para tipos e interfaces: `RiskCalculator.ts`
- Agrupar tipos relacionados en el mismo archivo
- Preferir `interface` sobre `type` para objetos

#### **Archivos de Contexto**
- Sufijo `Context` + **PascalCase**: `ThemeContext.tsx`
- Exportar tanto el Context como el Provider

### 2.3 Nomenclatura en Código

#### **Variables y Constantes**
```typescript
// ✅ Correcto
const userName = "John Doe";
const MAX_RETRIES = 3;
const isAuthenticated = true;

// ❌ Incorrecto
const UserName = "John Doe";        // No usar PascalCase para variables
const max_retries = 3;              // No usar snake_case
const authenticated = true;         // No omitir prefijo is/has para booleans
```

#### **Funciones**
```typescript
// ✅ Correcto
function calculateRiskScore(impact: number, probability: number): number { }
const handleSubmit = () => { };

// ❌ Incorrecto
function CalculateRiskScore() { }   // No usar PascalCase
function calc_risk() { }            // No usar snake_case
```

#### **Interfaces y Tipos**
```typescript
// ✅ Correcto
interface RiskCalculationRequest {
  impact: number;
  probability: number;
}

type RiskLevel = "low" | "medium" | "high" | "critical";

// ❌ Incorrecto
interface riskRequest { }           // No usar camelCase
type risk_level = string;           // No usar snake_case
```

#### **Componentes React**
```typescript
// ✅ Correcto
export const RiskMatrix: React.FC<RiskMatrixProps> = ({ data }) => { };

// ❌ Incorrecto
export const riskMatrix = () => { }; // No usar camelCase
export function Risk_Matrix() { }    // No usar snake_case
```

### 2.4 Organización del Código

#### **Orden de Importaciones**
```typescript
// 1. Dependencias externas
import React, { useState, useEffect } from 'react';
import axios from 'axios';

// 2. Importaciones internas (absolutos)
import { RiskCalculationRequest } from '@/types/riskCalculator';
import { useToast } from '@/hooks/useToast';

// 3. Importaciones relativas
import { validateRiskInput } from './validators';

// 4. Estilos
import './RiskMatrix.css';
```

#### **Orden de Elementos en Componentes**
```typescript
export const MyComponent: React.FC<Props> = ({ prop1, prop2 }) => {
  // 1. Hooks de estado
  const [state, setState] = useState();
  
  // 2. Hooks de contexto
  const { theme } = useTheme();
  
  // 3. Custom hooks
  const { showToast } = useToast();
  
  // 4. Efectos
  useEffect(() => { }, []);
  
  // 5. Funciones helper
  const handleClick = () => { };
  
  // 6. Renderizado
  return (<div>...</div>);
};
```

---

## 3. Reglas de Commits y Ramas

### 3.1 Conventional Commits

Todos los commits deben seguir el formato **Conventional Commits**:

```
<type>(<scope>): <subject>

<body (opcional)>

<footer (opcional)>
```

#### **Tipos Permitidos**

| Tipo       | Descripción                                          | Ejemplo                                    |
|------------|------------------------------------------------------|--------------------------------------------|
| `feat`     | Nueva funcionalidad                                  | `feat(risk): add risk matrix visualization`|
| `fix`      | Corrección de bug                                    | `fix(api): handle null response error`     |
| `security` | Corrección de vulnerabilidad o mejora de seguridad   | `security(auth): sanitize user input`      |
| `refactor` | Refactorización sin cambio de funcionalidad          | `refactor(hooks): extract validation logic`|
| `style`    | Cambios de formato (no afectan lógica)               | `style(ui): adjust button spacing`         |
| `test`     | Añadir o modificar tests                             | `test(risk): add unit tests for service`   |
| `docs`     | Documentación                                        | `docs(readme): update installation guide`  |
| `chore`    | Tareas de mantenimiento (deps, config, etc.)         | `chore(deps): update vite to v5.0`         |
| `perf`     | Mejoras de rendimiento                               | `perf(render): memoize expensive component`|
| `ci`       | Cambios en CI/CD                                     | `ci(github): add security scan workflow`   |

#### **Reglas de Subject**
- Máximo **50 caracteres**
- Imperativo, en minúsculas: "add feature" no "added feature"
- Sin punto final
- Descriptivo y claro

#### **Ejemplo Completo**
```
security(validation): sanitize user input before API call

- Add DOMPurify to sanitize HTML content
- Validate numeric ranges for impact and probability
- Prevent injection attacks on description field

Fixes #42
```

### 3.2 Estrategia de Ramas (Git Flow Adaptado)

#### **Ramas Principales**

- **`main`**: Código en producción, siempre estable
- **`develop`**: Rama de integración continua

#### **Ramas de Soporte**

| Tipo            | Prefijo       | Ejemplo                         | Base      | Merge a       |
|-----------------|---------------|---------------------------------|-----------|---------------|
| Feature         | `feature/`    | `feature/risk-matrix-ui`        | `develop` | `develop`     |
| Bugfix          | `bugfix/`     | `bugfix/toast-notification`     | `develop` | `develop`     |
| Hotfix          | `hotfix/`     | `hotfix/critical-xss-patch`     | `main`    | `main` + `develop` |
| Security Fix    | `security/`   | `security/sanitize-inputs`      | `develop` | `develop`     |
| Release         | `release/`    | `release/v1.2.0`                | `develop` | `main` + `develop` |

#### **Reglas de Ramas**
- **Nunca** hacer commit directo a `main` o `develop`
- Crear Pull Request para todo merge
- Borrar ramas después de merge exitoso
- Mantener ramas actualizadas con `develop` mediante rebase frecuente

#### **Ejemplo de Flujo**
```bash
# Crear feature branch
git checkout develop
git pull origin develop
git checkout -b feature/security-headers

# Trabajar y commitear
git add .
git commit -m "security(headers): add CSP and HSTS config"

# Actualizar con develop antes de PR
git fetch origin develop
git rebase origin/develop

# Push y crear PR
git push origin feature/security-headers
```

---

## 4. Buenas Prácticas de Seguridad en Frontend

### 4.1 Prevención de XSS (Cross-Site Scripting)

#### **Sanitización de Contenido Dinámico**
```typescript
// ✅ Correcto: React escapa por defecto
<div>{userInput}</div>

// ⚠️ Peligroso: dangerouslySetInnerHTML
// Solo usar si el contenido está sanitizado
import DOMPurify from 'dompurify';
<div dangerouslySetInnerHTML={{ 
  __html: DOMPurify.sanitize(userInput) 
}} />

// ❌ NUNCA hacer esto
<div dangerouslySetInnerHTML={{ __html: userInput }} />
```

#### **Validación de Inputs**
```typescript
// ✅ Validar y sanitizar antes de usar
const sanitizeInput = (input: string): string => {
  return input.trim().replace(/[<>]/g, '');
};

// Validar rangos numéricos
const validateRiskValue = (value: number): boolean => {
  return value >= 1 && value <= 5 && Number.isInteger(value);
};
```

### 4.2 Manejo Seguro de Datos Sensibles

#### **NO almacenar datos sensibles en localStorage**
```typescript
// ❌ NUNCA
localStorage.setItem('apiKey', apiKey);
localStorage.setItem('password', password);

// ✅ Usar variables de entorno para configuración
const API_URL = import.meta.env.VITE_API_URL;

// ✅ Tokens en cookies httpOnly (manejado por backend)
// Frontend solo lee mediante API
```

#### **Evitar Exponer Información en Console**
```typescript
// ❌ No dejar en producción
console.log('User data:', userData);
console.log('API Response:', response);

// ✅ Usar condicionales para debug
if (import.meta.env.DEV) {
  console.debug('Debug info:', data);
}
```

### 4.3 Protección Contra CSRF (Cross-Site Request Forgery)

- Usar tokens CSRF proporcionados por el backend
- Validar origen de peticiones mediante headers
- Configurar SameSite en cookies

```typescript
// Ejemplo de header CSRF
axios.defaults.headers.common['X-CSRF-TOKEN'] = getCsrfToken();
```

### 4.4 Content Security Policy (CSP)

Configurar CSP en `index.html` o mediante servidor:

```html
<meta http-equiv="Content-Security-Policy" 
      content="
        default-src 'self';
        script-src 'self' 'unsafe-inline';
        style-src 'self' 'unsafe-inline';
        img-src 'self' data: https:;
        connect-src 'self' https://api.riskcalculator.com;
      ">
```

### 4.5 Manejo de Errores Seguro

```typescript
// ❌ Exponer detalles técnicos
catch (error) {
  showToast(`Error: ${error.stack}`);
}

// ✅ Mensaje genérico al usuario, log detallado interno
catch (error) {
  console.error('[RiskAPI] Error:', error); // Solo en dev
  showToast('Ocurrió un error al procesar tu solicitud. Por favor intenta nuevamente.');
  
  // Enviar a sistema de logging (Sentry, etc.)
  logErrorToMonitoring(error);
}
```

### 4.6 Dependencias y Supply Chain Security

- **Auditar regularmente** las dependencias con `npm audit`
- **Usar versiones exactas** en `package.json` cuando sea crítico
- **Revisar permisos** de paquetes de terceros
- **Evitar paquetes abandonados** o sin mantenimiento
- **Usar Snyk o similar** para escaneo continuo de vulnerabilidades

---

## 5. Herramientas de Linting y Escaneo Local

### 5.1 ESLint

#### **Configuración Obligatoria**

El proyecto debe usar ESLint con reglas de seguridad y calidad:

```javascript
// eslint.config.js
export default [
  {
    rules: {
      'no-eval': 'error',                    // Prevenir eval()
      'no-implied-eval': 'error',            // Prevenir setTimeout con strings
      'no-new-func': 'error',                // Prevenir new Function()
      'no-console': 'warn',                  // Advertir console.log
      '@typescript-eslint/no-explicit-any': 'error', // Evitar any
      'react/no-danger': 'error',            // Evitar dangerouslySetInnerHTML
      'react-hooks/rules-of-hooks': 'error', // Reglas de hooks
      'react-hooks/exhaustive-deps': 'warn', // Dependencias de useEffect
    }
  }
];
```

#### **Ejecutar ESLint Pre-Commit**
```bash
# En cada commit (usar husky)
npm run lint

# Autofix cuando sea posible
npm run lint -- --fix
```

### 5.2 TypeScript Strict Mode

Mantener configuración estricta en `tsconfig.json`:

```json
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true,
    "strictFunctionTypes": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true
  }
}
```

**Beneficios de seguridad:**
- Previene errores de tipo que pueden derivar en vulnerabilidades
- Detecta código muerto o no utilizado
- Obliga a manejar casos null/undefined

### 5.3 Snyk (Escaneo de Vulnerabilidades)

#### **Instalación y Uso Local**
```bash
# Instalar Snyk CLI
npm install -g snyk

# Autenticarse
snyk auth

# Escanear proyecto
snyk test

# Escanear código fuente (SAST)
snyk code test

# Monitorear continuamente
snyk monitor
```

#### **Integración en Workflow**
- Ejecutar `snyk test` antes de cada PR
- Configurar Snyk GitHub Integration para escaneo automático
- **No mergear** si existen vulnerabilidades críticas o altas sin resolver

### 5.4 Prettier (Formato de Código)

Usar Prettier para consistencia visual:

```json
// .prettierrc
{
  "semi": true,
  "singleQuote": true,
  "tabWidth": 2,
  "trailingComma": "es5",
  "printWidth": 100
}
```

```bash
# Formatear todo el código
npm run format

# Verificar formato
npm run format:check
```

### 5.5 Husky + Lint-Staged

Configurar hooks pre-commit:

```json
// package.json
{
  "lint-staged": {
    "*.{ts,tsx}": [
      "eslint --fix",
      "prettier --write",
      "snyk code test"
    ]
  }
}
```

---

## 6. Uso Responsable de Agentes de IA

### 6.1 Principios Generales

El uso de herramientas de IA (GitHub Copilot, ChatGPT, etc.) está **permitido y fomentado**, pero debe ser **responsable y consciente**.

#### **Responsabilidades del Desarrollador**

1. **Revisar TODO el código generado por IA**
   - No aceptar sugerencias ciegamente
   - Verificar que cumple con los estándares del proyecto
   - Comprobar que no introduce vulnerabilidades

2. **NO compartir código propietario o sensible**
   - No copiar código completo del proyecto a herramientas externas
   - No compartir APIs keys, tokens o credenciales
   - Usar contexto genérico al consultar

3. **Validar seguridad del código generado**
   - Escanear con Snyk o herramientas SAST
   - Verificar que no usa dependencias inseguras
   - Comprobar que sigue principios de Security by Design

4. **Atribuir autoría correctamente**
   - En commits, el desarrollador es responsable del código
   - Documentar si se usó IA para generación masiva (en PR)

### 6.2 Casos de Uso Recomendados

#### **✅ Uso Apropiado**

- Generar boilerplate de componentes React
- Sugerir tipos TypeScript complejos
- Escribir tests unitarios
- Refactorizar código existente
- Documentar funciones complejas
- Generar expresiones regulares validadas

#### **⚠️ Uso con Precaución**

- Lógica de negocio crítica (revisar exhaustivamente)
- Manejo de autenticación/autorización
- Código que interactúa con APIs externas
- Configuraciones de seguridad (CSP, CORS, etc.)

#### **❌ Uso Prohibido**

- Generar credenciales o secretos
- Compartir código completo del proyecto con servicios externos no aprobados
- Aceptar código sin entender su funcionamiento
- Usar IA para "saltarse" code reviews

### 6.3 Validación de Código Generado por IA

**Checklist obligatorio antes de commitear código de IA:**

- [ ] He leído y comprendido todo el código
- [ ] Cumple con las convenciones de nomenclatura del proyecto
- [ ] No introduce dependencias innecesarias o inseguras
- [ ] Pasó ESLint sin errores
- [ ] Pasó escaneo de Snyk
- [ ] Tiene tests (si aplica)
- [ ] La documentación es precisa y útil

### 6.4 Privacidad y Compliance

- **Verificar** políticas de privacidad de la herramienta de IA usada
- **NO usar** herramientas que entrenen modelos con código del proyecto (verificar opt-out)
- **Cumplir** con políticas corporativas sobre uso de IA si existen

---

## 7. Validación y Pruebas (BDD y Unit Tests)

### 7.1 Estrategia de Testing

El proyecto debe mantener cobertura de tests en:

- **Servicios (services/)**: 80% mínimo
- **Hooks (hooks/)**: 70% mínimo
- **Componentes (components/)**: 60% mínimo

#### **Herramientas**

- **Vitest**: Para unit tests y integration tests
- **React Testing Library**: Para tests de componentes
- **MSW (Mock Service Worker)**: Para mockear APIs
- **Playwright/Cypress**: Para E2E (opcional)

### 7.2 Unit Tests

#### **Estructura de Tests**
```typescript
// useRiskCalculation.test.ts
import { renderHook } from '@testing-library/react';
import { useRiskCalculation } from './useRiskCalculation';

describe('useRiskCalculation', () => {
  it('should calculate correct risk level', () => {
    const { result } = renderHook(() => useRiskCalculation());
    
    const risk = result.current.calculateRisk(5, 5);
    
    expect(risk.level).toBe('critical');
    expect(risk.score).toBe(25);
  });
  
  it('should handle invalid input gracefully', () => {
    const { result } = renderHook(() => useRiskCalculation());
    
    expect(() => {
      result.current.calculateRisk(-1, 5);
    }).toThrow('Invalid risk value');
  });
});
```

#### **Reglas de Unit Tests**

- Usar patrón **AAA** (Arrange, Act, Assert)
- Un test por cada caso de uso importante
- Nombres descriptivos: `should [expected behavior] when [condition]`
- Tests independientes (no depender del orden de ejecución)
- Mockear dependencias externas (APIs, localStorage, etc.)

### 7.3 Tests de Componentes

```typescript
// RiskMatrix.test.tsx
import { render, screen } from '@testing-library/react';
import { RiskMatrix } from './RiskMatrix';

describe('RiskMatrix', () => {
  it('should render risk matrix with correct data', () => {
    const mockData = { impact: 4, probability: 3 };
    
    render(<RiskMatrix data={mockData} />);
    
    expect(screen.getByText(/Impact: 4/i)).toBeInTheDocument();
    expect(screen.getByText(/Probability: 3/i)).toBeInTheDocument();
  });
  
  it('should sanitize user input before rendering', () => {
    const maliciousData = { description: '<script>alert("XSS")</script>' };
    
    render(<RiskMatrix data={maliciousData} />);
    
    // No debe ejecutar script
    expect(screen.queryByText(/<script>/i)).not.toBeInTheDocument();
  });
});
```

### 7.4 BDD (Behavior-Driven Development)

#### **Estructura de Features**

Para features críticas de seguridad, documentar en formato BDD:

```gherkin
Feature: Validación de entrada de usuario
  Como desarrollador
  Quiero validar las entradas del usuario
  Para prevenir ataques de inyección

  Scenario: Usuario ingresa datos válidos
    Given el usuario está en la calculadora de riesgos
    When ingresa impact "4" y probability "3"
    Then el sistema calcula el riesgo correctamente
    And muestra el resultado sin errores

  Scenario: Usuario intenta inyectar script malicioso
    Given el usuario está en el campo de descripción
    When ingresa "<script>alert('XSS')</script>"
    Then el sistema sanitiza la entrada
    And NO ejecuta el script
    And muestra la descripción sanitizada
```

#### **Implementación en Tests**
```typescript
// riskCalculation.bdd.test.ts
describe('Feature: Validación de entrada de usuario', () => {
  describe('Scenario: Usuario ingresa datos válidos', () => {
    it('should calculate risk correctly with valid input', () => {
      // Given
      const impact = 4;
      const probability = 3;
      
      // When
      const result = calculateRisk(impact, probability);
      
      // Then
      expect(result.score).toBe(12);
      expect(result.level).toBe('high');
    });
  });
  
  describe('Scenario: Usuario intenta inyectar script malicioso', () => {
    it('should sanitize malicious input', () => {
      // Given
      const maliciousInput = '<script>alert("XSS")</script>';
      
      // When
      const sanitized = sanitizeInput(maliciousInput);
      
      // Then
      expect(sanitized).not.toContain('<script>');
      expect(sanitized).not.toContain('alert');
    });
  });
});
```

### 7.5 Ejecución de Tests

#### **Comandos Estándar**
```bash
# Ejecutar todos los tests
npm test

# Ejecutar con cobertura
npm run test:coverage

# Ejecutar en watch mode (desarrollo)
npm run test:watch

# Ejecutar tests de seguridad específicos
npm run test:security
```

#### **Criterios de Aceptación**

Antes de mergear un PR:

- [ ] Todos los tests pasan (100%)
- [ ] Cobertura no disminuye respecto a develop
- [ ] Tests de seguridad incluidos para features críticas
- [ ] No hay tests deshabilitados (skip/only) sin justificación

---

## 8. Revisión de Código y Control de Calidad

### 8.1 Proceso de Code Review

#### **Flujo Obligatorio**

1. **Desarrollador** crea PR desde feature branch
2. **Asigna reviewers** (mínimo 1, recomendado 2)
3. **CI/CD** ejecuta checks automáticos:
   - ESLint
   - TypeScript compilation
   - Tests unitarios
   - Snyk security scan
   - Build production
4. **Reviewers** analizan código y comentan
5. **Desarrollador** atiende comentarios
6. **Aprobación** de al menos 1 reviewer
7. **Merge** a develop (squash o merge commit según caso)

#### **Responsabilidades del Reviewer**

- Verificar que cumple los estándares de este documento
- Validar lógica de negocio y seguridad
- Comprobar que existen tests adecuados
- Sugerir mejoras de arquitectura o rendimiento
- Aprobar solo si el código es "production-ready"

### 8.2 Checklist de Code Review

#### **Seguridad**
- [ ] No hay vulnerabilidades introducidas (Snyk passed)
- [ ] Inputs de usuario están validados y sanitizados
- [ ] No se exponen datos sensibles en logs o UI
- [ ] No hay secretos hardcodeados
- [ ] Se manejan errores de forma segura

#### **Calidad de Código**
- [ ] Sigue convenciones de nomenclatura
- [ ] Código es legible y mantenible
- [ ] No hay duplicación innecesaria (DRY)
- [ ] Funciones tienen responsabilidad única (SRP)
- [ ] Comentarios útiles solo donde sea necesario

#### **Testing**
- [ ] Existen tests para la nueva funcionalidad
- [ ] Tests cubren casos edge y errores
- [ ] Cobertura de tests es adecuada
- [ ] Tests son independientes y reproducibles

#### **Performance**
- [ ] No hay re-renders innecesarios
- [ ] Uso adecuado de useMemo/useCallback si aplica
- [ ] No hay memory leaks (useEffect cleanup)
- [ ] Tamaño del bundle no aumenta excesivamente

#### **Documentación**
- [ ] Funciones complejas están documentadas
- [ ] README actualizado si cambió funcionalidad principal
- [ ] Tipos TypeScript están bien definidos

### 8.3 Niveles de Prioridad en Reviews

#### **🔴 Bloqueo (Must Fix)**
- Vulnerabilidades de seguridad
- Bugs críticos que rompen funcionalidad
- Violaciones graves de estándares

#### **🟡 Importante (Should Fix)**
- Problemas de rendimiento
- Violaciones menores de convenciones
- Falta de tests en código crítico
- Código difícil de mantener

#### **🟢 Sugerencia (Nice to Have)**
- Mejoras de legibilidad
- Optimizaciones no críticas
- Comentarios adicionales

### 8.4 Merge Strategies

#### **Squash Merge**
Usar para feature branches con muchos commits pequeños:
```bash
git merge --squash feature/my-feature
```

**Ventajas:**
- Historial limpio en `develop`
- Un commit por feature

#### **Merge Commit**
Usar para releases o features grandes con commits significativos:
```bash
git merge --no-ff release/v1.2.0
```

**Ventajas:**
- Preserva historial completo
- Clara separación de features

#### **Rebase**
Usar para actualizar feature branch con `develop`:
```bash
git rebase develop
```

**Ventajas:**
- Historial lineal
- Facilita code review

### 8.5 Protección de Ramas

Configurar protecciones en GitHub/GitLab para `main` y `develop`:

- ✅ Require pull request before merging
- ✅ Require approvals (mínimo 1)
- ✅ Require status checks to pass (CI/CD)
- ✅ Require branches to be up to date
- ✅ Require conversation resolution
- ✅ Do not allow bypassing (ni siquiera admins)

---

## Firmas y Aprobaciones del Equipo de Desarrollo

Este documento ha sido revisado y aprobado por el equipo de desarrollo del proyecto Risk Calculator. Su cumplimiento es obligatorio para todos los miembros del equipo y colaboradores externos.

### Aprobaciones

| Rol                          | Nombre                  | Firma                  | Fecha       |
|------------------------------|-------------------------|------------------------|-------------|
| **Tech Lead / Arquitecto**   | _____________________   | ______________________ | __________ |
| **Security Champion**        | _____________________   | ______________________ | __________ |
| **Frontend Lead**            | _____________________   | ______________________ | __________ |
| **QA/Testing Lead**          | _____________________   | ______________________ | __________ |
| **DevOps Engineer**          | _____________________   | ______________________ | __________ |

### Control de Versiones del Documento

| Versión | Fecha          | Autor              | Cambios Realizados                                |
|---------|----------------|--------------------|---------------------------------------------------|
| 1.0.0   | 2025-10-28     | Equipo Desarrollo  | Versión inicial - SSDLC Part 1                    |
|         |                |                    |                                                   |
|         |                |                    |                                                   |

### Declaración de Compromiso

Al firmar este documento, cada miembro del equipo se compromete a:

1. **Leer y comprender** íntegramente este documento
2. **Aplicar** los estándares y buenas prácticas en su trabajo diario
3. **Promover** la cultura de seguridad en el equipo
4. **Reportar** vulnerabilidades o incumplimientos detectados
5. **Actualizar** sus conocimientos en seguridad y mejores prácticas
6. **Colaborar** en la mejora continua de estos estándares

---

### Recursos Adicionales

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [React Security Best Practices](https://snyk.io/blog/10-react-security-best-practices/)
- [TypeScript Security Guide](https://cheatsheetseries.owasp.org/cheatsheets/Nodejs_Security_Cheat_Sheet.html)
- [Snyk Documentation](https://docs.snyk.io/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

### Contacto y Soporte

Para dudas sobre este documento o reportar problemas de seguridad:

- **Security Issues:** security@riskcalculator.com (reporte privado)
- **Preguntas Generales:** dev-team@riskcalculator.com
- **Documentación Interna:** [Wiki del Proyecto](#)

---

**© 2025 Risk Calculator Development Team**  
**Confidencial - Solo para uso interno del equipo de desarrollo**
