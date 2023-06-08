using System.ComponentModel;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;
#pragma warning restore IDE0130

#if !NET5_0_OR_GREATER
/// <summary>
/// Reserved to be used by the compiler for tracking metadata.
/// This class should not be used by developers in source code.
/// This dummy class is required to compile records when targeting .NET Standard
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IsExternalInit
{
}
#endif
