// Polyfill necesario para usar record types en netstandard2.0.
// Los record usan init-only setters que requieren IsExternalInit.
// En .NET 5+ este tipo viene incluido; en netstandard2.0 debemos definirlo.

namespace System.Runtime.CompilerServices;

internal static class IsExternalInit { }
