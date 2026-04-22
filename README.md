# Itenium Forge — Feature Flags

Module Federation **remote** voor de [Itenium.Forge.Poc](https://github.com/Itenium-Forge/Itenium.Forge.Poc) shell. Demonstreert hoe een aparte microservice zijn frontend als lazy-loadable remote exposed, en hoe de backend via het Forge framework opgezet wordt.

---

## Structuur

```
Itenium.Forge.FeatureFlags/
├── FeatureFlags.Api/           .NET backend, poort 5200
├── FeatureFlags.Api.Tests/     NUnit integratie tests
└── feature-flags-ui/           React/Vite remote, poort 3001
```

---

## Opstarten

### Vereisten

- .NET 10 SDK
- Node.js 20+
- GitHub PAT met `read:packages` scope (voor de Forge NuGet feed)

### Backend

```bash
cd FeatureFlags.Api && dotnet run          # http://localhost:5200
```

### Frontend

```bash
cd feature-flags-ui
npm install
npm run build
npm run preview        # http://localhost:3001
```

### Testen

```bash
dotnet test
```

---

## Hoe werkt het?

`feature-flags-ui` exposed één module via Module Federation:

| Module | Wat |
|--------|-----|
| `featureFlags/App` | React component — toont de flags tabel |

De shell ontdekt de remote URL at runtime via `Shell.Api/apps` — er is geen hardcoded URL in de shell build. Pad (`/feature-flags`) en label (`Feature Flags`) worden afgeleid uit de naam via conventie.

De shell laadt de component lazy wanneer de gebruiker naar `/feature-flags` navigeert. De flags data wordt opgehaald via Shell.Api (`5100/api/flags`), die als proxy naar `FeatureFlags.Api` fungeert.

```
shell-ui (3000) → GET /apps    → Shell.Api (5100)              remote URL ophalen
               → lazy import   → feature-flags-ui (3001)       component laden
               → GET /api/flags → Shell.Api (5100)             proxy
                                   └── Refit → FeatureFlags.Api (5200)
```

---

## Forge Framework

FeatureFlags.Api gebruikt de volgende Forge packages (v0.3.14):

| Package | Wat het doet |
|---------|-------------|
| `Itenium.Forge.Settings` | appsettings laden, `IForgeSettings` valideren, metadata loggen bij startup |
| `Itenium.Forge.Logging` | Serilog bootstrap, structured request logging, log naar bestand |
| `Itenium.Forge.Controllers` | CORS, response compression (Brotli/Gzip), camelCase JSON |
| `Itenium.Forge.HealthChecks` | `/health/live` en `/health/ready` endpoints |

---

## Module Federation

`feature-flags-ui` gebruikt `@module-federation/vite` op Vite 7 als remote.

> **Opmerking:** Vite 8 (Rolldown) is niet compatibel met `@module-federation/vite` vanwege een CJS/ESM conflict in de gegenereerde virtual modules. Vite 7 (Rollup) werkt wel correct.

---

## NuGet configuratie

De Forge packages komen van GitHub Packages. Voeg een PAT toe:

```bash
dotnet nuget add source "https://nuget.pkg.github.com/Itenium-Forge/index.json" \
  --name itenium \
  --username <github-username> \
  --password <PAT-met-read:packages>
```
