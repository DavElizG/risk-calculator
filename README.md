## **LINKS**

- **API**: [https://risk-calculator-api.onrender.com/](https://risk-calculator-api.onrender.com/)
- **Demo**: [https://risk-calculator-a6f2.onrender.com/](https://risk-calculator-a6f2.onrender.com/)

# 🛡️ Cybersecurity Risk Assessment Calculator

A comprehensive cybersecurity risk assessment tool that calculates risk using the formula: **RISK = THREAT × VULNERABILITY**

## 🚀 Features

- **Interactive Risk Calculation**: Visual sliders for threat and vulnerability levels (1-10 scale)
- **Visual Risk Matrix**: Color-coded matrix showing risk levels
- **Comprehensive Threat Types**: 17+ predefined threat categories including:
  - Malware, Ransomware, Phishing
  - Supply Chain Attacks, Zero-Day Exploits
  - Cloud Security Threats, IoT Threats
  - Custom threat types support
- **Extensive Vulnerability Categories**: 18+ vulnerability categories including:
  - Software Vulnerabilities, Network Security
  - Access Control, Data Protection
  - Cloud Security, Mobile Security
  - Custom vulnerability categories support
- **Dark/Light Mode**: Toggle between themes
- **Real-time Notifications**: Toast notifications for errors and success
- **Professional UI**: Modern, responsive design with smooth animations

## 🏗️ Architecture

### Backend (.NET 8 Web API)
- **Framework**: ASP.NET Core 8
- **Validation**: FluentValidation + Data Annotations
- **Documentation**: Swagger/OpenAPI
- **Logging**: Serilog
- **CORS**: Configured for frontend integration

### Frontend (React + TypeScript)
- **Framework**: React 18 with TypeScript
- **Styling**: Tailwind CSS
- **State Management**: TanStack Query (React Query)
- **Build Tool**: Vite
- **Notifications**: Custom Toast system

## 📋 Prerequisites

- **.NET 8 SDK** or later
- **Node.js 18** or later
- **npm** or **yarn**

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone <https://github.com/DavElizG/risk-calculator.git>
cd risk-calculator
```

### 2. Setup Backend
```bash
cd backend/risk-calculator-api/risk-calculator-api
dotnet restore
dotnet run
```
The API will be available at:
- HTTP: `http://localhost:5062`
- HTTPS: `https://localhost:7070`
- Swagger UI: `http://localhost:5062`

### 3. Setup Frontend
```bash
cd frontend/my-app
npm install
npm run dev
```
The frontend will be available at: `http://localhost:5173`

## 🔧 Configuration

### Backend Configuration
- **Port Configuration**: Edit `launchSettings.json`
- **CORS Settings**: Configure in `Program.cs`
- **Logging**: Configure in `Program.cs` (Serilog)

### Frontend Configuration
- **API URL**: Set `VITE_API_URL` in `.env` file
- **Default**: `VITE_API_URL=http://localhost:5062/api`

## 🧪 API Endpoints

### Risk Calculation
- `POST /api/RiskCalculator/calculate` - Calculate risk
- `GET /api/RiskCalculator/matrix` - Get risk matrix

### Configuration
- `GET /api/RiskCalculator/threat-types` - Get available threat types
- `GET /api/RiskCalculator/vulnerability-categories` - Get vulnerability categories

### History & Recommendations
- `POST /api/RiskCalculator/history` - Save calculation to history
- `GET /api/RiskCalculator/history` - Get calculation history
- `GET /api/RiskCalculator/recommendations` - Get risk mitigation recommendations

### Health Check
- `GET /api/RiskCalculator/health` - API health status

## 📊 Risk Calculation Details

### Risk Levels (1-100 scale)
- **1-20**: Very Low (Green)
- **21-35**: Low (Yellow)
- **36-55**: Medium (Orange)
- **56-75**: High (Red)
- **76-100**: Very High (Dark Red)

### Threat Levels (1-10)
- Higher values indicate greater threat likelihood and impact potential

### Vulnerability Levels (1-10)
- Higher values indicate greater system vulnerability and easier exploitation

## 🛠️ Development

### Project Structure
```
risk-calculator/
├── backend/
│   └── risk-calculator-api/
│       └── risk-calculator-api/
│           ├── Controllers/
│           ├── Models/
│           ├── Services/
│           ├── Validators/
│           └── Middleware/
├── frontend/
│   └── my-app/
│       ├── src/
│       │   ├── components/
│       │   ├── hooks/
│       │   ├── services/
│       │   ├── types/
│       │   └── contexts/
│       └── public/
└── README.md
```

### Key Features Implementation
- **Validation**: Dual validation with FluentValidation and Data Annotations
- **Error Handling**: Global exception middleware
- **Security**: CORS configuration, security headers
- **UI/UX**: Dark mode, responsive design, toast notifications
- **Type Safety**: Full TypeScript implementation

## 🚀 Deployment

### Backend Deployment
```bash
cd backend/risk-calculator-api/risk-calculator-api
dotnet publish -c Release
```

### Frontend Deployment
```bash
cd frontend/my-app
npm run build
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 👨‍💻 Author

Professional Cybersecurity Risk Assessment Tool
© 2025

---

**Formula**: RISK = THREAT × VULNERABILITY

Calculate cybersecurity risk with precision and confidence! 🛡️
