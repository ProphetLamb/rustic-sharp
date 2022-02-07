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
        [Rustic.Attributes.DataEnum(typeof((int, int)))]
        [System.ComponentModel.Description("Descr")]
        Minimum = 1,
        [Rustic.Attributes.DataEnum(typeof((long, long)))]
        Maximum = 2,
    }

    public static class DummyExtensions
    {
        public static System.String Name(this Rustic.Yee.Dummy value)
        {
            switch (value)
            {
                case Rustic.Yee.Dummy.Default:
                    return nameof(Rustic.Yee.Dummy.Default);
                default:
                    return value.ToString();
            }
        }
    }

    [Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public readonly struct DummyValue : System.IEquatable<DummyValue>, System.IEquatable<Rustic.Yee.Dummy>, ISerializable
    {
        public static System.ReadOnlySpan<Rustic.Yee.Dummy> Values
        {
            get
            {
                return new Rustic.Yee.Dummy[]
                {
                    Rustic.Yee.Dummy.Default,
                    Rustic.Yee.Dummy.Minimum,
                    Rustic.Yee.Dummy.Maximum,
                };
            }
        }

        [System.Runtime.InteropServices.FieldOffset(0)]
        public readonly Rustic.Yee.Dummy Value;

        [System.Runtime.InteropServices.FieldOffset(sizeof(Rustic.Yee.Dummy))]
        public readonly (int, int) MinimumUnchecked;

        [System.Runtime.InteropServices.FieldOffset(sizeof(Rustic.Yee.Dummy))]
        public readonly (long, long) MaximumUnchecked;

        #region Constructor

        private DummyValue(Rustic.Yee.Dummy value, (int, int) minimum, (long, long) maximum)
        {
            this.Value = value;
            switch (value)
            {
                case Rustic.Yee.Dummy.Minimum:
                    this.MaximumUnchecked = maximum;

                    this.MinimumUnchecked = minimum;
                    break;
                case Rustic.Yee.Dummy.Maximum:
                    this.MinimumUnchecked = minimum;

                    this.MaximumUnchecked = maximum;
                    break;
                default:
                    this.MinimumUnchecked = default!;
                    this.MaximumUnchecked = default!;
                    break;
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static DummyValue Default()
        {
            return new DummyValue(Rustic.Yee.Dummy.Default, default!, default!);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static DummyValue Minimum((int, int) value)
        {
            return new DummyValue(Rustic.Yee.Dummy.Minimum, value, default!);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static DummyValue Maximum((long, long) value)
        {
            return new DummyValue(Rustic.Yee.Dummy.Maximum, default!, value);
        }

        #endregion Constructor

        public bool IsDefault
        {
            get { return this.Equals(Rustic.Yee.Dummy.Default); }
        }

        public bool IsMinimum
        {
            get { return this.Equals(Rustic.Yee.Dummy.Minimum); }
        }

        public bool IsMaximum
        {
            get { return this.Equals(Rustic.Yee.Dummy.Maximum); }
        }

        public bool TryMinimum(out (int, int) value)
        {
            if (this.IsMinimum)
            {
                value = this.MinimumUnchecked;
                return true;
            }

            value = default!;
            return false;
        }

        public bool TryMaximum(out (long, long) value)
        {
            if (this.IsMaximum)
            {
                value = this.MaximumUnchecked;
                return true;
            }

            value = default!;
            return false;
        }

        public (long, long) ExpectMinimum(string? message)
        {
            if (this.IsMinimum)
            {
                return MinimumUnchecked;
            }

            throw new System.InvalidOperationException(message);
        }

        public (long, long) ExpectMaximum(string? message)
        {
            if (this.IsMaximum)
            {
                return MaximumUnchecked;
            }

            throw new System.InvalidOperationException(message);
        }


        #region IEquatable members

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(Rustic.Yee.Dummy other)
        {
            return this.Value == other;
        }

        public bool Equals(DummyValue other)
        {
            if (this.Value != other.Value)
            {
                return false;
            }

            switch (this.Value)
            {
                case Rustic.Yee.Dummy.Minimum:
                    return System.Collections.Generic.EqualityComparer<(int, int)>.Default.Equals(this.MinimumUnchecked, other.MinimumUnchecked);
                case Rustic.Yee.Dummy.Maximum:
                    return System.Collections.Generic.EqualityComparer<(long, long)>.Default.Equals(this.MaximumUnchecked, other.MaximumUnchecked);
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is DummyValue other)
            {
                return this.Equals(other);
            }
            if (obj is Rustic.Yee.Dummy value)
            {
                return this.Equals(value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            System.HashCode hash = new System.HashCode();
            hash.Add(this.Value);
            switch (this.Value)
            {
                case Rustic.Yee.Dummy.Minimum:
                    hash.Add(this.MinimumUnchecked);
                    break;
                case Rustic.Yee.Dummy.Maximum:
                    hash.Add(this.MaximumUnchecked);
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
        public static implicit operator Rustic.Yee.Dummy(in DummyValue value)
        {
            return value.Value;
        }
    }
}
#nullable restore
