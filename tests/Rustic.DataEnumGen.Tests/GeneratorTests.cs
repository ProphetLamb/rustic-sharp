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

using Rustic.Source;

namespace Rustic.DataEnumGen.Tests;

[TestFixture]
public class GeneratorTests
{
    public GeneratorTests()
    {
        var writer = new StreamWriter($"GeneratorTests-{typeof(string).Assembly.ImageRuntimeVersion}.log", true);
        writer.AutoFlush = true;
        Logger = new Logger(nameof(GeneratorTests), InternalTraceLevel.Debug, writer);
    }

    internal Logger Logger { get; }

    [Test]
    public void SimpleGeneratorTest()
    {
        // Create the 'input' compilation that the generator will act on
        Compilation inputCompilation = CreateCompilation(@"
using System;
using System.ComponentModel;

using Rustic;

namespace Rustic.DataEnumGen.Tests.TestAssembly
{
    public enum Dummy : byte
    {
        [Description(""The default value."")]
        Default = 0,
        [Rustic.DataEnum(typeof((int, int)))]
        Minimum = 1,
        [Rustic.DataEnum(typeof((long, long)))]
        Maximum = 2,
    }

    public enum NoAttr
    {
        [Description(""This is a description."")]
        This,
        Is,
        Sparta,
    }

    [Flags]
    public enum NoFlags : byte
    {
        Flag = 1 << 0,
        Enums = 1 << 1,
        Are = 1 << 2,
        Not = 1 << 3,
        Supported = 1 << 4,
    }

    public static class Program
    {
        public static void Main()
        {
            DummyValue value = default!;
        }
    }
}
");
        const int TEST_SOURCES_LEN = 1;
        const int GEN_SOURCES_LEN = 4; // Attribute + Dummy + NoAttr + NoFlags
        DataEnumGen generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        Logging(outputCompilation, diagnostics);

        Debug.Assert(!diagnostics.AnyWarning()); // there were no diagnostics created by the generators
        Debug.Assert(outputCompilation.SyntaxTrees.Count() == TEST_SOURCES_LEN + GEN_SOURCES_LEN); // we have two syntax trees, the original 'user' provided one, and two added by the generator
        Debug.Assert(!outputCompilation.GetDiagnostics().AnyError()); // verify the compilation with the added source has no diagnostics

        GeneratorDriverRunResult runResult = driver.GetRunResult();

        Debug.Assert(runResult.GeneratedTrees.Length == GEN_SOURCES_LEN);
        Debug.Assert(runResult.Diagnostics.IsEmpty);

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

        var d = comp.GetDiagnostics();
        foreach (var diag in d)
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
                MetadataReference.CreateFromFile(@"C:\Program Files (x86)\dotnet\shared\Microsoft.NETCore.App\6.0.1\System.Runtime.dll"),
                MetadataReference.CreateFromFile(typeof(HashCode).GetTypeInfo().Assembly.Location),
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}
