## 🧠 Evidencia 1 — Escáner Estático Local con Snyk (VS Code Extension)

**Fecha de ejecución:** 28/10/2025  
**Herramienta utilizada:** [Snyk Vulnerability Scanner](https://marketplace.visualstudio.com/items?itemName=snyk-security.snyk-vulnerability-scanner)  
**Entorno de ejecución:** Visual Studio Code — Proyecto `RISK-CALCULATOR/frontend/my-app`

---

### 🔍 Resultados del análisis

El escaneo se realizó de manera local utilizando la extensión de **Snyk**, con el objetivo de detectar vulnerabilidades en dependencias de código abierto, código fuente y configuraciones del entorno.

#### **1. Open Source Security**
- **Librería:** `axios@1.1.1`  
- **Tipo de vulnerabilidad:** Allocation of Resources Without Limits or Throttling  
- **Riesgo:** ⚠️ Medio  
- **Recomendación:** Actualizar a una versión segura (`axios >= 1.7.2`) para reducir el riesgo de abuso de recursos en peticiones HTTP.

#### **2. Code Security**
- **Archivo afectado:** `RiskCalculatorController.cs`  
- **Tipo de vulnerabilidad:** Log Forging (riesgo de manipulación de registros)  
- **Riesgo:** 🟡 Bajo  
- **Recomendación:** Sanitizar todas las entradas antes de registrarlas en logs y aplicar políticas de logging seguro en el backend.

#### **3. Configuration Issues**
- ✅ **Resultado:** No se detectaron configuraciones inseguras en el entorno de desarrollo.

---

### 📁 Evidencias

Las evidencias generadas se almacenaron en la siguiente ruta del proyecto:

frontend/
└── docs/
└── evidences/
├── snyk_scan.png
└── snyk_scan_details.txt

**Archivo:** `snyk_scan.png` — contiene el resultado visual del escaneo realizado desde la extensión.  
**Archivo:** `snyk_scan_details.txt` — contiene el resultado del escaneo detallado en formato de texto (CLI opcional).

---

### 🧩 Conclusión

El análisis con **Snyk VS Code Extension** permitió identificar vulnerabilidades reales en dependencias y código fuente, cumpliendo con los requerimientos del punto **1. “Implementar escáner estático local (VS Code Extension)”** del proyecto SSDLC.  

El uso de esta herramienta refuerza la **seguridad preventiva**, fomenta la **detección temprana de riesgos**, y contribuye a mantener la **integridad del ciclo de vida seguro de desarrollo (SSDLC)** dentro del proyecto *Risk Calculator*.

---

## 🔎 Evidencia 2 — Análisis de Calidad de Código con ESLint

**Fecha de ejecución:** 28/10/2025  
**Herramienta utilizada:** ESLint v8.57.1 con TypeScript ESLint  
**Comando ejecutado:** `npx eslint "src/**/*.{ts,tsx,js,jsx}"`  
**Entorno de ejecución:** Node.js — Proyecto `RISK-CALCULATOR/frontend/my-app`

---

### 🔍 Resultados del análisis

El escaneo se realizó utilizando **ESLint** con reglas de TypeScript y React para detectar problemas de calidad de código, vulnerabilidades potenciales y malas prácticas de programación.

#### **Resumen de problemas detectados:**
- 🔴 **7 Errores críticos**
- ⚠️ **2 Advertencias**
- 📊 **5 archivos afectados**

---

### 📋 Detalle de problemas por categoría

#### **1. Type Safety Issues (Seguridad de Tipos)**
**Regla:** `@typescript-eslint/no-explicit-any` — **6 ocurrencias**  
**Severidad:** 🔴 Error  
**Riesgo:** Alto - Pérdida de type safety, posibles errores en runtime

| Archivo | Línea | Descripción |
|---------|-------|-------------|
| `src/App.tsx` | 50:46 | Uso de `any` en tipo de datos |
| `src/App.tsx` | 86:72 | Uso de `any` en tipo de datos |
| `src/services/riskCalculatorApi.ts` | 39:40 | Uso de `any` en parámetros de función |
| `src/services/riskCalculatorApi.ts` | 45:22 | Uso de `any` en tipo de retorno |
| `src/types/riskCalculator.ts` | 28:39 | Uso de `any` en definición de tipo |
| `src/types/riskCalculator.ts` | 119:39 | Uso de `any` en definición de tipo |

**Impacto de seguridad:**
- ❌ **Bypass de validación de tipos:** El uso de `any` elimina la protección que ofrece TypeScript
- ❌ **Posibles errores en runtime:** No se detectan errores de tipo en tiempo de compilación
- ❌ **Vulnerabilidad a inyección de datos:** Sin validación estricta de tipos, datos maliciosos pueden pasar

**Recomendación:**
```typescript
// ❌ Evitar
const handleData = (data: any) => { ... }

// ✅ Usar tipos específicos o genéricos
const handleData = <T extends RiskData>(data: T) => { ... }

// ✅ Usar Zod para validación en runtime
import { z } from 'zod';
const RiskDataSchema = z.object({
  threat: z.number().min(1).max(5),
  vulnerability: z.number().min(1).max(5)
});
```

---

#### **2. Logic Errors (Errores Lógicos)**
**Regla:** `no-dupe-else-if` — **1 ocurrencia**  
**Severidad:** 🔴 Error  
**Riesgo:** Medio - Código inaccesible, lógica incorrecta

| Archivo | Línea | Descripción |
|---------|-------|-------------|
| `src/services/riskCalculatorApi.ts` | 69:16 | Condición duplicada en if-else-if chain |

**Impacto de seguridad:**
- ⚠️ **Dead code:** Rama de código que nunca se ejecutará
- ⚠️ **Comportamiento inesperado:** Puede causar que validaciones de seguridad se omitan

**Recomendación:**
```typescript
// ❌ Evitar condiciones duplicadas
if (status === 400) {
  // ...
} else if (status === 500) {
  // ...
} else if (status === 500) {  // ← Esta condición nunca se ejecuta
  // ...
}

// ✅ Corregir lógica
if (status === 400) {
  // ...
} else if (status === 500) {
  // ...
} else if (status === 503) {  // Condición única
  // ...
}
```

---

#### **3. React Hooks Issues (Problemas de Hooks)**
**Regla:** `react-hooks/exhaustive-deps` — **1 ocurrencia**  
**Severidad:** ⚠️ Warning  
**Riesgo:** Bajo - Comportamiento inesperado en efectos

| Archivo | Línea | Descripción |
|---------|-------|-------------|
| `src/components/Toast.tsx` | 29:6 | useEffect con dependencia faltante: 'handleClose' |

**Impacto:**
- 🔄 **Stale closures:** El efecto puede usar valores obsoletos
- 🐛 **Bugs difíciles de rastrear:** Comportamiento inconsistente en re-renders

**Recomendación:**
```typescript
// ✅ Opción 1: Agregar la dependencia
useEffect(() => {
  // ...
}, [handleClose]);

// ✅ Opción 2: Usar useCallback para estabilizar la función
const handleClose = useCallback(() => {
  // ...
}, []);

useEffect(() => {
  // ...
}, [handleClose]);
```

---

#### **4. Fast Refresh Issues (Problemas de Hot Module Replacement)**
**Regla:** `react-refresh/only-export-components` — **1 ocurrencia**  
**Severidad:** ⚠️ Warning  
**Riesgo:** Muy Bajo - Solo afecta experiencia de desarrollo

| Archivo | Línea | Descripción |
|---------|-------|-------------|
| `src/contexts/ThemeContext.tsx` | 10:14 | Exportación mixta de componentes y constantes |

**Recomendación:**
```typescript
// ❌ Evitar exports mixtos
export const ThemeContext = createContext(...);
export const ThemeProvider = () => { ... };

// ✅ Separar en archivos diferentes
// theme-context.ts
export const ThemeContext = createContext(...);

// ThemeProvider.tsx
import { ThemeContext } from './theme-context';
export const ThemeProvider = () => { ... };
```

---

### 📊 Análisis de Impacto por Severidad

```
🔴 ERRORES CRÍTICOS (7):
├── Type Safety: 6 (86%)
└── Logic Errors: 1 (14%)

⚠️ ADVERTENCIAS (2):
├── React Hooks: 1 (50%)
└── Fast Refresh: 1 (50%)
```

**Distribución por archivo:**
- `riskCalculatorApi.ts`: 3 errores (43%)
- `App.tsx`: 2 errores (29%)
- `riskCalculator.ts`: 2 errores (29%)
- `Toast.tsx`: 1 warning (14%)
- `ThemeContext.tsx`: 1 warning (14%)

---

### 🛡️ Recomendaciones de Seguridad

#### **Alta Prioridad (Semana 1):**
1. ✅ **Eliminar todos los `any` types** y reemplazar con tipos específicos o genéricos
2. ✅ **Corregir la condición duplicada** en `riskCalculatorApi.ts` línea 69
3. ✅ **Implementar validación con Zod** en todas las funciones que usan `any`

#### **Media Prioridad (Semana 2):**
4. ⚠️ **Corregir dependencias de React Hooks** en `Toast.tsx`
5. ⚠️ **Refactorizar exports** en `ThemeContext.tsx` para mejorar HMR

#### **Configuración Recomendada de ESLint:**
```javascript
// eslint.config.js - Reglas de seguridad adicionales
export default tseslint.config({
  rules: {
    '@typescript-eslint/no-explicit-any': 'error',
    '@typescript-eslint/no-unsafe-assignment': 'error',
    '@typescript-eslint/no-unsafe-member-access': 'error',
    '@typescript-eslint/no-unsafe-call': 'error',
    '@typescript-eslint/no-unsafe-return': 'error',
    'no-console': ['warn', { allow: ['warn', 'error'] }],
    'no-dupe-else-if': 'error',
    'react-hooks/exhaustive-deps': 'warn',
  }
});
```

---

### 📁 Evidencias

```bash
# Comando ejecutado
PS C:\Users\majif\Downloads\risk-calculator\frontend\my-app> npx eslint "src/**/*.{ts,tsx,js,jsx}"

# Salida completa
C:\Users\majif\Downloads\risk-calculator\frontend\my-app\src\App.tsx
  50:46  error  Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
  86:72  error  Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

C:\Users\majif\Downloads\risk-calculator\frontend\my-app\src\components\Toast.tsx
  29:6  warning  React Hook useEffect has a missing dependency: 'handleClose'. 
       Either include it or remove the dependency array  react-hooks/exhaustive-deps

C:\Users\majif\Downloads\risk-calculator\frontend\my-app\src\contexts\ThemeContext.tsx
  10:14  warning  Fast refresh only works when a file only exports components. 
        Use a new file to share constants or functions between components  
        react-refresh/only-export-components

C:\Users\majif\Downloads\risk-calculator\frontend\my-app\src\services\riskCalculatorApi.ts
  39:40  error  Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
  45:22  error  Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
  69:16  error  This branch can never execute. Its condition is a duplicate or covered 
        by previous conditions in the if-else-if chain  no-dupe-else-if

C:\Users\majif\Downloads\risk-calculator\frontend\my-app\src\types\riskCalculator.ts
  28:39  error  Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
  119:39  error  Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

✖ 9 problems (7 errors, 2 warnings)
```

**Archivos de evidencia generados:**
```
frontend/
└── docs/
    └── evidences/
        ├── eslint_scan_output.txt          ← Salida completa del comando
        ├── eslint_config.js                ← Configuración utilizada
        └── eslint_security_report.md       ← Este documento
```

---

### 🧩 Conclusión

El análisis con **ESLint** permitió identificar **9 problemas de calidad de código** que incluyen:
- ✅ **Riesgos de seguridad de tipos:** 6 usos de `any` que eliminan protección TypeScript
- ✅ **Errores lógicos:** 1 condición duplicada que genera código inaccesible
- ✅ **Mejoras de estabilidad:** 2 advertencias sobre hooks y HMR

**Cumplimiento con SSDLC:**
- 🎯 **Static Analysis:** Análisis estático automatizado integrado en el flujo de desarrollo
- 🎯 **Code Quality:** Detección temprana de problemas antes de commit
- 🎯 **Type Safety:** Enforcement de tipos estrictos para prevenir vulnerabilidades

**Próximos pasos:**
1. Corregir los 7 errores críticos (prioridad alta)
2. Integrar ESLint en pre-commit hooks con Husky
3. Agregar ESLint al pipeline de CI/CD
4. Establecer política de "zero warnings" para nuevos PRs

---

📅 **Última actualización:** 28/10/2025  
👤 **Autor de la evidencia:** _Equipo de Desarrollo — Proyecto Risk Calculator_ Marco Jimenez, Norman Alvarado, Jose Guadamuz, Melina Cruz