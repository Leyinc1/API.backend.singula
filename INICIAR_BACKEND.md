# 🚀 Cómo Iniciar el Backend

## ⚠️ Error Resuelto: DirectoryNotFoundException

**Problema:** El backend no arrancaba con error `DirectoryNotFoundException` en `WebApplication.CreateBuilder(args)`

**Causa:** Faltaba la carpeta `wwwroot` que ASP.NET Core necesita para servir archivos estáticos (como PDFs generados).

**Solución:** ✅ Se creó la carpeta `wwwroot` vacía en el proyecto.

---

## 🔧 Error Actual: Archivo bloqueado por Visual Studio

Si ves este error:
```
MSB3027: No se pudo copiar Singula.Core.dll. El archivo se ha bloqueado por: "Microsoft Visual Studio 2022"
```

**Causa:** Visual Studio tiene el proyecto abierto y bloquea los archivos DLL durante la compilación.

---

## ✅ Solución: Iniciar Backend desde Terminal

### Opción 1: Cerrar Visual Studio (Recomendado)

1. **Cierra Visual Studio 2022 completamente**
2. Abre PowerShell
3. Ejecuta:
```powershell
cd "c:\DESARROLLO DE APLICACIONES WEB\PROYECTO FINAL\API.backend.singula\API.backend.singula"
dotnet run
```

4. Deberías ver:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5192
```

### Opción 2: Desde Visual Studio

1. Abre `API.backend.singula.sln` en Visual Studio
2. Presiona **F5** o click en **▶ Start**
3. Espera a que compile y arranque

⚠️ **IMPORTANTE:** No ejecutes `dotnet run` mientras Visual Studio esté abierto con el proyecto cargado.

---

## 🔍 Verificar que el Backend está Corriendo

### Desde el navegador:
```
http://localhost:5192/api/ConfigSla
```

Deberías ver un JSON con configuraciones SLA.

### Desde PowerShell:
```powershell
Invoke-WebRequest -Uri "http://localhost:5192/api/ConfigSla" -UseBasicParsing
```

---

## 📱 Conectar App Móvil al Backend

### 1. Encuentra tu IP local:
```powershell
ipconfig
```

Busca: `Dirección IPv4. . . : 192.168.X.X`

### 2. Actualiza RetrofitClient.kt:

**Archivo:** `SLATrackerAPP/app/src/main/java/dev/esandamzapp/slatrackerapp/data/network/RetrofitClient.kt`

**Línea 13:**
```kotlin
private const val BASE_URL = "http://TU_IP_AQUI:5192/api/"
```

Ejemplo:
```kotlin
private const val BASE_URL = "http://192.168.10.100:5192/api/"
```

### 3. Verifica desde el celular:

Abre el navegador del celular y ve a:
```
http://TU_IP:5192/api/ConfigSla
```

Si ves el JSON, ¡la conexión funciona! 🎉

---

## 🛑 Detener el Backend

**Si ejecutaste con `dotnet run`:**
- Presiona `Ctrl + C` en la terminal

**Si ejecutaste desde Visual Studio:**
- Click en el botón **⏹ Stop** o presiona `Shift + F5`

---

## 📋 Checklist Antes de Probar la App Móvil

- [ ] Carpeta `wwwroot` existe (ya creada ✅)
- [ ] Visual Studio cerrado (si usas `dotnet run`)
- [ ] Backend corriendo en puerto 5192
- [ ] IP local encontrada con `ipconfig`
- [ ] `BASE_URL` actualizada en `RetrofitClient.kt`
- [ ] Celular en misma WiFi que PC
- [ ] Probaste la URL desde navegador del celular

---

## 🐛 Solución de Problemas

### Backend no arranca:
- Verifica que PostgreSQL esté corriendo
- Revisa la cadena de conexión en `appsettings.json`
- Verifica que el puerto 5192 no esté ocupado

### App móvil no conecta:
- Verifica que backend esté corriendo
- Confirma que celular y PC estén en misma WiFi
- Revisa que la IP en `RetrofitClient.kt` sea correcta
- Verifica Firewall de Windows (puede bloquear puerto 5192)

### Firewall bloqueando:
```powershell
# Permitir puerto 5192 entrante
New-NetFirewallRule -DisplayName "ASP.NET Backend API" -Direction Inbound -LocalPort 5192 -Protocol TCP -Action Allow
```

---

## 📚 Archivos Importantes Creados

✅ `wwwroot/` - Carpeta para archivos estáticos (PDFs, imágenes, etc.)
✅ `CONFIGURACION_RED.md` - Guía detallada de configuración de red móvil
✅ Este archivo - Guía de inicio del backend

---

## 🎯 Siguiente Paso

Una vez que el backend esté corriendo:

1. ✅ Verifica `http://localhost:5192/api/ConfigSla` en navegador
2. 📱 Actualiza IP en `RetrofitClient.kt`
3. 🚀 Ejecuta la app Android desde Android Studio
4. 🔐 Login con: `admin` / `admin`
5. 📊 Navega a Estadísticas y verifica que carga datos reales

¡Todo listo! 🎉
