using System;
using System.Linq;

using NUnit.Framework;

using Rustic.Text;

namespace Rustic.Text.Tests
{
    [TestFixture]
    public class SplitIterTests
    {
        public static readonly string Dummy = "I hate code that is not working as intended... I am clearly in the wrong profession!";

        [Test]
        public void IterEmpty()
        {
            string? v = null;
            string[]? arr = v.AsSpan().Split(' ').ToArray();
            Assert.AreEqual(Array.Empty<string>(), arr);
        }

        [Test]
        public void IterSoloTest()
        {
            string[]? arr = Dummy.AsSpan().Split(' ').ToArray();
            string[]? probe = Dummy.Split(' ').ToArray();
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterDoubleTest()
        {
            var sep = Dummy.AsSpan().Split(" .".AsSpan());
            using Rustic.Memory.PoolBufWriter<string> buf = new();
            while (sep.MoveNext())
            {
                buf.Add(sep.Current.ToString());
            }

            string[]? arr = buf.ToArray();
            string[]? probe = Dummy.Split(' ', '.').ToArray();
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterTripleTest()
        {
            string[]? arr = Dummy.AsSpan().Split(" .-".AsSpan()).ToArray();
            string[]? probe = Dummy.Split(' ', '.', '-').ToArray();
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterRemoveEmptyTest()
        {
            string[]? arr = Dummy.AsSpan().Split('.', SplitOptions.RemoveEmptyEntries).ToArray();
            string[]? probe = Dummy.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            Assert.AreEqual(probe, arr);
        }
        [Test]
        public void IterIncludeSeparatorsTest()
        {
            string[]? arr = Dummy.AsSpan().Split(' ', SplitOptions.IncludeSeparator).ToArray();
            string[]? probe = Dummy.Split(' ').ToArray();
            InsertSeparators(probe);
            Assert.AreEqual(probe, arr);
        }

        private static void InsertSeparators(string[] array)
        {
            int last = array.Length - 1;
            for (var i = 0; i < last; i += 1)
            {
                array[i] += " ";
            }
        }
    }
}
