# 10. Deployment & Entornos

## Estrategia de entornos

MediPrax sigue un modelo de 3 entornos separados, adecuado para software médico con requisitos regulatorios (DSGVO, KBV, gematik):

| Entorno | Propósito | Datos | Ubicación |
|---------|-----------|-------|-----------|
| **INT** (Integración) | Pruebas de desarrollo, puede romperse | Demo/seed | Hetzner cloud |
| **VAL** (Validación) | Validación pre-release por Praxisleitung | Anonimizados | Hetzner cloud |
| **PROD** (Producción) | Datos reales de pacientes, uso diario | Reales, DSGVO | Servidor local en consulta |

### Principios

- **Aislamiento total de BD**: cada entorno tiene su propia instancia PostgreSQL y volumen Docker
- **Sin datos reales fuera de PROD**: INT y VAL solo usan datos demo o anonimizados
- **Paridad de entornos**: Docker garantiza que INT, VAL y PROD ejecutan el mismo artefacto
- **Producción local**: los datos de pacientes nunca salen de la red de la consulta

---

## Servidor cloud — Hetzner (INT + VAL)

**IP:** `46.225.170.6`
**OS:** Ubuntu 24.04 LTS
**Recursos:** 8 GB RAM, 150 GB SSD, 4 vCPUs
**Acceso:** SSH como `deployer` (grupo `docker`, sin sudo sin password)

### Entorno INT

| Parámetro | Valor |
|-----------|-------|
| URL | `http://46.225.170.6:8081` |
| Directorio | `~/mediprax-int/` |
| Docker project | `mediprax-int` |
| Puerto app | 8081 → 8080 (container) |
| BD | `mediprax_int` (volumen `mediprax-int_int-db-data`) |
| ASPNETCORE_ENVIRONMENT | `Development` |
| Branch | `main` (o feature branches para pruebas) |

### Entorno VAL

| Parámetro | Valor |
|-----------|-------|
| URL | `http://46.225.170.6:8082` |
| Directorio | `~/mediprax-val/` |
| Docker project | `mediprax-val` |
| Puerto app | 8082 → 8080 (container) |
| BD | `mediprax_val` (volumen `mediprax-val_val-db-data`) |
| ASPNETCORE_ENVIRONMENT | `Staging` |
| Branch | Tags de release candidate |

### Credenciales demo (ambos entornos)

| Email | Rol | Password |
|-------|-----|----------|
| `admin@neuropsych-bremen.de` | Admin | `mediprax2026` |
| `meier@neuropsych-bremen.de` | Arzt | `mediprax2026` |
| `schmidt@neuropsych-bremen.de` | Arzt | `mediprax2026` |
| `koch@neuropsych-bremen.de` | MFA | `mediprax2026` |

(Todos los usuarios de seed comparten la misma contraseña)

---

## Comandos de gestión

### Actualizar INT (desarrollo continuo)

```bash
cd ~/mediprax-int
git pull
docker compose -p mediprax-int up --build -d
```

### Desplegar release candidate en VAL

```bash
cd ~/mediprax-val
git fetch --tags
git checkout v1.1.0    # tag del release candidate
docker compose -p mediprax-val up --build -d
```

### Ver logs

```bash
docker compose -p mediprax-int logs app --tail=50 -f
docker compose -p mediprax-val logs app --tail=50 -f
```

### Reiniciar

```bash
docker compose -p mediprax-int restart app
docker compose -p mediprax-val restart app
```

### Reset completo (borrar BD y recrear)

```bash
cd ~/mediprax-int
docker compose -p mediprax-int down -v
docker compose -p mediprax-int up --build -d
```

### Estado general

```bash
docker ps --format 'table {{.Names}}\t{{.Status}}\t{{.Ports}}'
```

---

## Servidor local — Producción (futuro)

**OS:** Windows Server 2022
**Requisitos:** Docker Desktop (o Docker CE via WSL2) + Git
**Conexión TI:** Konnektor en la misma red local

La configuración Docker es idéntica a INT/VAL, con estos cambios:

| Parámetro | Valor |
|-----------|-------|
| Puerto app | 8080 |
| BD | `mediprax_prod` |
| ASPNETCORE_ENVIRONMENT | `Production` |
| EnsureCreated | No — usar migraciones explícitas |
| Datos | Reales, sin seed demo |
| Backups | `pg_dump` automatizado, cifrado, offsite |

### Diferencias con INT/VAL

- **Sin `EnsureCreated`** — migraciones manuales con `dotnet ef database update`
- **Sin seed demo** — el bloque `DemoSeedService.Seed()` solo corre si no hay datos
- **Backups cifrados** — diario con retención 30 días + semanal con retención 1 año
- **HTTPS obligatorio** — Caddy con certificado o detrás de proxy del Konnektor
- **Data Protection keys** — persistidos en volumen Docker (no efímeros)

---

## Fixes aplicados para Docker deploy

Estos cambios son necesarios al desplegar con `docker compose` y están aplicados en los directorios del servidor (no en el repo Git):

1. **`Dockerfile` línea 13**: `dotnet restore MediPrax.slnx` → `dotnet restore src/MediPrax.Server/MediPrax.Server.csproj` (test projects no se copian al container)
2. **`Program.cs`**: eliminado parámetro `createScopeForStatusCodePages` (no existe en SDK preview del servidor)
3. **`Program.cs`**: añadido `ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))` (modelo con cambios pendientes no capturados en migraciones)
4. **`Program.cs`**: `db.Database.EnsureCreated()` + seed ejecutados en todos los entornos (en vez de solo `Development`)
5. **`docker-compose.yml`**: PostgreSQL sin puerto expuesto al host (solo accesible dentro de la red Docker)

---

## Arquitectura de red

```
┌─── Hetzner (46.225.170.6) ───────────────────────────────────┐
│                                                               │
│  ┌─ mediprax-int ──────────┐  ┌─ mediprax-val ─────────────┐ │
│  │  app (:8081)            │  │  app (:8082)                │ │
│  │  db (internal:5432)     │  │  db (internal:5432)         │ │
│  │  net: mediprax-int      │  │  net: mediprax-val          │ │
│  │  vol: int-db-data       │  │  vol: val-db-data           │ │
│  └─────────────────────────┘  └─────────────────────────────┘ │
│                                                               │
│  Sin datos reales. Sin conexión TI.                           │
└───────────────────────────────────────────────────────────────┘

┌─── Consulta (Windows Server 2022) ───────────────────────────┐
│                                                               │
│  ┌─ mediprax-prod ─────────┐   ┌──────────────────────┐      │
│  │  app (:8080)            │   │  Konnektor TI        │      │
│  │  db (internal:5432)     │◄──┤  (misma LAN)         │      │
│  │  net: mediprax-prod     │   └──────────────────────┘      │
│  │  vol: prod-db-data      │                                  │
│  └─────────────────────────┘                                  │
│                                                               │
│  Datos reales. DSGVO. Backups cifrados.                       │
└───────────────────────────────────────────────────────────────┘
```

---

## Cuando haya dominio

Si se apunta un dominio a la IP de Hetzner, se puede añadir Caddy como reverse proxy con SSL automático (Let's Encrypt):

```
# Caddyfile
int.mediprax.example.com {
    reverse_proxy localhost:8081
}

val.mediprax.example.com {
    reverse_proxy localhost:8082
}
```
