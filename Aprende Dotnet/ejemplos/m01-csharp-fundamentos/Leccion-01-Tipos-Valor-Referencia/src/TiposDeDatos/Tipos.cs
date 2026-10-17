// ──────────────────────────────────────────────────────────────────────────
// Tipos usados en la demostración de la Lección 01
// Sin namespace para que sean accesibles desde top-level statements
// ──────────────────────────────────────────────────────────────────────────

// ──── struct: tipo de valor, vive en el stack ────
internal struct Punto2D
{
    public int X; // Se asigna en el inicializador de objeto: new() { X = 10, Y = 20 }
    public int Y;
#pragma warning disable CS0649 // Los campos se asignan mediante inicializador de objeto
#pragma warning restore CS0649
}

// ──── class: tipo de referencia, vive en el heap ────
internal class Cliente
{
    public string Nombre { get; set; }
    public string Email { get; set; }

    public Cliente(string nombre, string email)
    {
        Nombre = nombre;
        Email = email;
    }
}

// ──── record class: referencia, inmutable, value equality ────
internal record Persona(string Nombre, string Apellido, DateOnly FechaNacimiento);

// ──── record struct: valor, inmutable, alto rendimiento ────
internal readonly record struct Punto3D(int X, int Y, int Z);
