using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;
using NUnit.Framework.Internal;

using Rustic.DataEnumGenerator;

namespace Rustic.Memory.Tests;

[TestFixture]
public class GeneratorTests
{
    private readonly StreamWriter _writer;

    public GeneratorTests()
    {
        _writer = new($"../../../{nameof(GeneratorTests)}.log", true);
        _writer.AutoFlush = true;
        Logger = new Logger(nameof(GeneratorTests), InternalTraceLevel.Debug, _writer);
    }

    ~GeneratorTests()
    {
        _writer.Dispose();
    }

    internal Logger Logger { get; }

    [Test]
    public void SimpleGeneratorTest()
    {
        // Create the 'input' compilation that the generator will act on
        Compilation inputCompilation = CreateCompilation(File.ReadAllText("TestFile.cs.test"));
        const int TEST_SOURCES_LEN = 1;
        const int GEN_SOURCES_LEN = 3; // Attribute + Dummy + NoAttr

        // directly create an instance of the generator
        // (Note: in the compiler this is loaded from an assembly, and created via reflection at runtime)
        DataEnumGen generator = new();

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generation pass
        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        Logging(outputCompilation, diagnostics);

        // We can now assert things about the resulting compilation:
        Debug.Assert(diagnostics.IsEmpty); // there were no diagnostics created by the generators
        Debug.Assert(outputCompilation.SyntaxTrees.Count() == TEST_SOURCES_LEN + GEN_SOURCES_LEN); // we have two syntax trees, the original 'user' provided one, and two added by the generator
        Debug.Assert(!outputCompilation.GetDiagnostics().Any(static (d) => d.Severity >= DiagnosticSeverity.Error)); // verify the compilation with the added source has no diagnostics

        // Or we can look at the results directly:
        GeneratorDriverRunResult runResult = driver.GetRunResult();

        // The runResult contains the combined results of all generators passed to the driver
        Debug.Assert(runResult.GeneratedTrees.Length == GEN_SOURCES_LEN);
        Debug.Assert(runResult.Diagnostics.IsEmpty);

        // Or you can access the individual results on a by-generator basis
        GeneratorRunResult generatorResult = runResult.Results[0];
        Debug.Assert(generatorResult.Generator.GetGeneratorType() == generator.GetType()); // Allow for IncrementalGeneratorWrapper
        Debug.Assert(generatorResult.Diagnostics.IsEmpty);
        Debug.Assert(generatorResult.GeneratedSources.Length == GEN_SOURCES_LEN);
        Debug.Assert(generatorResult.Exception is null);
    }

    private void Logging(Compilation comp, ImmutableArray<Diagnostic> diagnostics)
    {

        foreach (var diag in diagnostics)
        {
            Logger.Debug("Initial diagnostics {0}", diag.ToString());
        }

        foreach (var tree in comp.SyntaxTrees)
        {
            Logger.Debug("SyntaxTree\nName=\"{0}\",\nText=\"{1}\"", tree.FilePath, tree.ToString());
        }

        foreach (var diag in comp.GetDiagnostics())
        {
            Logger.Debug("Diagnostics {0}", diag.ToString());
        }
    }

    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(System.String).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.ComponentModel.DescriptionAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ReadOnlySpan<char>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<char>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.MethodImplAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.Serialization.ISerializable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.InteropServices.StructLayoutAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(@"C:\Program Files (x86)\dotnet\shared\Microsoft.NETCore.App\5.0.13\System.Runtime.dll"),
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}
