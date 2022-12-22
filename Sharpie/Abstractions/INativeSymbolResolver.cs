namespace Sharpie.Abstractions;

using System.Reflection;

/// <summary>
/// Interface implemented by objects that can provide native symbols (such as function) to the callers.
/// The existing implementation is <see cref="NativeLibraryWrapper{TFunctions}"/> class.
/// </summary>
[PublicAPI]
internal interface INativeSymbolResolver
{
    /// <summary>
    /// Resolves a given function based on its delegate type.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the function.</typeparam>
    /// <returns>The resolved function.</returns>
    /// <exception cref="MissingMethodException">Thrown if the given function could not be resolved.</exception>
    TDelegate Resolve<TDelegate>() where TDelegate: MulticastDelegate;
}
