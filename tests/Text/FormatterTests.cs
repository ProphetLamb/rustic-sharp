using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

namespace Rustic.Text.Tests;

[TestFixture]
public class FormatterTests {
    public static IEnumerable<object[]> IndexFormatTestCases = new List<object[]> {
        new object[] { "Hello World", Array.Empty<string>(), "Hello World" },
        new object[] { "{0}", new[] { "Hello World" }, "Hello World" },
        new object[] { "Hello {0}", new[] { "World" }, "Hello World" },
        new object[] { "Hello{0}World", new[] { " " }, "Hello World" },
        new object[] { "{0} {1}", new[] { "Hello", "World" }, "Hello World" },
        new object[] { "{0}{1}{2}", new[] { "Hello", " ", "World" }, "Hello World" },
        new object[] { "{1}{0}{2}", new[] { " ", "Hello", "World" }, "Hello World" },
        new object[] { "", Array.Empty<string>(), "" },
        new object[] { "", new[] { "Empty", "Format" }, "" },
        new object[] { "{0} {1} {2}", new[] { "Hello", "World", "!", "?" }, "Hello World !" },
        new object[] { "", new[] { "Empty", "Format" }, "" },
    };

    [Test, TestCaseSource(nameof(IndexFormatTestCases))]
    public void IndexFormat(string format, string[] substitutes, string expected) {
        Fmt.Def.Index(format.AsSpan(), substitutes).Should().Be(expected);
    }

    public static IEnumerable<object[]> IndexFormatInvalidTestCases = new List<object[]> {
        new object[] { "{", Array.Empty<string>() },
        new object[] { "{0", Array.Empty<string>() },
        new object[] { "{0}", Array.Empty<string>() },
        new object[] { "{1}", new[] { "Hello World" } },
        new object[] { "{poof}", new[] { "Hello World" } },
    };

    [Test, TestCaseSource(nameof(IndexFormatInvalidTestCases))]
    public void IndexFormatInvalid(string format, string[] substitutes) {
        Assert.Catch<FormatException>(() => Fmt.Def.Index(format.AsSpan(), substitutes));
    }

    [Test, TestCaseSource(nameof(IndexFormatTestCases))]
    public void PrefixIndexFormatMissingPrefix(string format, string[] substitutes, string expected) {
        IdxDef<string> def = new("$", substitutes);
        Fmt.Format(format.AsSpan(), ref def).Should().Be(format);
    }

    public static IEnumerable<object[]> NamedFormatTestCases = new List<object[]> {
        new object[] { "Hello World", new Dictionary<string, string>(), "Hello World" },
        new object[] { "{foo}", new Dictionary<string, string> { ["foo"] = "Hello World" }, "Hello World" },
        new object[] { "Hello {foo}", new Dictionary<string, string> { ["foo"] = "World" }, "Hello World" },
        new object[] { "Hello{foo}World", new Dictionary<string, string> { ["foo"] = " " }, "Hello World" },
        new object[] { "{foo} {bar}", new Dictionary<string, string> { ["foo"] = "Hello", ["bar"] = "World" }, "Hello World" },
        new object[] { "{foo}{bar}{boo}", new Dictionary<string, string> { ["foo"] = "Hello", ["bar"] = " ", ["boo"] = "World" }, "Hello World" },
        new object[] { "{bar}{foo}{boo}", new Dictionary<string, string> { ["foo"] = " ", ["bar"] = "Hello", ["boo"] = "World" }, "Hello World" },
        new object[] { "", new Dictionary<string, string>(), "" },
        new object[] { "", new Dictionary<string, string> { ["foo"] = "Empty", ["bar"] = "Format" }, "" },
        new object[] { "{foo} {bar} {boo}", new Dictionary<string, string> { ["foo"] = "Hello", ["bar"] = "World", ["boo"] = "!", ["goo"] = "?" }, "Hello World !" },
        new object[] { "", new Dictionary<string, string> { ["foo"] = "Empty", ["bar"] = "Format" }, "" },
    };

    [Test, TestCaseSource(nameof(NamedFormatTestCases))]
    public void NamedFormat(string format, Dictionary<string, string> substitutes, string expected) {
        Fmt.Def.Named(format.AsSpan(), substitutes).Should().Be(expected);
    }

    public static IEnumerable<object[]> NamedFormatInvalidTestCases = new List<object[]> {
        new object[] { "{", new Dictionary<string, string>() },
        new object[] { "{0", new Dictionary<string, string>() },
        new object[] { "{0}", new Dictionary<string, string>() },
        new object[] { "{1}", new Dictionary<string, string> { ["foo"] = "Hello World" } },
        new object[] { "{poof}", new Dictionary<string, string> { ["foop"] = "Hello World" } },
    };

    [Test, TestCaseSource(nameof(NamedFormatInvalidTestCases))]
    public void NamedFormatInvalid(string format, Dictionary<string, string> substitutes) {
        Assert.Catch<FormatException>(() => Fmt.Def.Named(format.AsSpan(), substitutes));
    }

    [Test, TestCaseSource(nameof(NamedFormatTestCases))]
    public void PrefixNamedFormatMissingPrefix(string format, Dictionary<string, string> substitutes, string expected) {
        NamedDef<string> def = new("$", substitutes);
        Fmt.Format(format.AsSpan(), ref def).Should().Be(format);
    }

}
