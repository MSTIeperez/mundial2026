# 🏆 Mundial 2026 — Fixture Predictor

Sistema completo para registrar y visualizar resultados del FIFA World Cup 2026.
**Backend:** .NET 8 · **Frontend:** Angular 18

---

## 📁 Estructura del Proyecto

```
mundial2026/
├── backend/                  # API .NET 8
│   ├── Controllers/
│   │   └── Controllers.cs    # AuthController, MatchesController, StandingsController
│   ├── Data/
│   │   ├── AppDbContext.cs   # Entity Framework + SQLite
│   │   └── DbSeeder.cs       # 48 equipos, 12 grupos, partidos del Mundial 2026
│   ├── Models/
│   │   └── Models.cs         # User, Team, Match, GroupStanding
│   ├── Services/
│   │   ├── AuthService.cs    # JWT login/registro
│   │   └── Services.cs       # MatchService, StandingsService
│   ├── Program.cs
│   ├── appsettings.json
│   └── Mundial2026.Api.csproj
│
└── frontend/                 # Angular 18 SPA
    └── src/
        └── app/
            ├── models/models.ts
            ├── services/
            │   ├── auth.service.ts
            │   ├── api.service.ts
            │   └── auth.guard.ts
            ├── pages/
            │   ├── login/login.component.ts
            │   ├── grupos/grupos.component.ts      # Partidos + tabla de posiciones
            │   └── eliminatorias/eliminatorias.component.ts  # Ronda de 32
            ├── app.component.ts   # Navbar + shell
            ├── app.routes.ts
            └── app.config.ts
```

---

## 🚀 Instalación y Ejecución

### Requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) + npm
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`

---

### 1️⃣ Backend (.NET)

```bash
cd backend

# Restaurar paquetes
dotnet restore

# Ejecutar (crea la BD SQLite automáticamente + seed de datos)
dotnet run
```

La API corre en: **http://localhost:5000**  
Swagger UI en: **http://localhost:5000/swagger**

> La BD SQLite (`mundial2026.db`) se crea automáticamente en la primera ejecución
> con los 48 equipos y todos los partidos de la fase de grupos del Mundial 2026.

**Usuario administrador por defecto:**
- Usuario: `admin`
- Contraseña: `Admin123!`

---

### 2️⃣ Frontend (Angular)

```bash
cd frontend

# Instalar dependencias
npm install

# Ejecutar servidor de desarrollo
ng serve
```

La app corre en: **http://localhost:4200**

---

## 🔐 Roles de Usuario

| Rol   | Permisos                                              |
|-------|-------------------------------------------------------|
| admin | Ver partidos, registrar/editar resultados             |
| user  | Ver partidos, ver tabla de posiciones (solo lectura)  |

Para promover un usuario a admin, actualiza la BD:
```sql
UPDATE Users SET Role = 'admin' WHERE Username = 'tu_usuario';
```

---

## 📋 Funcionalidades

### Vista Grupos (`/grupos`)
- Selector de grupo A–L con navegación rápida
- Listado de 6 partidos por grupo (3 jornadas × 2 partidos)
- Registro y edición de resultados (admin)
- **Tabla de posiciones en tiempo real** con criterios FIFA:
  - Puntos → Diferencia de goles → Goles a favor → Fairplay
- Indicador visual de clasificados (1°, 2°) y posibles 3er lugar

### Vista Eliminatorias (`/eliminatorias`)
- **Resumen de todos los grupos** (12 grupos, quién clasifica)
- **Ronda de 32 — emparejamientos FIFA 2026:**
  - 1° y 2° de cada grupo (24 equipos)
  - 8 mejores 3er lugares (8 equipos)
  - Cruces conforme a reglamento FIFA
- **Tracker de mejores 3er lugares** ordenados por criterios FIFA
- Los emparejamientos se actualizan en vivo según resultados

---

## 🌐 Endpoints API

### Auth
| Método | Endpoint             | Descripción              |
|--------|----------------------|--------------------------|
| POST   | /api/auth/login      | Iniciar sesión           |
| POST   | /api/auth/register   | Registrar usuario        |
| GET    | /api/auth/me         | Info del usuario actual  |

### Partidos
| Método | Endpoint                     | Descripción                     |
|--------|------------------------------|---------------------------------|
| GET    | /api/matches/groups          | Todos los partidos de grupos    |
| GET    | /api/matches/groups/{grupo}  | Partidos de un grupo específico |
| PUT    | /api/matches/{id}/score      | Registrar resultado (admin)     |
| GET    | /api/matches/knockout/{phase}| Todos los partidos eliminatorios |
| GET    | /api/matches/knockout/{phase}| Partidos eliminatorios por fase |

### Posiciones
| Método | Endpoint                    | Descripción                        |
|--------|-----------------------------|------------------------------------|
| GET    | /api/standings              | Todas las tablas de posiciones     |
| GET    | /api/standings/{grupo}      | Tabla de un grupo específico       |
| GET    | /api/standings/knockout-slots | Emparejamientos ronda de 32        |

---

## ⚙️ Configuración

### Cambiar JWT Secret (`backend/appsettings.json`)
```json
{
  "Jwt": {
    "Key": "TU_CLAVE_SECRETA_AQUI_MINIMO_32_CHARS"
  }
}
```

### Cambiar puerto del backend
Si el backend corre en un puerto distinto al 5000, actualiza en:
- `frontend/src/app/services/auth.service.ts` → `private readonly API`
- `frontend/src/app/services/api.service.ts` → `private readonly API`

---

## 📦 Paquetes NuGet (Backend)

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
<PackageReference Include="Microsoft.IdentityModel.Tokens" Version="7.0.3" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.3" />
```

---

## 🎨 Diseño

- Tema oscuro con paleta de colores FIFA (dorado, azul profundo, rojo)
- Tipografía: Bebas Neue (display) + Inter (body) + JetBrains Mono (datos)
- Diseño totalmente responsivo (mobile-first)
- Animaciones sutiles en transiciones

---

## 🗺️ Próximos pasos (extensiones)

- [ ] Rondas completas: Octavos → Cuartos → Semis → Final
- [ ] Sistema de predicciones por usuario con puntaje
- [ ] Notificaciones en tiempo real con SignalR
- [ ] Exportar tabla a PDF/Excel
- [ ] Modo torneo con múltiples instancias
