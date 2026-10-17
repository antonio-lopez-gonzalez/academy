// ──────────────────────────────────────────────────────────────────────────
// Lección 03 — dotnet CLI, estructura de proyecto y VS Code
// ──────────────────────────────────────────────────────────────────────────
// En esta lección aprendemos a usar la CLI de .NET como un profesional,
// entendemos cada archivo de un proyecto .NET, y configuramos VS Code
// para tener autocompletado, depuración y análisis en tiempo real.
// ──────────────────────────────────────────────────────────────────────────

using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

// ══════════════════════════════════════════════════════════════════════════
// PARTE 1: LA CLI DE .NET — COMANDOS ESENCIALES
// ══════════════════════════════════════════════════════════════════════════
//
// `dotnet` es la navaja suiza de .NET. Desde una terminal puedes:
// - Crear proyectos
// - Compilar
// - Ejecutar
// - Testear
// - Publicar
// - Gestionar paquetes NuGet
// - Analizar rendimiento
//
// No necesitas Visual Studio para nada. Todo se hace desde CLI.
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("1. COMANDOS ESENCIALES DE dotnet CLI");

Console.WriteLine("""
    ┌──────────────────────────┬─────────────────────────────────────────┐
    │ Comando                  │ Qué hace                                │
    ├──────────────────────────┼─────────────────────────────────────────┤
    │ dotnet new list          │ Lista todas las plantillas disponibles  │
    │ dotnet new console       │ Crea un proyecto de consola             │
    │ dotnet new webapi        │ Crea un proyecto de API web             │
    │ dotnet new xunit         │ Crea un proyecto de tests (xUnit)       │
    │ dotnet new sln           │ Crea un archivo de solución (.sln)      │
    │ dotnet build             │ Compila el proyecto                     │
    │ dotnet run               │ Compila y ejecuta                       │
    │ dotnet test              │ Ejecuta los tests                       │
    │ dotnet publish           │ Publica para producción                 │
    │ dotnet add package       │ Añade un paquete NuGet                  │
    │ dotnet add reference     │ Añade referencia entre proyectos        │
    │ dotnet sln add           │ Añade un proyecto a la solución         │
    │ dotnet restore           │ Restaura paquetes NuGet                 │
    │ dotnet clean             │ Limpia archivos de build                │
    │ dotnet format            │ Formatea código según .editorconfig     │
    │ dotnet watch run         │ Ejecuta y reinicia al cambiar archivos  │
    │ dotnet ef migrations add │ Crea una migración de BD (EF Core)      │
    │ dotnet user-secrets set  │ Guarda secretos en desarrollo           │
    │ dotnet --version         │ Muestra la versión del SDK              │
    │ dotnet --list-sdks       │ Lista los SDKs instalados               │
    │ dotnet --list-runtimes   │ Lista los runtimes instalados           │
    └──────────────────────────┴─────────────────────────────────────────┘
    """);

// También podemos ver la versión desde código:
Version? sdkVersion = Environment.Version;
Console.WriteLine($"Versión del CLR en ejecución: {sdkVersion}");
Console.WriteLine($"Directorio actual: {Environment.CurrentDirectory}");

// ══════════════════════════════════════════════════════════════════════════
// PARTE 2: ANATOMÍA DE UN PROYECTO .NET
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("2. ESTRUCTURA DE UN PROYECTO .NET");

Console.WriteLine("""

    MiProyecto/
    ├── MiProyecto.csproj          ← Archivo de proyecto (MSBuild)
    ├── Program.cs                 ← Punto de entrada (top-level statements)
    ├── appsettings.json           ← Configuración (ASP.NET Core)
    ├── appsettings.Development.json
    ├── Properties/
    │   └── launchSettings.json    ← Perfiles de ejecución (desarrollo)
    ├── obj/                       ← Archivos temporales de build
    │   ├── project.assets.json    ← Grafo de dependencias NuGet
    │   └── *.cache, *.props, ...
    └── bin/
        └── Debug/
            └── net8.0/
                ├── MiProyecto.dll ← El assembly compilado (IL)
                ├── MiProyecto.exe ← El ejecutable (solo Windows)
                └── *.dll          ← Dependencias copiadas
    """);

// ══════════════════════════════════════════════════════════════════════════
// PARTE 3: EL ARCHIVO .csproj EXPLICADO
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("3. EL ARCHIVO .csproj — ANATOMÍA DETALLADA");

// Vamos a leer el .csproj de este mismo proyecto para analizarlo.
// El .csproj es un archivo XML que MSBuild usa para compilar.
// Es el equivalente a pom.xml (Maven), build.gradle (Gradle) o package.json (npm).

string rutaProyecto = Path.Combine(
    AppContext.BaseDirectory, "..", "..", "..", "DotNetCLI.csproj");

if (File.Exists(rutaProyecto))
{
  string contenidoCsproj = File.ReadAllText(rutaProyecto);
  Console.WriteLine("Contenido de DotNetCLI.csproj:");
  Console.WriteLine(new string('─', 50));
  Console.WriteLine(contenidoCsproj);
  Console.WriteLine(new string('─', 50));
}

// Explicación de cada parte del .csproj:
Console.WriteLine("""

    ANATOMÍA DE UN .csproj:

    1. <Project Sdk="Microsoft.NET.Sdk">
       └─ SDK: define el tipo de proyecto.
          - Microsoft.NET.Sdk → consola, librería, web...
          - Microsoft.NET.Sdk.Web → ASP.NET Core
          - Microsoft.NET.Sdk.Worker → Background Service

    2. <OutputType>Exe</OutputType>
       └─ Exe → ejecutable. Library → .dll (librería de clases).

    3. <TargetFramework>net8.0</TargetFramework>
       └─ El TFM (Target Framework Moniker).
          - net8.0 → .NET 8 (multiplataforma)
          - net48 → .NET Framework 4.8 (SOLO Windows, legacy)
          - net8.0-windows → .NET 8 con APIs específicas de Windows
          - net8.0-android → .NET 8 para Android (.NET MAUI)

    4. <Nullable>enable</Nullable>
       └─ Activa Nullable Reference Types. El compilador avisa de posibles null.
          Siempre debe estar enable en proyectos nuevos.

    5. <ImplicitUsings>enable</ImplicitUsings>
       └─ Importa automáticamente using comunes (System, System.Linq, etc.)
          Evita escribir 20 using al inicio de cada archivo.

    6. <PackageReference Include="..." Version="..." />
       └─ Dependencia de un paquete NuGet.

    7. <ProjectReference Include="..." />
       └─ Referencia a otro proyecto de la misma solución.
    """);

// ══════════════════════════════════════════════════════════════════════════
// PARTE 4: DEMOSTRACIÓN PRÁCTICA DE COMANDOS CLI
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("4. DEMOSTRACIÓN DE dotnet CLI DESDE CÓDIGO");

// Podemos ejecutar comandos dotnet desde C# con Process.Start.
// Esto es útil para scripts de automatización o CI/CD.
Console.WriteLine("Ejecutando 'dotnet --version' desde C#...");
string versionCLI = EjecutarDotNet("--version");
Console.WriteLine($"Resultado: dotnet --version → {versionCLI}");

Console.WriteLine("\nEjecutando 'dotnet --list-sdks' desde C#...");
string sdks = EjecutarDotNet("--list-sdks");
Console.WriteLine($"SDKs instalados:\n{sdks}");

// ══════════════════════════════════════════════════════════════════════════
// PARTE 5: PUBLICACIÓN Y DESPLIEGUE
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("5. COMANDOS DE PUBLICACIÓN (PUBLISH)");

Console.WriteLine("""

    TIPOS DE PUBLICACIÓN:

    1. Framework-Dependent (FDD):
       dotnet publish -c Release
       └─ Genera un .exe pequeño + .dll.
          Requiere que el runtime .NET esté instalado en el servidor.
          Tamaño: ~100 KB tu código + ~70 MB runtime (compartido).

    2. Self-Contained:
       dotnet publish -c Release --self-contained
       └─ Incluye el runtime .NET en el output.
          NO requiere instalar .NET en el servidor.
          Tamaño: ~70 MB (runtime incluido).

    3. Self-Contained + Trimmed:
       dotnet publish -c Release --self-contained -p:PublishTrimmed=true
       └─ Elimina código no usado del runtime.
          Tamaño: ~25 MB.

    4. NativeAOT (experimental):
       dotnet publish -c Release -p:PublishAot=true
       └─ Compila a código máquina nativo (sin JIT ni IL).
          Arranque instantáneo. Tamaño: ~5-10 MB.
          PERO: sin reflection dinámica ni Assembly.Load.

    Para producción en servidores con .NET instalado:
      dotnet publish -c Release -o ./publish

    Para contenedores Docker:
      dotnet publish -c Release -o ./publish
      # Luego copiar ./publish a la imagen de runtime
    """);

// ══════════════════════════════════════════════════════════════════════════
// PARTE 6: VISUAL STUDIO CODE — EXTENSIONES ESENCIALES
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("6. VS CODE — EXTENSIONES ESENCIALES");

Console.WriteLine("""
    ┌────────────────────────────────┬──────────────────────────────────┐
    │ Extensión                      │ ID                               │
    ├────────────────────────────────┼──────────────────────────────────┤
    │ C# Dev Kit (Obligatoria)       │ ms-dotnettools.csdevkit          │
    │  └─ Incluye: IntelliSense,     │                                   │
    │     navegación, refactorización│                                   │
    │     testing, debugging         │                                   │
    ├────────────────────────────────┼──────────────────────────────────┤
    │ NuGet Gallery                  │ patcx.vscode-nuget-gallery       │
    │  └─ Buscar/instalar paquetes   │                                   │
    │     NuGet desde la UI          │                                   │
    ├────────────────────────────────┼──────────────────────────────────┤
    │ .NET Core Test Explorer        │ formulahendry.dotnet-test-explorer│
    │  └─ Árbol visual de tests      │                                   │
    ├────────────────────────────────┼──────────────────────────────────┤
    │ GitLens                        │ eamodio.gitlens                  │
    │  └─ Git avanzado en VS Code    │                                   │
    ├────────────────────────────────┼──────────────────────────────────┤
    │ EditorConfig for VS Code       │ editorconfig.editorconfig        │
    │  └─ Soporte para .editorconfig  │                                   │
    ├────────────────────────────────┼──────────────────────────────────┤
    │ REST Client                    │ humao.rest-client                │
    │  └─ Probar APIs desde archivos │                                   │
    │     .http en VS Code           │                                   │
    └────────────────────────────────┴──────────────────────────────────┘

    Instalación rápida desde terminal:
      code --install-extension ms-dotnettools.csdevkit
      code --install-extension patcx.vscode-nuget-gallery
    """);

// ══════════════════════════════════════════════════════════════════════════
// PARTE 7: CONFIGURACIÓN DE VS CODE PARA .NET
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("7. CONFIGURACIÓN DE VS CODE (settings.json)");

// Leemos y mostramos la configuración recomendada de VS Code
Console.WriteLine("""
    // Archivo .vscode/settings.json (configuración recomendada):
    {
      "editor.formatOnSave": true,
      "[csharp]": {
        "editor.defaultFormatter": "ms-dotnettools.csharp",
        "editor.codeActionsOnSave": {
          "source.fixAll": "explicit",
          "source.organizeUsings": "explicit"
        }
      },
      "dotnet.defaultSolution": "curso-dotnet.sln",
      "csharp.suppressDotnetInstallWarning": true,
      "omnisharp.enableRoslynAnalyzers": true
    }

    // Archivo .vscode/launch.json (configuración de debugging):
    {
      "version": "0.2.0",
      "configurations": [
        {
          "name": ".NET Launch (console)",
          "type": "coreclr",
          "request": "launch",
          "preLaunchTask": "build",
          "program": "${workspaceFolder}/bin/Debug/net8.0/MiProyecto.dll",
          "args": [],
          "cwd": "${workspaceFolder}",
          "console": "internalConsole",
          "stopAtEntry": false
        }
      ]
    }

    // Archivo .vscode/tasks.json (tareas de build automático):
    {
      "version": "2.0.0",
      "tasks": [
        {
          "label": "build",
          "command": "dotnet",
          "type": "process",
          "args": ["build", "${workspaceFolder}/MiProyecto.csproj"],
          "problemMatcher": "$msCompile"
        }
      ]
    }
    """);

// ══════════════════════════════════════════════════════════════════════════
// PARTE 8: DEMO DE JSON CONFIGURATION
// ══════════════════════════════════════════════════════════════════════════

ImprimirSeccion("8. CONFIGURACIÓN CON JSON (System.Text.Json)");

// Demostración de cómo leer configuración JSON con System.Text.Json.
// Esto es lo que hace ASP.NET Core internamente con appsettings.json.
// Usamos JsonSerializer en lugar de leer manualmente el archivo.

string configJson = """
    {
      "AppName": "Mi Aplicación .NET 8",
      "Version": "1.0.0",
      "Logging": {
        "Level": "Information",
        "OutputPath": "logs/app.log"
      },
      "Database": {
        "Provider": "PostgreSQL",
        "ConnectionString": "Host=localhost;Database=cms;Username=admin"
      }
    }
    """;

// Deserializar JSON a objetos tipados
Configuracion? config = JsonSerializer.Deserialize<Configuracion>(configJson);

if (config is not null)
{
  Console.WriteLine($"Nombre de la app : {config.AppName}");
  Console.WriteLine($"Versión          : {config.Version}");
  Console.WriteLine($"Log Level        : {config.Logging.Level}");
  Console.WriteLine($"DB Provider      : {config.Database.Provider}");

  // También podemos volver a serializar con formato indentado
  string jsonBonito = JsonSerializer.Serialize(
      config, new JsonSerializerOptions { WriteIndented = true });
  Console.WriteLine($"\nJSON indentado:\n{jsonBonito}");
}

// ══════════════════════════════════════════════════════════════════════════
// FIN DEL PROGRAMA
// ══════════════════════════════════════════════════════════════════════════

Console.WriteLine();
Console.WriteLine("✅ Fin de la Lección 03.");
Console.WriteLine("   Ahora sabes usar dotnet CLI como un profesional.");
Console.WriteLine("   El siguiente paso: crear el proyecto CMS desde cero.");

// ──────────────────────────────────────────────────────────────────────────
// Métodos auxiliares
// ──────────────────────────────────────────────────────────────────────────

/// <summary>
/// Ejecuta un comando dotnet y devuelve su salida estándar.
/// Usa Process.Start con redirección de streams.
/// </summary>
static string EjecutarDotNet(string argumentos)
{
  // Configuramos el proceso para capturar la salida
  ProcessStartInfo psi = new("dotnet", argumentos)
  {
    RedirectStandardOutput = true,   // Capturamos stdout
    RedirectStandardError = true,    // Capturamos stderr
    UseShellExecute = false,         // Necesario para redirigir
    CreateNoWindow = true            // Sin ventana de consola extra
  };

  using Process? proceso = Process.Start(psi);
  if (proceso is null)
  {
    return "Error: No se pudo iniciar el proceso dotnet";
  }

  // Leemos toda la salida (síncrono para simplicidad)
  string salida = proceso.StandardOutput.ReadToEnd();
  string errores = proceso.StandardError.ReadToEnd();

  proceso.WaitForExit();

  if (proceso.ExitCode != 0)
  {
    return $"Error (código {proceso.ExitCode}): {errores}";
  }

  return salida.Trim();
}

/// <summary>
/// Imprime una sección con formato visual consistente.
/// </summary>
static void ImprimirSeccion(string titulo)
{
  Console.WriteLine();
  Console.WriteLine(new string('═', 65));
  Console.WriteLine($"  {titulo}");
  Console.WriteLine(new string('═', 65));
}

// ──────────────────────────────────────────────────────────────────────────
// Tipos para la demo de JSON
// ──────────────────────────────────────────────────────────────────────────

/// <summary>
/// Representa la configuración de la aplicación (simula appsettings.json).
/// El atributo JsonPropertyName permite mapear propiedades con nombres
/// distintos entre JSON y C#.
/// </summary>
internal sealed class Configuracion
{
  [JsonPropertyName("AppName")]
  public string AppName { get; set; } = string.Empty;

  [JsonPropertyName("Version")]
  public string Version { get; set; } = string.Empty;

  [JsonPropertyName("Logging")]
  public LoggingConfig Logging { get; set; } = new();

  [JsonPropertyName("Database")]
  public DatabaseConfig Database { get; set; } = new();
}

internal sealed class LoggingConfig
{
  [JsonPropertyName("Level")]
  public string Level { get; set; } = "Information";

  [JsonPropertyName("OutputPath")]
  public string OutputPath { get; set; } = string.Empty;
}

internal sealed class DatabaseConfig
{
  [JsonPropertyName("Provider")]
  public string Provider { get; set; } = string.Empty;

  [JsonPropertyName("ConnectionString")]
  public string ConnectionString { get; set; } = string.Empty;
}
