using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;
using NUnit.Framework.Internal;

using Rustic.Source;

namespace Rustic.DataEnumGen.Tests;

[TestFixture]
public class GeneratorTests
{
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
    using static PreferThis;
    using static DummyData;

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

    internal enum PreferThisEnum
    {
        [DataEnum(typeof(string))]
        Symbol,
        IsPreferThis,
    }

    public static class Program
    {
        public static void Main()
        {
            // Extension classes
            Type dummyExType = typeof(DummyExtensions);
            Type NoAttrExType = typeof(NoAttrExtensions);
            Type NoFlagsExType = typeof(NoFlagsExtensions);
            // Data structs
            DummyData appenedTheDataSuffix = default!;
            PreferThis removedTheEnumSuffix = Symbol(""Jannis"");
        }
    }
}
");
        const int TEST_SOURCES_LEN = 1;
        const int GEN_SOURCES_LEN = 5; // Attribute + Dummy + NoAttr + NoFlags + PreferThisEnum
        DataEnumGen generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var generatorDiagnostics);

        generatorDiagnostics.Should().NotContain(d => d.Severity == DiagnosticSeverity.Warning);
        outputCompilation.SyntaxTrees.Count().Should().Be(TEST_SOURCES_LEN + GEN_SOURCES_LEN);

        var analyzerDiagnostics = outputCompilation.GetDiagnostics();
        analyzerDiagnostics.Should().NotContain(d => d.Severity == DiagnosticSeverity.Error);

        GeneratorDriverRunResult runResult = driver.GetRunResult();

        runResult.GeneratedTrees.Length.Should().Be(GEN_SOURCES_LEN);
        Assert.IsTrue(runResult.Diagnostics.IsEmpty);

        GeneratorRunResult generatorResult = runResult.Results[0];
        Assert.IsTrue(generatorResult.Generator.GetGeneratorType() == generator.GetType());
        Assert.IsTrue(generatorResult.Diagnostics.IsEmpty);
        generatorResult.GeneratedSources.Length.Should().Be(GEN_SOURCES_LEN);
        generatorResult.Exception.Should().BeNull();
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
                MetadataReference.CreateFromFile(Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(HashCode).GetTypeInfo().Assembly.Location),
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}
