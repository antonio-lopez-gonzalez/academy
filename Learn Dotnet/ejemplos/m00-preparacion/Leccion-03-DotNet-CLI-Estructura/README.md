# Lección 03 — dotnet CLI, estructura de proyecto y VS Code

---

## 1. Introducción

Vamos a dominar las herramientas que usarás **todos los días** como desarrollador .NET: la CLI (`dotnet`), la estructura de un proyecto y Visual Studio Code.

- **`dotnet` CLI**: crear, compilar, ejecutar, testear y publicar proyectos sin tocar un IDE.
- **Estructura de proyecto**: qué es cada archivo (`.csproj`, `Program.cs`, `appsettings.json`...).
- **VS Code**: extensiones esenciales, configuración de debugging y atajos de teclado.
- **Errores típicos**: no saber qué versión del SDK usar, confundir `net8.0` con `net48`, instalar extensiones incorrectas.

---

## 2. Teoría

### 2.1 La CLI de .NET

`dotnet` es el punto de entrada universal. Todos los comandos siguen el patrón:

```
dotnet [comando] [argumentos] [opciones]
```

**Comandos principales:**

| Comando | Descripción |
|---|---|
| `dotnet new` | Crear proyectos desde plantillas |
| `dotnet build` | Compilar (C# → IL) |
| `dotnet run` | Compilar + ejecutar |
| `dotnet test` | Ejecutar tests |
| `dotnet publish` | Publicar para producción |
| `dotnet restore` | Restaurar paquetes NuGet |
| `dotnet add package` | Añadir dependencia NuGet |
| `dotnet add reference` | Referenciar otro proyecto |
| `dotnet clean` | Limpiar bin/obj |
| `dotnet format` | Formatear código |
| `dotnet watch` | Hot reload en desarrollo |

### 2.2 Anatomía de un proyecto .NET

```
MiProyecto/
├── MiProyecto.csproj       ← Manifiesto del proyecto (MSBuild)
├── Program.cs              ← Punto de entrada
├── appsettings.json        ← Configuración (ASP.NET Core)
├── Properties/
│   └── launchSettings.json ← Perfiles de debug
├── obj/                    ← Archivos temporales de build
└── bin/
    └── Debug/net8.0/       ← Output compilado (.dll con IL)
```

### 2.3 El archivo .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

- `Sdk`: tipo de proyecto (consola, web, worker, librería)
- `TargetFramework`: `net8.0` (.NET 8 multiplataforma), `net48` (.NET Framework 4.8 Windows)
- `Nullable`: activa análisis de nullabilidad
- `ImplicitUsings`: importa `System`, `System.Linq`, etc. automáticamente

### 2.4 VS Code para .NET

Extensiones esenciales:
- **C# Dev Kit** (`ms-dotnettools.csdevkit`): IntelliSense, navegación, testing, debugging
- **NuGet Gallery** (`patcx.vscode-nuget-gallery`): gestionar paquetes
- **EditorConfig** (`editorconfig.editorconfig`): estilos de código consistentes

---

## 3. Ejemplo mínimo

```bash
# Crear, compilar y ejecutar en 3 comandos
dotnet new console -n HolaMundo -o ./HolaMundo
dotnet run --project ./HolaMundo
```

---

## 4. Ejemplo real

El proyecto `DotNetCLI` incluido demuestra:
- Ejecutar comandos `dotnet` desde C# con `Process.Start`
- Leer el propio `.csproj` para analizar su estructura
- Serializar/deserializar JSON con `System.Text.Json`
- Tablas de referencia de comandos, estructura y extensiones

```bash
dotnet run --project ejemplos/m00-preparacion/Leccion-03-DotNet-CLI-Estructura/src/DotNetCLI
```

---

## 5. Buenas prácticas

| ✅ Hacer | ❌ Evitar |
|---|---|
| Usar `dotnet` CLI en CI/CD | Depender de Visual Studio GUI para builds |
| `.csproj` siempre con `<Nullable>enable</Nullable>` | Dejar `<Nullable>disable</Nullable>` |
| `dotnet format` antes de commit | Código sin formatear |
| `dotnet watch run` en desarrollo | Parar/iniciar manualmente tras cada cambio |
| `global.json` para fijar versión del SDK | Usar "la que haya instalada" sin control |

---

## 6. Errores habituales

**Error: confundir `net8.0` con `net48`**

```xml
<!-- ❌ INCORRECTO: .NET Framework 4.8 (solo Windows, legacy) -->
<TargetFramework>net48</TargetFramework>

<!-- ✅ CORRECTO: .NET 8 (multiplataforma, moderno) -->
<TargetFramework>net8.0</TargetFramework>
```

**Error: no restaurar paquetes tras clonar**

```bash
# ❌ dotnet build sin restore previo puede fallar
# ✅ Siempre:
dotnet restore
dotnet build
```

---

## 7. Ejercicio guiado

1. Crea un proyecto de consola: `dotnet new console -n MiApp -o ./MiApp`
2. Añade el paquete `Newtonsoft.Json` (solo para practicar — en el curso usamos System.Text.Json)
3. Añade una referencia a otro proyecto (crea una librería con `dotnet new classlib`)
4. Ejecuta los tests: `dotnet test`
5. Publica en modo Release: `dotnet publish -c Release -o ./publicacion`
6. Inspecciona el output en `./publicacion`

---

## 8. Reto

Crea un script (`.ps1` o `.sh`) que automatice:
1. `dotnet new webapi -n MiApi`
2. `dotnet new xunit -n MiApi.Tests`
3. `dotnet add MiApi.Tests reference MiApi`
4. Crea un `.sln` que contenga ambos proyectos
5. Compila y ejecuta los tests

---

## 9. Resumen

- `dotnet` CLI es la herramienta universal: `new`, `build`, `run`, `test`, `publish`.
- `.csproj` define el proyecto: SDK, TargetFramework, Nullable, paquetes, referencias.
- VS Code con C# Dev Kit es la configuración recomendada.
- `global.json` fija la versión del SDK para todo el equipo.
- Publicación: Framework-Dependent (ligera) vs Self-Contained (autónoma) vs NativeAOT (rápida).

---

## 10. Preguntas frecuentes

### ¿Necesito Visual Studio (el IDE grande)?

No. VS Code + `dotnet` CLI cubren el 100% de los flujos de trabajo. Visual Studio es útil si trabajas con WinForms, WPF o necesitas diseñadores visuales.

### ¿Qué hace `dotnet restore` exactamente?

Lee los `<PackageReference>` del `.csproj`, descarga los paquetes de NuGet.org (o tu feed privado) a la caché local (`~/.nuget/packages/`), y resuelve el grafo de dependencias. `dotnet build` y `dotnet run` lo ejecutan implícitamente, por lo que rara vez necesitas llamarlo manualmente.

### ¿Puedo tener múltiples versiones del SDK instaladas?

Sí. `dotnet --list-sdks` muestra todas. Usa `global.json` para que cada proyecto sepa qué versión usar:

```json
{
  "sdk": {
    "version": "8.0.423",
    "rollForward": "latestFeature"
  }
}
```

---

**Siguiente:** Setup inicial del CMS
