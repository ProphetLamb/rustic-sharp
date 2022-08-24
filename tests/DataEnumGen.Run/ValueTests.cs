using NUnit.Framework;

using System;
using System.ComponentModel;

using FluentAssertions;

using Rustic;

using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Rustic.DataEnumGen.Run.Tests
{
    using static DummyData;

    public enum Dummy : byte
    {
        [Description("The default value.")]
        Default = 0,
        [DataEnum(typeof((int, int)))]
        Minimum = 1,
        [DataEnum(typeof((long, long)))]
        Maximum = 2,
    }

    public enum NoAttr
    {
        [Description("This is a description.")]
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

    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void Test()
        {

        }
    }

    [TestFixture]
    public class ValueTests
    {
        [Test]
        public void TestCtorEnumEqNoData()
        {
            DummyData defV = Default();
            Dummy def = defV;

            defV.Value.Should().Be(def);

            Assert.IsTrue(defV.Equals(def));
        }

        [Test]
        public void TestCtorEnumEqData()
        {
            DummyData minV = Minimum((0, 123));
            Dummy min = minV;

            minV.Value.Should().Be(min);

            minV.Equals(min).Should().BeTrue();
        }
    }
}
