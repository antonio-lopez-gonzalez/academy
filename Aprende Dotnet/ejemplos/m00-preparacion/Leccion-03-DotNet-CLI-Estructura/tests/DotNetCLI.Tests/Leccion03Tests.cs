using FluentAssertions;
using System.Text.Json;

namespace DotNetCLI.Tests;

public class Leccion03_DotNetCLI_Estructura
{
    [Fact]
    public void EnvironmentVersion_DebeSerNet8OSuperior()
    {
        Version version = Environment.Version;
        version.Major.Should().BeGreaterOrEqualTo(8);
    }

    [Fact]
    public void CurrentDirectory_DebeSerValida()
    {
        string dir = Environment.CurrentDirectory;
        Directory.Exists(dir).Should().BeTrue();
    }

    [Fact]
    public void JsonSerializer_Deserialize_DebeMapearCorrectamente()
    {
        string json = """
            {
                "AppName": "Test App",
                "Version": "2.0.0",
                "Logging": {
                    "Level": "Debug",
                    "OutputPath": "logs/"
                },
                "Database": {
                    "Provider": "PostgreSQL",
                    "ConnectionString": "Host=localhost"
                }
            }
            """;

        ConfigTest? config = JsonSerializer.Deserialize<ConfigTest>(json);

        config.Should().NotBeNull();
        config!.AppName.Should().Be("Test App");
        config.Version.Should().Be("2.0.0");
        config.Logging.Level.Should().Be("Debug");
        config.Database.Provider.Should().Be("PostgreSQL");
    }

    [Fact]
    public void JsonSerializer_Serialize_DebeGenerarJsonValido()
    {
        ConfigTest config = new()
        {
            AppName = "MiApp",
            Version = "1.0",
            Logging = new() { Level = "Warning", OutputPath = "out/" },
            Database = new() { Provider = "SQLite", ConnectionString = "DataSource=:memory:" }
        };

        string json = JsonSerializer.Serialize(config);

        json.Should().Contain("\"AppName\"");
        json.Should().Contain("MiApp");
        json.Should().Contain("\"Provider\"");
        json.Should().Contain("SQLite");
    }

    [Fact]
    public void JsonSerializerOptions_WriteIndented_DebeFormatearBonito()
    {
        var obj = new { Nombre = "Test", Valor = 42 };

        string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });

        json.Should().Contain("\n");
        json.Should().Contain("  ");
    }

    [Fact]
    public void PathCombine_DebeUnirRutas()
    {
        string ruta = Path.Combine("proyecto", "src", "Program.cs");

        ruta.Should().EndWith("Program.cs");
        ruta.Should().Contain("proyecto");
        ruta.Should().Contain("src");
    }

    [Fact]
    public void DirectoryGetCurrentDirectory_DebeDevolverDirectorioValido()
    {
        string dir = Directory.GetCurrentDirectory();

        dir.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void AppContextBaseDirectory_DebeExistir()
    {
        string baseDir = AppContext.BaseDirectory;

        Directory.Exists(baseDir).Should().BeTrue();
    }

    [Fact]
    public void EnvironmentDotNetVariables_DebeExistir()
    {
        string? dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        
        // DOTNET_ROOT puede no estar configurada en todas las máquinas
        // pero si lo está, debe apuntar a un directorio válido
        if (dotnetRoot is not null)
        {
            Directory.Exists(dotnetRoot).Should().BeTrue();
        }
    }

    // Tipos para los tests de JSON
    private sealed class ConfigTest
    {
        public string AppName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public LoggingTest Logging { get; set; } = new();
        public DatabaseTest Database { get; set; } = new();
    }

    private sealed class LoggingTest
    {
        public string Level { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
    }

    private sealed class DatabaseTest
    {
        public string Provider { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }
}
