using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

namespace Rustic.Memory.Tests;

[TestFixture]
public class TinySpanTests
{
    [Test]
    public void TestConstruct()
    {
        var vec = TinySpan.From(SampleData.Users[0]);
        vec.Count().Should().Be(1);
        vec = TinySpan.From(SampleData.Users[0], SampleData.Users[1]);
        vec.Count().Should().Be(2);
        vec = TinySpan.From(SampleData.Users[0], SampleData.Users[1], SampleData.Users[2]);
        vec.Count().Should().Be(3);
        vec = TinySpan.From(SampleData.Users[0], SampleData.Users[1], SampleData.Users[2], SampleData.Users[3]);
        vec.Count().Should().Be(4);
        vec = TinySpan.From(SampleData.Users, 12, 65);
        vec.Count().Should().Be(65);
        vec = TinySpan.Copy<User, IEnumerable<User>>(SampleData.Users.Take(12));
        vec.Count().Should().Be(12);
    }
}

internal static class TinySpanExtensions
{
    public static int Count<T>(in this TinySpan<T> self)
    {
        var count = 0;
        TinySpan<T>.Enumerator en = self.GetEnumerator();
        while (en.MoveNext())
        {
            T item = en.Current;
            count += 1;
        }
        return count;
    }

}
