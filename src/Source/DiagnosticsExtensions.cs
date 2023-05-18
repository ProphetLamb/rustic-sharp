using System;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis;

namespace Rustic.Source;

/// <summary>Extensions simplifying handling <see cref="Diagnostic"/>s emitted during code generation.</summary>
[CLSCompliant(false)]
public static class DiagnosticsExtensions {
    /// <summary>Indicates whether any of the diagnostics is a warning</summary>
    [DebuggerStepThrough]
    public static bool AnyWarning(in this ImmutableArray<Diagnostic> diagnostics) {
        foreach (Diagnostic? d in diagnostics) {
            if (d.Severity >= DiagnosticSeverity.Warning) {
                return true;
            }
        }

        return false;
    }

    /// <summary>Indicates whether any of the diagnostics is a error</summary>
    [DebuggerStepThrough]
    public static bool AnyError(in this ImmutableArray<Diagnostic> diagnostics) {
        foreach (Diagnostic? d in diagnostics) {
            if ((d.Severity >= DiagnosticSeverity.Warning && d.IsWarningAsError)
             || d.Severity == DiagnosticSeverity.Error) {
                return true;
            }
        }

        return false;
    }
}
