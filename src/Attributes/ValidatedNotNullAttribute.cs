using System;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace Microsoft.VisualStudio.Validation;
#pragma warning restore IDE0130

/// <summary>
/// Indicates to Code Analysis that a method validates a particular parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class ValidatedNotNullAttribute : Attribute {
}
