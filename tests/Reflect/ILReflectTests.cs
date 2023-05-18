using System;
using System.Reflection;

using NUnit.Framework;

namespace Rustic.Reflect.Tests;

public class TestObject {
    public int SomeField = 5;
    public readonly int SomeReadonlyField = 55;
    public const string SomeConstField = "This is a const field";

    public int SomeProperty { get; set; }
    public int SomeBackedProperty {
        get => SomeField;
        set => SomeField = value;
    }
    public int SomeReadonlyProperty => SomeReadonlyField;
    public string SomeConstProperty => SomeConstField;
}

[TestFixture]
public class ILReflectTests {
    [Test]
    public void Can_Get_Property_Getter() {
        PropertyInfo propertyInfo = typeof(TestObject).GetProperty("SomeProperty");
        MemberGetter<object, object> getter = null;

        Assert.DoesNotThrow(
            () => {
                getter = propertyInfo.DelegateForGet();
            }
        );

        Assert.IsNotNull(getter);
    }

    [Test]
    public void Can_Get_Property() {
        TestObject testObject = new();
        PropertyInfo propertyInfo = typeof(TestObject).GetProperty("SomeProperty");
        MemberGetter<object, object> getter = propertyInfo.DelegateForGet();

        Assert.AreEqual(0, getter(testObject));
    }

    [Test]
    public void Can_Get_Property_Setter() {
        PropertyInfo propertyInfo = typeof(TestObject).GetProperty("SomeProperty");
        MemberSetter<object, object> setter = null;

        Assert.DoesNotThrow(
            () => {
                setter = propertyInfo.DelegateForSet();
            }
        );

        Assert.IsNotNull(setter);
    }

    [Test]
    public void Can_Set_Property() {
        TestObject testObject = new();
        object testObjectAsObj = (object) testObject;
        PropertyInfo propertyInfo = typeof(TestObject).GetProperty("SomeProperty");
        MemberSetter<object, object> setter = propertyInfo.DelegateForSet();
        const int valueToSet = 123;

        Assert.DoesNotThrow(() => setter(ref testObjectAsObj, valueToSet));
        Assert.AreEqual(valueToSet, testObject.SomeProperty);
    }

    [Test]
    public void Can_Get_Field_Getter() {
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeField");
        MemberGetter<object, object> getter = null;

        Assert.DoesNotThrow(
            () => {
                getter = fieldInfo.DelegateForGet();
            }
        );

        Assert.IsNotNull(getter);
    }

    [Test]
    public void Can_Get_Field() {
        TestObject testObject = new();
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeField");
        MemberGetter<object, object> getter = fieldInfo.DelegateForGet();

        Assert.AreEqual(5, getter(testObject));
    }

    [Test]
    public void Can_Get_Field_Setter() {
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeField");
        MemberSetter<object, object> setter = null;

        Assert.DoesNotThrow(
            () => {
                setter = fieldInfo.DelegateForSet();
            }
        );

        Assert.IsNotNull(setter);
    }

    [Test]
    public void Can_Set_Field() {
        TestObject testObject = new();
        object testObjectAsObj = (object) testObject;
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeField");
        MemberSetter<object, object> setter = fieldInfo.DelegateForSet();
        const int valueToSet = 123;

        Assert.DoesNotThrow(() => setter(ref testObjectAsObj, valueToSet));
        Assert.AreEqual(valueToSet, testObject.SomeField);
    }

    [Test]
    public void Can_Get_Readonly_Field_Getter() {
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeReadonlyField");
        MemberGetter<object, object> getter = null;

        Assert.DoesNotThrow(
            () => {
                getter = fieldInfo.DelegateForGet();
            }
        );

        Assert.IsNotNull(getter);
    }

    [Test]
    public void Can_Get_Readonly_Field() {
        TestObject testObject = new();
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeReadonlyField");
        MemberGetter<object, object> getter = fieldInfo.DelegateForGet();

        Assert.AreEqual(55, getter(testObject));
    }

    [Test]
    public void Can_Get_Const_Field_Getter() {
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeConstField");
        MemberGetter<object, object> getter = null;

        Assert.DoesNotThrow(
            () => {
                getter = fieldInfo.DelegateForGet();
            }
        );

        Assert.IsNotNull(getter);
    }

    [Test]
    public void Can_Get_Const_Field() {
        TestObject testObject = new();
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeConstField");
        MemberGetter<object, object> getter = fieldInfo.DelegateForGet();

        Assert.AreEqual("This is a const field", getter(testObject));
    }

    [Test]
    public void Cant_Set_Const_Field() {
        FieldInfo fieldInfo = typeof(TestObject).GetField("SomeConstField");

        Assert.Throws<NotSupportedException>(() => fieldInfo.DelegateForSet());
    }

    [Test]
    public void Can_Get_Readonly_Backing_Field_Getter() {
        PropertyInfo propertyInfo = typeof(TestObject).GetProperty("SomeReadonlyProperty");
        MemberGetter<object, object> getter = null;

        Assert.DoesNotThrow(
            () => {
                getter = propertyInfo.DelegateForGet();
            }
        );

        Assert.IsNotNull(getter);
    }

    [Test]
    public void Can_Get_Readonly_Backing_Field() {
        TestObject testObject = new();
        PropertyInfo propertyInfo = typeof(TestObject).GetProperty("SomeReadonlyProperty");
        MemberGetter<object, object> getter = propertyInfo.DelegateForGet();

        Assert.AreEqual(55, getter(testObject));
    }

    [Test]
    public void Can_Get_Const_Backing_Field_Getter() {
        PropertyInfo propertyInfo = typeof(TestObject).GetProperty("SomeConstProperty");
        MemberGetter<object, object> getter = null;

        Assert.DoesNotThrow(
            () => {
                getter = propertyInfo.DelegateForGet();
            }
        );

        Assert.IsNotNull(getter);
    }

    [Test]
    public void Can_Get_Const_Backing_Field() {
        TestObject testObject = new();
        PropertyInfo propertyInfo = typeof(TestObject).GetProperty("SomeConstProperty");
        MemberGetter<object, object> getter = propertyInfo.DelegateForGet();

        Assert.AreEqual("This is a const field", getter(testObject));
    }
}
