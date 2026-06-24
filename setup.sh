#!/bin/bash
# ─── Mundial 2026 - Script de Configuración Rápida ───────────────────────────
echo "🏆 Mundial 2026 - Fixture Predictor"
echo "====================================="

# Check .NET
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8 SDK no encontrado."
    echo "   Descarga en: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check Node
if ! command -v node &> /dev/null; then
    echo "❌ Node.js no encontrado."
    echo "   Descarga en: https://nodejs.org/"
    exit 1
fi

# Check Angular CLI
if ! command -v ng &> /dev/null; then
    echo "📦 Instalando Angular CLI..."
    npm install -g @angular/cli
fi

echo ""
echo "📦 Configurando Backend (.NET 8)..."
cd backend
dotnet restore
echo "✅ Backend listo"

echo ""
echo "📦 Instalando dependencias Frontend (Angular)..."
cd ../frontend
npm install
echo "✅ Frontend listo"

echo ""
echo "====================================="
echo "✅ INSTALACIÓN COMPLETA"
echo ""
echo "Para iniciar el sistema:"
echo ""
echo "  Terminal 1 (Backend):"
echo "  cd backend && dotnet run"
echo "  → API: http://localhost:5000"
echo "  → Swagger: http://localhost:5000/swagger"
echo ""
echo "  Terminal 2 (Frontend):"
echo "  cd frontend && ng serve"
echo "  → App: http://localhost:4200"
echo ""
echo "  Credenciales admin: admin / Admin123!"
echo "====================================="
