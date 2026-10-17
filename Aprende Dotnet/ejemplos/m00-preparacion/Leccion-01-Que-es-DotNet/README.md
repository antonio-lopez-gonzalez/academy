# Lección 01 — ¿Qué es .NET? Historia y ecosistema

---

## 1. Introducción

En esta primera lección vamos a entender **qué es realmente .NET** más allá de la definición de Wikipedia.

Vamos a responder:

- **¿Qué es .NET?** No es solo C#, no es solo un framework web. Es una plataforma de desarrollo completa.
- **¿Por qué existe?** Microsoft necesitaba competir con Java (JVM) a finales de los 90.
- **¿Qué problema resuelve?** Escribir código una vez y ejecutarlo en cualquier sistema operativo, con alto rendimiento y seguridad de tipos.
- **¿Cuándo se utiliza?** Backend web, APIs, microservicios, cloud, escritorio, videojuegos, IoT, IA/ML.
- **Errores típicos:** Confundir .NET Framework (el viejo, solo Windows) con .NET moderno (multiplataforma). Pensar que C# es lento. Creer que .NET solo sirve para Windows.

> **Objetivo:** Al terminar esta lección, sabrás exactamente qué es .NET 8, de dónde viene, y por qué deberías considerarlo para tu próximo proyecto.

---

## 2. Teoría

### 2.1 ¿Qué es .NET?

.NET es una **plataforma de desarrollo de software** creada por Microsoft. Tiene tres componentes principales:

```
┌─────────────────────────────────────────────────────┐
│                    .NET 8                           │
├─────────────────────────────────────────────────────┤
│  CLR (Common Language Runtime)                      │
│  ─ El motor de ejecución. Gestiona la memoria (GC), │
│    la seguridad de tipos, las excepciones y los      │
│    hilos. Equivalente a la JVM de Java.              │
├─────────────────────────────────────────────────────┤
│  BCL (Base Class Library)                           │
│  ─ La biblioteca estándar. Contiene miles de clases  │
│    para trabajar con strings, colecciones, archivos, │
│    red, fechas, cifrado, compresión, HTTP, JSON...   │
│    Equivalente a la JDK de Java o la stdlib de Python│
├─────────────────────────────────────────────────────┤
│  SDK + Tooling                                      │
│  ─ Compilador Roslyn (C# → IL), dotnet CLI,         │
│    MSBuild, NuGet (gestor de paquetes).              │
│    Equivalente a javac + Maven/Gradle en Java.       │
└─────────────────────────────────────────────────────┘
```

### 2.2 La historia: de .NET Framework a .NET 8

#### .NET Framework (2002-2019)

- **Lanzado en 2002** junto con C# 1.0 y Visual Studio .NET.
- **Solo Windows.** Instalado como parte del sistema operativo.
- **Monolítico.** Todo venía en un solo paquete gigante (cientos de MB).
- **Propietario.** Código cerrado. Solo Microsoft podía arreglar bugs.
- **Versiones:** 1.0, 1.1, 2.0, 3.0, 3.5, 4.0, 4.5, 4.6, 4.7, 4.8 (la última).
- **Problema:** Si una aplicación necesitaba una versión específica de una librería, y otra aplicación necesitaba otra versión, podían surgir conflictos (el "DLL Hell").

#### .NET Core (2016-2019)

- **Reinicio desde cero.** Microsoft reescribió .NET para ser:
  - **Open source** (licencia MIT en GitHub: <https://github.com/dotnet>)
  - **Multiplataforma** (Windows, Linux, macOS, ARM32/64)
  - **Modular** (instalas solo lo que necesitas vía NuGet)
  - **Side-by-side** (múltiples versiones instalables sin conflictos)
  - **Rendimiento superior** (ASP.NET Core está en el top 10 de TechEmpower benchmarks)
- **Versiones:** 1.0, 1.1, 2.0, 2.1, 2.2, 3.0, 3.1 (LTS).
- **Problema:** Había dos .NET: .NET Framework (Windows) y .NET Core (multiplataforma). Esto confundía a todo el mundo.

#### .NET 5/6/7/8 (2020-presente)

- **Unificación.** A partir de .NET 5, solo hay UN .NET:
  - `net5.0`, `net6.0`, `net7.0`, `net8.0` (Target Framework Monikers).
  - Se saltaron el "4" para evitar confusión con .NET Framework 4.x.
- **LTS vs STS:**
  - **LTS (Long Term Support):** 3 años de soporte. Versiones pares: .NET 6, .NET 8, .NET 10.
  - **STS (Standard Term Support):** 18 meses de soporte. Versiones impares: .NET 5, .NET 7, .NET 9.
- **Cada año** sale una versión nueva en noviembre.

```
2002 ──── .NET Framework 1.0 ──── Windows-only, propietario
 │
2016 ──── .NET Core 1.0     ──── Open source, multiplataforma
 │
2020 ──── .NET 5            ──── Unificación de .NET Core + Mono + Xamarin
2021 ──── .NET 6 (LTS)      ──── C# 10, Minimal APIs, File-scoped namespaces
2022 ──── .NET 7            ──── C# 11, NativeAOT experimental, mejoras GC
2023 ──── .NET 8 (LTS)      ──── C# 12, NativeAOT maduro, Primary constructors
2024 ──── .NET 9 (STS)      ──── C# 13
2025 ──── .NET 10 (LTS)     ──── Próximo LTS
```

### 2.3 El ecosistema hoy

```
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│  ASP.NET    │  │  .NET MAUI  │  │  Blazor     │
│  (web/api)  │  │  (móvil)    │  │  (web UI)   │
└──────┬──────┘  └──────┬──────┘  └──────┬──────┘
       │                │                │
       └────────────────┼────────────────┘
                        │
               ┌────────┴────────┐
               │    .NET 8       │
               │  (plataforma)   │
               └────────┬────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
  ┌─────┴─────┐   ┌─────┴─────┐   ┌─────┴─────┐
  │ Windows   │   │  Linux    │   │  macOS    │
  └───────────┘   └───────────┘   └───────────┘
```

- **ASP.NET Core**: framework web para APIs y aplicaciones web.
- **.NET MAUI**: interfaz de usuario multiplataforma (sucesor de Xamarin).
- **Blazor**: construir UIs web interactivas con C# en lugar de JavaScript (WebAssembly).
- **Entity Framework Core**: ORM para bases de datos.
- **ML.NET**: machine learning en .NET.
- **Unity / Godot**: motores de videojuegos que usan C#.

### 2.4 Comparación con Java

| Característica | .NET (C#) | Java |
|---|---|---|
| **Runtime** | CLR | JVM |
| **Bytecode** | CIL (MSIL) | Java Bytecode |
| **Bibl. estándar** | BCL (System.*) | JDK (java.*, javax.*) |
| **Paquetes** | NuGet | Maven Central |
| **Build** | MSBuild / `dotnet build` | Maven / Gradle |
| **GC** | Generacional + compactación | G1, ZGC, Shenandoah |
| **Value types** | Sí (`struct`, stack) | No (Proyecto Valhalla, aún no lanzado) |
| **Propiedades** | Sí (`get; set;`) | No (getters/setters manuales) |
| **LINQ** | Sí (integrado en lenguaje) | Streams API (similar, menos flexible) |
| **async/await** | 2012 (C# 5) | 2023 (Project Loom, Java 21) |
| **Nullable ref** | Sí (C# 8+, compilador) | Limitado (@Nullable, opcional) |
| **Extension methods** | Sí (C# 3+) | No |
| **AOT compilation** | Sí (NativeAOT) | Sí (GraalVM Native Image) |

**Veredicto:** .NET ofrece una experiencia de desarrollo más moderna y productiva. Java tiene un ecosistema más maduro en big data (Hadoop, Spark) y Android.

### 2.5 Comparación con Node.js / Python

| Característica | .NET (C#) | Node.js | Python |
|---|---|---|---|
| **Tipado** | Estático y fuerte | Dinámico (TS opcional) | Dinámico (type hints opcionales) |
| **Rendimiento CPU** | Muy alto (JIT+AOT) | Medio (V8 JIT) | Bajo (interpretado) |
| **Concurrencia** | Multi-hilo real | Single thread + event loop | GIL (Global Interpreter Lock) |
| **Ecosistema** | Unificado (Microsoft) | Fragmentado (1000 tools) | Fragmentado (pip, poetry, venv) |
| **Startup time** | Lento (JIT) / Rápido (AOT) | Instantáneo | Instantáneo |
| **Cold start** | ~300ms-1s | ~50ms | ~100ms |
| **Peso en memoria** | ~30-50 MB base | ~20-40 MB base | ~15-25 MB base |
| **Ideal para** | APIs, microservicios, enterprise | I/O intensivo, real-time | IA/ML, scripting, data science |

**Veredicto:** .NET brilla en backend empresarial y sistemas de alto rendimiento. Node.js es mejor para prototipado rápido y aplicaciones I/O-bound. Python domina el ecosistema de IA/ML.

---

## 3. Ejemplo mínimo

El ejemplo más pequeño de .NET es un "Hello World":

```csharp
// Todo programa C# comienza con la ejecución del método Main
// (en .NET 6+, top-level statements ocultan Main automáticamente)
Console.WriteLine("¡Hola, .NET 8!");
```

Ejecución:

```bash
dotnet run
```

**Explicación línea por línea:**

- `Console` — clase de la BCL en el namespace `System`. Proporciona entrada/salida por consola.
- `WriteLine` — método estático que escribe una línea y añade salto de línea (`\n`).
- `"¡Hola, .NET 8!"` — string literal. Las comillas dobles en C# siempre son `string`. Para `char` se usan comillas simples.
- `;` — toda sentencia en C# termina con punto y coma (como Java, distinto de Python y JavaScript moderno).

---

## 4. Ejemplo real

El proyecto `IntroduccionDotNet` incluido en esta lección demuestra:

1. **Información del entorno .NET**: versión del runtime, sistema operativo, arquitectura.
2. **Uso de la BCL**: `ReadOnlySpan<char>`, `TimeProvider`, `Path.Combine`, `Guid`.
3. **LINQ**: consultas con query syntax y method syntax sobre colecciones.
4. **Tipado estático**: cómo `var` infiere tipos en compilación (no es como JS).
5. **Nullable Reference Types**: cómo el compilador previene `NullReferenceException`.
6. **Línea de tiempo**: historia de .NET en formato visual.

Para ejecutarlo:

```bash
# Desde la raíz del repositorio
dotnet run --project ejemplos/m00-preparacion/Leccion-01-Que-es-DotNet/src/IntroduccionDotNet
```

**Decisiones de diseño:**

- **Todo en un solo archivo `Program.cs`** — para que sea fácil de leer y ejecutar sin navegar entre múltiples archivos.
- **Top-level statements** — reducimos boilerplate innecesario en una lección introductoria.
- **Comentarios extensos en español** — cada línea explica el "por qué", no solo el "qué".
- **Uso de `ReadOnlySpan<char>` y `TimeProvider`** — mostramos características modernas desde el día 1, para que el alumno vea que .NET es moderno.

---

## 5. Buenas prácticas

### ✅ Hacer

| Práctica | Por qué |
|---|---|
| Usar siempre .NET 8+ (no .NET Framework) | Multiplataforma, más rápido, soporte activo |
| Preferir LTS para producción | 3 años de soporte y parches de seguridad |
| Usar el SDK más reciente con `global.json` | Fijas la versión exacta que usa tu equipo |
| Activar Nullable Reference Types (`<Nullable>enable</Nullable>`) | Previene `NullReferenceException` en compilación |
| Usar `dotnet` CLI en lugar de Visual Studio para CI/CD | Scriptable, portable, sin GUI |
| Nombrar proyectos con `TargetFramework` claro: `net8.0` | Evita ambigüedad con .NET Framework 4.x |

### ❌ Evitar

| Práctica | Por qué |
|---|---|
| `new HttpClient()` | Agota los sockets. Usar `IHttpClientFactory` |
| `Console.WriteLine` para logging en producción | No estructurado, no filtrable. Usar `ILogger<T>` |
| Usar .NET Framework para nuevos proyectos | Obsoleto, solo recibe parches de seguridad |
| Ignorar los warnings de nullabilidad | Se acumulan y esconden bugs reales |
| Instalar el runtime en modo "Global" en Windows | Mejor side-by-side con SDK |
| Mezclar librerías de .NET Framework con .NET moderno | No son compatibles binariamente |

---

## 6. Errores habituales

### Error 1: "Voy a usar .NET Framework porque es lo que conozco"

```csharp
// ❌ INCORRECTO: usando .NET Framework 4.8 en 2026
// - Solo compila en Windows
// - No tiene C# 12, Span<T>, NativeAOT...
// - Librerías abandonadas o congeladas
```

```xml
<!-- ✅ CORRECTO: .csproj moderno para .NET 8 -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

### Error 2: Creer que `var` es como JavaScript

```csharp
// ❌ INCORRECTO: pensar que var es dinámico
var x = 10;
x = "hola"; // Error CS0029: Cannot implicitly convert type 'string' to 'int'

// ✅ CORRECTO: var infiere el tipo en compilación. Es azúcar sintáctico.
// Es equivalente a escribir:
int x = 10;
```

### Error 3: No activar Nullable Reference Types

```csharp
// ❌ INCORRECTO: proyecto sin Nullable (o con <Nullable>disable</Nullable>)
string nombre = null; // El compilador no avisa...
int longitud = nombre.Length; // ¡NullReferenceException en producción!

// ✅ CORRECTO: <Nullable>enable</Nullable>
string? nombre = null; // Declaras explícitamente que puede ser null
int longitud = nombre?.Length ?? 0; // Manejas el caso null
```

---

## 7. Ejercicio guiado

### Ejercicio: Explorador de la BCL

Crea un programa que:

1. Muestre la versión de .NET en ejecución.
2. Liste los archivos `.csproj` en tu carpeta actual (usa `Directory.GetFiles`).
3. Genere un GUID y muestre cuántos caracteres tiene.
4. Use `Path.GetTempPath()` para mostrar la ruta temporal del sistema.
5. Convierta un número `double` a `string` con 2 decimales (usa `ToString("F2")`).

**Solución paso a paso:**

```csharp
// Paso 1: Mostrar versión del runtime
// Environment.Version devuelve la versión del CLR
Console.WriteLine($"Runtime: {Environment.Version}");

// Paso 2: Listar archivos .csproj en el directorio actual
// Directory.GetCurrentDirectory() devuelve el directorio de trabajo
// Directory.GetFiles busca archivos con un patrón
string directorioActual = Directory.GetCurrentDirectory();
string[] archivosCsproj = Directory.GetFiles(directorioActual, "*.csproj");
Console.WriteLine($"\nArchivos .csproj en {directorioActual}:");
foreach (string archivo in archivosCsproj)
{
    // Path.GetFileName extrae solo el nombre del archivo (sin la ruta)
    Console.WriteLine($"  - {Path.GetFileName(archivo)}");
}

// Paso 3: Generar GUID y mostrar su longitud
// Guid.NewGuid() genera un identificador único usando algoritmo v4 (aleatorio)
Guid id = Guid.NewGuid();
string idTexto = id.ToString();
Console.WriteLine($"\nGUID: {idTexto}");
Console.WriteLine($"Longitud del GUID: {idTexto.Length} caracteres");

// Paso 4: Ruta temporal del sistema
// Path.GetTempPath() devuelve la ruta que usa el SO para archivos temporales
// Funciona en Windows (C:\Users\X\AppData\Local\Temp\), Linux (/tmp/) y macOS
Console.WriteLine($"\nRuta temporal del sistema: {Path.GetTempPath()}");

// Paso 5: Formatear número decimal
// ToString("F2") formatea con 2 decimales fijos (ej: 3.14, 100.00)
// "F" = Fixed-point, el número indica los decimales
double numero = 1234.5678;
Console.WriteLine($"\nNúmero original: {numero}");
Console.WriteLine($"Formateado (F2): {numero.ToString("F2")}");
```

---

## 8. Reto

**Reto: Descubriendo el Garbage Collector**

Investiga y responde:

1. ¿Qué es el Garbage Collector (GC) y por qué existe?
2. .NET usa un GC **generacional**. ¿Qué significa que tenga 3 generaciones (Gen0, Gen1, Gen2)?
3. ¿Qué es el **LOH** (Large Object Heap) y por qué se trata diferente?
4. ¿Cómo afecta el GC al rendimiento de una API con alta concurrencia?
5. ¿Por qué `struct` puede reducir la presión del GC frente a `class`?

> **Pista:** No tienes que programar nada todavía. Investiga la documentación oficial: <https://Aprende.microsoft.com/en-us/dotnet/standard/garbage-collection/>

---

## 9. Resumen

En esta lección hemos aprendido:

- **.NET** es una plataforma de desarrollo con 3 componentes: CLR (runtime), BCL (biblioteca estándar) y SDK (herramientas).
- **Historia:** .NET Framework (2002, solo Windows) → .NET Core (2016, open source, multiplataforma) → .NET 5+ (2020, unificado).
- **.NET 8 es LTS** (noviembre 2023), con C# 12 y soporte hasta noviembre 2026.
- **Frente a Java:** .NET ofrece tipos de valor (`struct`), LINQ, properties, async/await más maduro. Java lidera en big data y Android.
- **Frente a Node.js/Python:** .NET gana en rendimiento CPU, tipado estático y concurrencia multi-hilo. Ellos ganan en ecosistema de IA (Python) y prototipado rápido (Node).
- **Nullable Reference Types** y **tipado estático fuerte** previenen categorías enteras de bugs en compilación.
- **La BCL** contiene todo lo necesario: strings, colecciones, fechas, archivos, red, HTTP, JSON, cifrado, compresión... sin depender de 1000 paquetes npm.

**Conceptos clave:** CLR, BCL, IL, JIT, GC, LTS vs STS, NRT (Nullable Reference Types), multiplataforma, open source.

---

## 10. Preguntas frecuentes

### ¿Necesito Windows para programar en .NET?

**No.** .NET 8 funciona en Windows, Linux y macOS. Puedes desarrollar con VS Code, Rider o Visual Studio. La CLI (`dotnet`) es idéntica en todas las plataformas.

### ¿C# es propiedad de Microsoft?

**No.** C# es un estándar ECMA (ECMA-334) e ISO (ISO/IEC 23270). El compilador Roslyn es open source (Apache 2.0). Cualquiera puede implementar C# — existen compiladores alternativos como Mono.

### ¿Puedo usar .NET para frontend web?

**Sí**, con **Blazor**. Blazor ejecuta C# en el navegador compilado a WebAssembly. En este curso nos centramos en backend (APIs), no en Blazor.

### ¿.NET es más rápido que Java?

Depende del benchmark y el escenario. En el TechEmpower Fortunes benchmark, ASP.NET Core está consistentemente en el top 10 (a veces top 5), por delante de Spring Boot. En computación numérica, .NET con `Span<T>` y SIMD tiene ventaja. En GC pause times, el ZGC de Java es excelente. Ambos son de clase mundial.

### ¿Qué es AOT (Ahead-of-Time compilation)?

Normalmente .NET compila C# → IL (bytecode) → JIT (código máquina en ejecución). Con **NativeAOT** (maduro en .NET 8), compilas directamente C# → código máquina nativo. Ventajas: arranque instantáneo, menos memoria. Desventajas: compilación más lenta, sin reflection en runtime.

### ¿Necesito saber C# para usar .NET?

No obligatoriamente. .NET soporta F# y VB.NET de forma nativa, y hay bindings para otros lenguajes. Pero el 99% del ecosistema (tutoriales, librerías, documentación) está en C#. Este curso asume C# como lenguaje principal.

---

**Siguiente lección:** CLR, BCL y cómo funciona la compilación (C# → IL → JIT → máquina)
