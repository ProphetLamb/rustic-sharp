using System;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis;

namespace Rustic.Source;

[CLSCompliant(false)]
public static class DiagnosticsExtensions
{
    [DebuggerStepThrough]
    public static bool AnyWarning(in this ImmutableArray<Diagnostic> diagnostics)
    {
        foreach (var d in diagnostics)
        {
            if (d.Severity >= DiagnosticSeverity.Warning)
            {
                return true;
            }
        }

        return false;
    }

    [DebuggerStepThrough]
    public static bool AnyError(in this ImmutableArray<Diagnostic> diagnostics)
    {
        foreach (var d in diagnostics)
        {
            if (d.Severity >= DiagnosticSeverity.Warning && d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
            {
                return true;
            }
        }

        return false;
    }
}
