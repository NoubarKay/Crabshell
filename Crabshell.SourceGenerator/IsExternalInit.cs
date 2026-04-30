// Polyfill required for 'record' and 'init' setters when targeting netstandard2.0
namespace System.Runtime.CompilerServices;

internal static class IsExternalInit { }