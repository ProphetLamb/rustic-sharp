using System;

using NUnit.Framework;

namespace Rustic.Text.Tests;

[TestFixture]
public class SeqSplitIterTests {
    public static readonly string Dummy =
        "I hate code that is not working as intended... I am clearly in the wrong profession!";

    [Test]
    public void IterEmpty() {
        string? v = null;
        string[] arr = v.AsSpan().Split(" ").ToArray();
        Assert.AreEqual(Array.Empty<string>(), arr);
    }

    [Test]
    public void IterSoloTest() {
        string[] arr = Dummy.AsSpan().Split(" ").ToArray();
        string[] probe = new[] {
            "I",
            "hate",
            "code",
            "that",
            "is",
            "not",
            "working",
            "as",
            "intended...",
            "I",
            "am",
            "clearly",
            "in",
            "the",
            "wrong",
            "profession!",
        };

        Assert.AreEqual(probe, arr);
    }

    [Test]
    public void IterDoubleTest() {
        SeqSplitIter<char, string> iter = Dummy.AsSpan().Split(" ", ".");
        string[] arr = iter.ToArray();
        string[] probe = new[] {
            "I",
            "hate",
            "code",
            "that",
            "is",
            "not",
            "working",
            "as",
            "intended",
            "",
            "",
            "",
            "I",
            "am",
            "clearly",
            "in",
            "the",
            "wrong",
            "profession!",
        };

        Assert.AreEqual(probe, arr);
    }

    [Test]
    public void IterTripleTest() {
        string[] arr = Dummy.AsSpan().Split(" ", ".", "-").ToArray();
        string[] probe = new[] {
            "I",
            "hate",
            "code",
            "that",
            "is",
            "not",
            "working",
            "as",
            "intended",
            "",
            "",
            "",
            "I",
            "am",
            "clearly",
            "in",
            "the",
            "wrong",
            "profession!",
        };

        Assert.AreEqual(probe, arr);
    }

    [Test]
    public void IterRemoveEmptyTest() {
        string[] arr = Dummy.AsSpan().Split(".", SplitOptions.RemoveEmptyEntries).ToArray();
        string[] probe = new[] {
            "I hate code that is not working as intended", " I am clearly in the wrong profession!",
        };

        Assert.AreEqual(probe, arr);
    }

    [Test]
    public void IterIncludeSeparatorsTest() {
        string[] arr = Dummy.AsSpan().Split(" ", SplitOptions.IncludeSeparator).ToArray();
        string[] probe = new[] {
            "I ",
            "hate ",
            "code ",
            "that ",
            "is ",
            "not ",
            "working ",
            "as ",
            "intended... ",
            "I ",
            "am ",
            "clearly ",
            "in ",
            "the ",
            "wrong ",
            "profession!",
        };

        Assert.AreEqual(probe, arr);
    }
}
