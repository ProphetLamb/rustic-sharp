using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

namespace Rustic.Memory.Tests;

[TestFixture]
public class TinyVecTests
{
    [Test]
    public void TestConstruct()
    {
        var vec = TinyVec.From(SampleData.Users[0]);
        vec.Count().Should().Be(1);
        vec = TinyVec.From(SampleData.Users[0], SampleData.Users[1]);
        vec.Count().Should().Be(2);
        vec = TinyVec.From(SampleData.Users[0], SampleData.Users[1], SampleData.Users[2]);
        vec.Count().Should().Be(3);
        vec = TinyVec.From(SampleData.Users[0], SampleData.Users[1], SampleData.Users[2], SampleData.Users[3]);
        vec.Count().Should().Be(4);
        vec = TinyVec.From(SampleData.Users, 12, 65);
        vec.Count().Should().Be(65);
        vec = TinyVec.Copy<User, IEnumerable<User>>(SampleData.Users.Take(12));
        vec.Count().Should().Be(12);
    }
}
