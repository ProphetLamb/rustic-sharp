using System;
using System.Collections.Generic;
using System.Linq;

using Bogus;
using Bogus.DataSets;

namespace Rustic.Memory.Tests;

public static class SampleData {
    public static Order[] Orders { get; } = GetFakers().OrderFaker.Generate(3000).ToArray();

    public static User[] Users { get; } = GetFakers().UserFaker.Generate(1000).ToArray();

    private static (Faker<User> UserFaker, Faker<Order> OrderFaker) GetFakers() {
        Randomizer.Seed = new Random(3897234);
        string[] fruit = {"apple", "banana", "orange", "strawberry", "kiwi",};

        int orderIds = 0;
        Faker<Order> testOrders = new Faker<Order>()
            //Ensure all properties have rules. By default, StrictMode is false
            //Set a global policy by using Faker.DefaultStrictMode if you prefer.
           .StrictMode(true)
            //OrderId is deterministic
           .RuleFor(o => o.OrderId, f => orderIds++)
            //Pick some fruit from a basket
           .RuleFor(o => o.Item, f => f.PickRandom(fruit))
            //A random quantity from 1 to 10
           .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10))
            //A nullable int? with 80% probability of being null.
            //The .OrNull extension is in the Bogus.Extensions namespace.
           .RuleFor(o => o.LotNumber, f => f.Random.Int(0, 100).OrNull(f, .8f));

        int userIds = 0;
        Faker<User> testUsers = new Faker<User>()
            //Optional: Call for objects that have complex initialization
           .CustomInstantiator(f => new User(userIds++, f.Random.Replace("###-##-####")))

            //Use an enum outside scope.
           .RuleFor(u => u.Gender, f => f.PickRandom<Name.Gender>())

            //Basic rules using built-in generators
           .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(u.Gender))
           .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(u.Gender))
           .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
           .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
           .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
           .RuleFor(u => u.SomethingUnique, f => $"Value {f.UniqueIndex}")

            //Use a method outside scope.
           .RuleFor(u => u.CartId, f => Guid.NewGuid())
            //Compound property with context, use the first/last name properties
           .RuleFor(u => u.FullName, (f, u) => u.FirstName + " " + u.LastName)
            //And composability of a complex collection.
           .RuleFor(u => u.Orders, f => testOrders.Generate(3).ToList());

        return (testUsers, testOrders);
    }
}

public class Order {
    public int OrderId { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }
    public int? LotNumber { get; set; }
}

public class User {
    public User(int userId, string ssn) {
        Id = userId;
        SSN = ssn;
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string SomethingUnique { get; set; }
    public Guid SomeGuid { get; set; }

    public string Avatar { get; set; }
    public Guid CartId { get; set; }
    public string SSN { get; set; }
    public Name.Gender Gender { get; set; }

    public List<Order> Orders { get; set; }
}

public class UserComparer : IComparer<User>, System.Collections.IComparer {
    public static UserComparer Instance => new();

    public static Comparison<User> Comparison => (left, right) => Instance.Compare(left, right);

    public int Compare(User x, User y) {
        if (ReferenceEquals(x, y)) {
            return 0;
        }

        if (y is null) {
            return 1;
        }

        if (x is null) {
            return -1;
        }

        int firstNameComparison = string.CompareOrdinal(x.FirstName, y.FirstName);
        if (firstNameComparison != 0) {
            return firstNameComparison;
        }

        int lastNameComparison = string.CompareOrdinal(x.LastName, y.LastName);
        if (lastNameComparison != 0) {
            return lastNameComparison;
        }

        return x.Gender.CompareTo(y.Gender);
    }

    public int Compare(object x, object y) {
        if (x == y) {
            return 0;
        }

        if (x is null) {
            return -1;
        }

        if (y is null) {
            return 1;
        }

        if (x is User a
         && y is User b) {
            return Compare(a, b);
        }

        throw new ArgumentException("", nameof(x));
    }
}
