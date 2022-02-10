using System;
using System.Linq;

using NUnit.Framework;

namespace Rustic.Text.Tests
{
    [TestFixture]
    public class SeqSplitIterTests
    {
        public static readonly string Dummy = "I hate code that is not working as intended... I am clearly in the wrong profession!";

        [Test]
        public void IterEmpty()
        {
            string? v = null;
            var arr = v.AsSpan().Split(" ").ToArray();
            Assert.AreEqual(Array.Empty<string>(), arr);
        }

        [Test]
        public void IterSoloTest()
        {
            var arr = Dummy.AsSpan().Split(" ").ToArray();
            var probe = new[] { "I", "hate", "code", "that", "is", "not", "working", "as", "intended...", "I", "am", "clearly", "in", "the", "wrong", "profession!" };
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterDoubleTest()
        {
            var iter = Dummy.AsSpan().Split(" ",".");
            var arr = iter.ToArray();
            var probe = new[] { "I", "hate", "code", "that", "is", "not", "working", "as", "intended", "", "", "", "I", "am", "clearly", "in", "the", "wrong", "profession!" };
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterTripleTest()
        {
            var arr = Dummy.AsSpan().Split(" ", ".", "-").ToArray();
            var probe = new[] { "I", "hate", "code", "that", "is", "not", "working", "as", "intended", "", "", "", "I", "am", "clearly", "in", "the", "wrong", "profession!" };
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterRemoveEmptyTest()
        {
            var arr = Dummy.AsSpan().Split(".", SplitOptions.RemoveEmptyEntries).ToArray();
            var probe = new[] { "I hate code that is not working as intended", " I am clearly in the wrong profession!" };
            Assert.AreEqual(probe, arr);
        }
        [Test]
        public void IterIncludeSeparatorsTest()
        {
            var arr = Dummy.AsSpan().Split(" ", SplitOptions.IncludeSeparator).ToArray();
            var probe = new[]
            {
                "I ", "hate ", "code ", "that ", "is ", "not ", "working ", "as ", "intended... ", "I ", "am ", "clearly ", "in ", "the ", "wrong ", "profession!"
            };
            Assert.AreEqual(probe, arr);
        }
    }
}
