using System;
using System.Collections.Generic;
using System.Linq;

using Bogus;

using FluentAssertions;

using NUnit.Framework;

using Rustic.Common.Extensions;
using Rustic.Memory.Vector;

namespace Rustic.Memory.Tests
{
    [TestFixture]
    public class VecTests
    {
        [Test]
        public void TestAdd()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.Add(null);
            reference.Add(null);

            for (int i = 0; i < 123; i++)
            {
                list.Add(SampleData.Users[i]);
                reference.Add(SampleData.Users[i]);
            }

            list.ToArray().Should().BeEquivalentTo(reference);
        }

        [Test]
        public void TestAddRange()
        {
            List<User> reference = new();
            Vec<User> list = new();

            int increment;
            for (int i = 0; i < 400; i += increment)
            {
                increment = Randomizer.Seed.Next(2, 10);

                User[] users = SampleData.Users.AsSpan(i, increment).ToArray();
                list.AddRange(users);
                reference.AddRange(users);
            }

            list.ToArray().Should().BeEquivalentTo(reference);
        }

        [Test]
        public void TestClear()
        {
            List<User> reference = new();
            Vec<User> list = new();

            reference.Clear();
            list.Clear();

            list.ToArray().Should().BeEquivalentTo(reference);

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            reference.Clear();
            list.Clear();

            list.ToArray().Should().BeEquivalentTo(reference);

            reference.Add(SampleData.Users[12]);
            list.Add(SampleData.Users[12]);

            list.ToArray().Should().BeEquivalentTo(reference);
        }

        [Test]
        public void TestIndexOf()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            for (int i = 0; i < 100; i++)
            {
                User user = Randomizer.Seed.ChooseFrom(SampleData.Users);
                list.IndexOf(user).Should().Be(reference.IndexOf(user));
            }

            list.IndexOf(null).Should().Be(reference.IndexOf(null));
        }

        [Test]
        public void TestInsert()
        {
            List<User> reference = new();
            Vec<User> list = new();

            for (int i = 0; i < 100; i++)
            {
                User user = Randomizer.Seed.ChooseFrom(SampleData.Users);
                int index = Randomizer.Seed.Next(0, reference.Count);
                reference.Insert(index, user);
                list.Insert(index, user);
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>().Insert(-1, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>().Insert(1, null));
        }

        [Test]
        public void TestInsertRange()
        {
            List<User> reference = new();
            Vec<User> list = new();

            int increment;
            for (int i = 0; i < 400; i += increment)
            {
                increment = Randomizer.Seed.Next(2, 10);

                User[] users = SampleData.Users.AsSpan(i, increment).ToArray();
                int index = Randomizer.Seed.Next(0, reference.Count);
                reference.InsertRange(index, users);
                list.InsertRange(index, users);
            }

            list.ToArray().Should().BeEquivalentTo(reference);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>().InsertRange(-1, Array.Empty<User>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>().InsertRange(1, Array.Empty<User>()));
        }

        [Test]
        public void TestLastIndexOf()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            for (int i = 0; i < 100; i++)
            {
                User user = Randomizer.Seed.ChooseFrom(SampleData.Users);
                list.LastIndexOf(user).Should().Be(reference.LastIndexOf(user));
            }

            list.LastIndexOf(null).Should().Be(reference.LastIndexOf(null));
        }

        [Test]
        public void TestRemove()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            for (int i = 0; i < 100; i++)
            {
                User user = Randomizer.Seed.ChooseFrom(SampleData.Users);
                list.Remove(user).Should().Be(reference.Remove(user));
            }

            list.ToArray().Should().BeEquivalentTo(reference);

            list.Remove(null).Should().Be(reference.Remove(null));

            list.ToArray().Should().BeEquivalentTo(reference);
        }

        [Test]
        public void TestRemoveAt()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            for (int i = 0; i < 100; i++)
            {
                int index = Randomizer.Seed.Next(0, list.Count);
                list.RemoveAt(index);
                reference.RemoveAt(index);
            }

            list.ToArray().Should().BeEquivalentTo(reference);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>(2).RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>(2).RemoveAt(1));
        }

        [Test]
        public void TestRemoveRange()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            int increment;
            for (int i = 0; i < 400; i += increment)
            {
                increment = Randomizer.Seed.Next(2, 10);

                int index = Randomizer.Seed.Next(0, list.Count - increment);
                list.RemoveRange(index, increment);
                reference.RemoveRange(index, increment);
            }

            list.ToArray().Should().BeEquivalentTo(reference);

            list.RemoveRange(0, 0);

            list.ToArray().Should().BeEquivalentTo(reference);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>(2).RemoveRange(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>(2).RemoveRange(1, 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>(2).RemoveRange(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>(2).RemoveRange(0, 1));
        }

        [Test]
        public void TestReverse()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            list.Reverse();
            reference.Reverse();

            list.ToArray().Should().BeEquivalentTo(reference);

            list.Reverse(1, 23);
            reference.Reverse(1, 23);

            list.ToArray().Should().BeEquivalentTo(reference);
        }

        [Test]
        public void TestSort()
        {
            new Vec<User>().Sort();
            new Vec<User>(SampleData.Users.ToArray()).Sort();
        }

        [Test]
        public void TestSortComparer()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            list.Sort(13, 52, UserComparer.Instance);
            reference.Sort(13, 52, UserComparer.Instance);

            list.ToArray().Should().BeEquivalentTo(reference);

            list.Sort(UserComparer.Instance);
            reference.Sort(UserComparer.Instance);

            list.ToArray().Should().BeEquivalentTo(reference);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>().Sort(-1, 0, UserComparer.Instance));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>().Sort(1, 0, UserComparer.Instance));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>().Sort(0, -1, UserComparer.Instance));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vec<User>().Sort(0, 1, UserComparer.Instance));
        }

        [Test]
        public void TestSortComparison()
        {
            List<User> reference = new();
            Vec<User> list = new();

            list.AddRange(SampleData.Users);
            reference.AddRange(SampleData.Users);

            list.Sort(UserComparer.Comparison);
            reference.Sort(UserComparer.Comparison);

            list.ToArray().Should().BeEquivalentTo(reference);
        }
    }
}
