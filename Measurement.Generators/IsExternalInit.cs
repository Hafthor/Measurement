namespace System.Runtime.CompilerServices;

// Polyfill so record structs / init-only setters compile on netstandard2.0 (the analyzer target
// framework does not ship System.Runtime.CompilerServices.IsExternalInit).
internal static class IsExternalInit { }
