using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

#nullable enable
namespace Rustic.Attributes
{
    using System;

    /// <summary>Allows a value member to ship with additional data.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DataEnumAttribute : Attribute
    {
        public DataEnumAttribute(Type data) { }
    }
}

namespace Rustic.Yee
{
    public enum Dummy : byte
    {
        Default = 0,
        [Attributes.DataEnum(typeof((int, int)))]
        [Description("Descr")]
        Minimum = 1,
        [Attributes.DataEnum(typeof((long, long)))]
        Maximum = 2,
    }

    public static class DummyExtensions
    {
        public static System.String Name(this Dummy value)
        {
            switch (value)
            {
                case Dummy.Default:
                    return nameof(Dummy.Default);
                default:
                    return value.ToString();
            }
        }
    }

    [Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public readonly struct DummyValue : IEquatable<DummyValue>, IEquatable<Dummy>, ISerializable
    {
        public static ReadOnlySpan<Dummy> Values
        {
            get
            {
                return new Rustic.Yee.Dummy[]
                {
                    Dummy.Default,
                    Dummy.Minimum,
                    Dummy.Maximum,
                };
            }
        }

        [System.Runtime.InteropServices.FieldOffset(0)]
        public readonly Dummy Value;

        [System.Runtime.InteropServices.FieldOffset(sizeof(Dummy))]
        public readonly (int, int) MinimumUnchecked;

        [System.Runtime.InteropServices.FieldOffset(sizeof(Dummy))]
        public readonly (long, long) MaximumUnchecked;

        #region Constructor

        private DummyValue(Dummy value, (int, int) minimum, (long, long) maximum)
        {
            Value = value;
            switch (value)
            {
                case Dummy.Minimum:
                    MaximumUnchecked = maximum;

                    MinimumUnchecked = minimum;
                    break;
                case Dummy.Maximum:
                    MinimumUnchecked = minimum;

                    MaximumUnchecked = maximum;
                    break;
                default:
                    MinimumUnchecked = default!;
                    MaximumUnchecked = default!;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DummyValue Default()
        {
            return new DummyValue(Dummy.Default, default!, default!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DummyValue Minimum((int, int) value)
        {
            return new DummyValue(Dummy.Minimum, value, default!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DummyValue Maximum((long, long) value)
        {
            return new DummyValue(Dummy.Maximum, default!, value);
        }

        #endregion Constructor

        public bool IsDefault
        {
            get { return Equals(Dummy.Default); }
        }

        public bool IsMinimum
        {
            get { return Equals(Dummy.Minimum); }
        }

        public bool IsMaximum
        {
            get { return Equals(Dummy.Maximum); }
        }

        public bool TryMinimum(out (int, int) value)
        {
            if (IsMinimum)
            {
                value = MinimumUnchecked;
                return true;
            }

            value = default!;
            return false;
        }

        public bool TryMaximum(out (long, long) value)
        {
            if (IsMaximum)
            {
                value = MaximumUnchecked;
                return true;
            }

            value = default!;
            return false;
        }

        public (long, long) ExpectMinimum(string? message)
        {
            if (IsMinimum)
            {
                return MinimumUnchecked;
            }

            throw new InvalidOperationException(message);
        }

        public (long, long) ExpectMaximum(string? message)
        {
            if (IsMaximum)
            {
                return MaximumUnchecked;
            }

            throw new InvalidOperationException(message);
        }


        #region IEquatable members

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Dummy other)
        {
            return Value == other;
        }

        public bool Equals(DummyValue other)
        {
            if (Value != other.Value)
            {
                return false;
            }

            switch (Value)
            {
                case Dummy.Minimum:
                    return System.Collections.Generic.EqualityComparer<(int, int)>.Default.Equals(MinimumUnchecked, other.MinimumUnchecked);
                case Dummy.Maximum:
                    return System.Collections.Generic.EqualityComparer<(long, long)>.Default.Equals(MaximumUnchecked, other.MaximumUnchecked);
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is DummyValue other)
            {
                return Equals(other);
            }
            if (obj is Dummy value)
            {
                return Equals(value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Value);
            switch (Value)
            {
                case Dummy.Minimum:
                    hash.Add(MinimumUnchecked);
                    break;
                case Dummy.Maximum:
                    hash.Add(MaximumUnchecked);
                    break;
            }

            return hash.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in DummyValue left, in DummyValue right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in DummyValue left, in DummyValue right)
        {
            return !left.Equals(right);
        }

        #endregion IEquatable members

        private DummyValue(SerializationInfo info, StreamingContext context)
        {
            Value = (Dummy)info.GetValue("Value", typeof(Dummy))!;
            switch (Value)
            {
                case Dummy.Minimum:
                    MaximumUnchecked = default!;

                    MinimumUnchecked = ((int, int))info.GetValue("MinimumUnchecked", typeof((int, int)))!;
                    break;
                case Dummy.Maximum:
                    MinimumUnchecked = default!;

                    MaximumUnchecked = ((long, long))info.GetValue("MaximumUnchecked", typeof((long, long)))!;
                    break;
                default:
                    MinimumUnchecked = default!;
                    MaximumUnchecked = default!;
                    break;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Value", Value, typeof(Dummy));
            switch (Value)
            {
                case Dummy.Minimum:
                    info.AddValue("MinimumUnchecked", MinimumUnchecked, typeof((int, int)));
                    break;
                case Dummy.Maximum:
                    info.AddValue("MaximumUnchecked", MaximumUnchecked, typeof((long, long)));
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Dummy(in DummyValue value)
        {
            return value.Value;
        }
    }
}
#nullable restore
