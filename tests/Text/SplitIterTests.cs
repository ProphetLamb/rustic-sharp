using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HeaplessUtility.Text;
using HeaplessUtility.IO;

using NUnit.Framework;
using HeaplessUtility.Extensions;

namespace HeaplessUtility.Tests.Text
{
    [TestFixture]
    public class SplitIterTests
    {
        public static readonly string Dummy = "I hate code that is not working as intended... I am clearly in the wrong profession!";

        [Test]
        public void IterSoloTest()
        {
            var arr = Dummy.AsSpan().Split(' ').ToArray();
            var probe = Dummy.Split(' ').ToArray();
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterDoubleTest()
        {
            var arr = Dummy.AsSpan().Split(" .".AsSpan()).ToArray();
            var probe = Dummy.Split(' ', '.').ToArray();
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterTripleTest()
        {
            var arr = Dummy.AsSpan().Split(" .-".AsSpan()).ToArray();
            var probe = Dummy.Split(' ', '.', '-').ToArray();
            Assert.AreEqual(probe, arr);
        }

        [Test]
        public void IterRemoveEmptyTest()
        {
            var arr = Dummy.AsSpan().Split('.', SplitOptions.RemoveEmptyEntries).ToArray();
            var probe = Dummy.Split('.', StringSplitOptions.RemoveEmptyEntries).ToArray();
            Assert.AreEqual(probe, arr);
        }
    }
}
