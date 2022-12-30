namespace Sharpie.Backend;

/// <summary>
/// Special attribute used by <see cref="NativeLibraryWrapper{TFunctions}"/> to indicate that expected
/// methods are not obligatory.
/// </summary>
[AttributeUsage(AttributeTargets.Delegate)]
internal sealed class OptionalFeatureAttribute: Attribute
{
}
