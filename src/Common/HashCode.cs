using System.Security.Cryptography;

using Rustic;
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER)
#nullable disable
/*

The xxHash32 implementation is based on the code published by Yann Collet:
https://raw.githubusercontent.com/Cyan4973/xxHash/5c174cfa4e45a42f94082dc0d4539b39696afea1/xxhash.c

  xxHash - Fast Hash algorithm
  Copyright (C) 2012-2016, Yann Collet

  BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)

  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions are
  met:

  * Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.
  * Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following disclaimer
  in the documentation and/or other materials provided with the
  distribution.

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
  OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

  You can contact the author at :
  - xxHash homepage: http://www.xxhash.com
  - xxHash source repository : https://github.com/Cyan4973/xxHash

*/
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CA1066 // Implement IEquatable when overriding Object.Equals

#pragma warning disable IDE0130
namespace System;
#pragma warning restore IDE0130

// xxHash32 is used for the hash code.
// https://github.com/Cyan4973/xxHash
/// <summary>Combines the hash code for multiple values into a single hash code.</summary>
public struct HashCode {
    private static readonly uint s_seed = (uint)LocalRandom.Shared.Next();
    private uint _v1;
    private uint _v2;
    private uint _v3;
    private uint _v4;
    private uint _queue1;
    private uint _queue2;
    private uint _queue3;
    private uint _length;

    /// <summary>Diffuses the hash code returned by the specified value.</summary>
    /// <param name="value1">The value to add to the hash code.</param>
    /// <typeparam name="T1">The type of the value to add the hash code.</typeparam>
    /// <returns>The hash code that represents the single value.</returns>
    public static int Combine<T1>(T1 value1) {
        uint queuedValue = value1 is not null ? (uint)value1.GetHashCode() : 0U;
        return (int)MixFinal(QueueRound(MixEmptyState() + 4U, queuedValue));
    }

    /// <summary>Combines two values into a hash code.</summary>
    /// <param name="value1">The first value to combine into the hash code.</param>
    /// <param name="value2">The second value to combine into the hash code.</param>
    /// <typeparam name="T1">The type of the first value to combine into the hash code.</typeparam>
    /// <typeparam name="T2">The type of the second value to combine into the hash code.</typeparam>
    /// <returns>The hash code that represents the two values.</returns>
    public static int Combine<T1, T2>(T1 value1, T2 value2) {
        uint queuedValue1 = value1 is not null ? (uint)value1.GetHashCode() : 0U;
        uint queuedValue2 = value2 is not null ? (uint)value2.GetHashCode() : 0U;
        return (int)MixFinal(
            QueueRound(QueueRound(MixEmptyState() + 8U, queuedValue1), queuedValue2)
        );
    }

    /// <summary>Combines three values into a hash code.</summary>
    /// <param name="value1">The first value to combine into the hash code.</param>
    /// <param name="value2">The second value to combine into the hash code.</param>
    /// <param name="value3">The third value to combine into the hash code.</param>
    /// <typeparam name="T1">The type of the first value to combine into the hash code.</typeparam>
    /// <typeparam name="T2">The type of the second value to combine into the hash code.</typeparam>
    /// <typeparam name="T3">The type of the third value to combine into the hash code.</typeparam>
    /// <returns>The hash code that represents the three values.</returns>
    public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3) {
        uint queuedValue1 = value1 is not null ? (uint)value1.GetHashCode() : 0U;
        uint queuedValue2 = value2 is not null ? (uint)value2.GetHashCode() : 0U;
        uint queuedValue3 = value3 is not null ? (uint)value3.GetHashCode() : 0U;
        return (int)MixFinal(
            QueueRound(
                QueueRound(QueueRound(MixEmptyState() + 12U, queuedValue1), queuedValue2),
                queuedValue3
            )
        );
    }

    /// <summary>Combines four values into a hash code.</summary>
    /// <param name="value1">The first value to combine into the hash code.</param>
    /// <param name="value2">The second value to combine into the hash code.</param>
    /// <param name="value3">The third value to combine into the hash code.</param>
    /// <param name="value4">The fourth value to combine into the hash code.</param>
    /// <typeparam name="T1">The type of the first value to combine into the hash code.</typeparam>
    /// <typeparam name="T2">The type of the second value to combine into the hash code.</typeparam>
    /// <typeparam name="T3">The type of the third value to combine into the hash code.</typeparam>
    /// <typeparam name="T4">The type of the fourth value to combine into the hash code.</typeparam>
    /// <returns>The hash code that represents the four values.</returns>
    public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4) {
        uint input1 = value1 is not null ? (uint)value1.GetHashCode() : 0U;
        uint input2 = value2 is not null ? (uint)value2.GetHashCode() : 0U;
        uint input3 = value3 is not null ? (uint)value3.GetHashCode() : 0U;
        uint input4 = value4 is not null ? (uint)value4.GetHashCode() : 0U;
        uint v1_1;
        uint v2;
        uint v3;
        uint v4;
        Initialize(out v1_1, out v2, out v3, out v4);
        uint v1_2 = Round(v1_1, input1);
        v2 = Round(v2, input2);
        v3 = Round(v3, input3);
        v4 = Round(v4, input4);
        return (int)MixFinal(MixState(v1_2, v2, v3, v4) + 16U);
    }

    /// <summary>Combines five values into a hash code.</summary>
    /// <param name="value1">The first value to combine into the hash code.</param>
    /// <param name="value2">The second value to combine into the hash code.</param>
    /// <param name="value3">The third value to combine into the hash code.</param>
    /// <param name="value4">The fourth value to combine into the hash code.</param>
    /// <param name="value5">The fifth value to combine into the hash code.</param>
    /// <typeparam name="T1">The type of the first value to combine into the hash code.</typeparam>
    /// <typeparam name="T2">The type of the second value to combine into the hash code.</typeparam>
    /// <typeparam name="T3">The type of the third value to combine into the hash code.</typeparam>
    /// <typeparam name="T4">The type of the fourth value to combine into the hash code.</typeparam>
    /// <typeparam name="T5">The type of the fifth value to combine into the hash code.</typeparam>
    /// <returns>The hash code that represents the five values.</returns>
    public static int Combine<T1, T2, T3, T4, T5>(
        T1 value1,
        T2 value2,
        T3 value3,
        T4 value4,
        T5 value5) {
        uint input1 = value1 is not null ? (uint)value1.GetHashCode() : 0U;
        uint input2 = value2 is not null ? (uint)value2.GetHashCode() : 0U;
        uint input3 = value3 is not null ? (uint)value3.GetHashCode() : 0U;
        uint input4 = value4 is not null ? (uint)value4.GetHashCode() : 0U;
        uint queuedValue = value5 is not null ? (uint)value5.GetHashCode() : 0U;
        uint v1_1;
        uint v2;
        uint v3;
        uint v4;
        Initialize(out v1_1, out v2, out v3, out v4);
        uint v1_2 = Round(v1_1, input1);
        v2 = Round(v2, input2);
        v3 = Round(v3, input3);
        v4 = Round(v4, input4);
        return (int)MixFinal(QueueRound(MixState(v1_2, v2, v3, v4) + 20U, queuedValue));
    }

    /// <summary>Combines six values into a hash code.</summary>
    /// <param name="value1">The first value to combine into the hash code.</param>
    /// <param name="value2">The second value to combine into the hash code.</param>
    /// <param name="value3">The third value to combine into the hash code.</param>
    /// <param name="value4">The fourth value to combine into the hash code.</param>
    /// <param name="value5">The fifth value to combine into the hash code.</param>
    /// <param name="value6">The sixth value to combine into the hash code.</param>
    /// <typeparam name="T1">The type of the first value to combine into the hash code.</typeparam>
    /// <typeparam name="T2">The type of the second value to combine into the hash code.</typeparam>
    /// <typeparam name="T3">The type of the third value to combine into the hash code.</typeparam>
    /// <typeparam name="T4">The type of the fourth value to combine into the hash code.</typeparam>
    /// <typeparam name="T5">The type of the fifth value to combine into the hash code.</typeparam>
    /// <typeparam name="T6">The type of the sixth value to combine into the hash code.</typeparam>
    /// <returns>The hash code that represents the six values.</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6>(
        T1 value1,
        T2 value2,
        T3 value3,
        T4 value4,
        T5 value5,
        T6 value6) {
        uint input1 = value1 is not null ? (uint)value1.GetHashCode() : 0U;
        uint input2 = value2 is not null ? (uint)value2.GetHashCode() : 0U;
        uint input3 = value3 is not null ? (uint)value3.GetHashCode() : 0U;
        uint input4 = value4 is not null ? (uint)value4.GetHashCode() : 0U;
        uint queuedValue1 = value5 is not null ? (uint)value5.GetHashCode() : 0U;
        uint queuedValue2 = value6 is not null ? (uint)value6.GetHashCode() : 0U;
        uint v1_1;
        uint v2;
        uint v3;
        uint v4;
        Initialize(out v1_1, out v2, out v3, out v4);
        uint v1_2 = Round(v1_1, input1);
        v2 = Round(v2, input2);
        v3 = Round(v3, input3);
        v4 = Round(v4, input4);
        return (int)MixFinal(
            QueueRound(
                QueueRound(MixState(v1_2, v2, v3, v4) + 24U, queuedValue1),
                queuedValue2
            )
        );
    }

    /// <summary>Combines seven values into a hash code.</summary>
    /// <param name="value1">The first value to combine into the hash code.</param>
    /// <param name="value2">The second value to combine into the hash code.</param>
    /// <param name="value3">The third value to combine into the hash code.</param>
    /// <param name="value4">The fourth value to combine into the hash code.</param>
    /// <param name="value5">The fifth value to combine into the hash code.</param>
    /// <param name="value6">The sixth value to combine into the hash code.</param>
    /// <param name="value7">The seventh value to combine into the hash code.</param>
    /// <typeparam name="T1">The type of the first value to combine into the hash code.</typeparam>
    /// <typeparam name="T2">The type of the second value to combine into the hash code.</typeparam>
    /// <typeparam name="T3">The type of the third value to combine into the hash code.</typeparam>
    /// <typeparam name="T4">The type of the fourth value to combine into the hash code.</typeparam>
    /// <typeparam name="T5">The type of the fifth value to combine into the hash code.</typeparam>
    /// <typeparam name="T6">The type of the sixth value to combine into the hash code.</typeparam>
    /// <typeparam name="T7">The type of the seventh value to combine into the hash code.</typeparam>
    /// <returns>The hash code that represents the seven values.</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6, T7>(
        T1 value1,
        T2 value2,
        T3 value3,
        T4 value4,
        T5 value5,
        T6 value6,
        T7 value7) {
        uint input1 = value1 is not null ? (uint)value1.GetHashCode() : 0U;
        uint input2 = value2 is not null ? (uint)value2.GetHashCode() : 0U;
        uint input3 = value3 is not null ? (uint)value3.GetHashCode() : 0U;
        uint input4 = value4 is not null ? (uint)value4.GetHashCode() : 0U;
        uint queuedValue1 = value5 is not null ? (uint)value5.GetHashCode() : 0U;
        uint queuedValue2 = value6 is not null ? (uint)value6.GetHashCode() : 0U;
        uint queuedValue3 = value7 is not null ? (uint)value7.GetHashCode() : 0U;
        uint v1_1;
        uint v2;
        uint v3;
        uint v4;
        Initialize(out v1_1, out v2, out v3, out v4);
        uint v1_2 = Round(v1_1, input1);
        v2 = Round(v2, input2);
        v3 = Round(v3, input3);
        v4 = Round(v4, input4);
        return (int)MixFinal(
            QueueRound(
                QueueRound(
                    QueueRound(MixState(v1_2, v2, v3, v4) + 28U, queuedValue1),
                    queuedValue2
                ),
                queuedValue3
            )
        );
    }

    /// <summary>Combines eight values into a hash code.</summary>
    /// <param name="value1">The first value to combine into the hash code.</param>
    /// <param name="value2">The second value to combine into the hash code.</param>
    /// <param name="value3">The third value to combine into the hash code.</param>
    /// <param name="value4">The fourth value to combine into the hash code.</param>
    /// <param name="value5">The fifth value to combine into the hash code.</param>
    /// <param name="value6">The sixth value to combine into the hash code.</param>
    /// <param name="value7">The seventh value to combine into the hash code.</param>
    /// <param name="value8">The eighth value to combine into the hash code.</param>
    /// <typeparam name="T1">The type of the first value to combine into the hash code.</typeparam>
    /// <typeparam name="T2">The type of the second value to combine into the hash code.</typeparam>
    /// <typeparam name="T3">The type of the third value to combine into the hash code.</typeparam>
    /// <typeparam name="T4">The type of the fourth value to combine into the hash code.</typeparam>
    /// <typeparam name="T5">The type of the fifth value to combine into the hash code.</typeparam>
    /// <typeparam name="T6">The type of the sixth value to combine into the hash code.</typeparam>
    /// <typeparam name="T7">The type of the seventh value to combine into the hash code.</typeparam>
    /// <typeparam name="T8">The type of the eighth value to combine into the hash code.</typeparam>
    /// <returns>The hash code that represents the eight values.</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 value1,
        T2 value2,
        T3 value3,
        T4 value4,
        T5 value5,
        T6 value6,
        T7 value7,
        T8 value8) {
        uint input1 = value1 is not null ? (uint)value1.GetHashCode() : 0U;
        uint input2 = value2 is not null ? (uint)value2.GetHashCode() : 0U;
        uint input3 = value3 is not null ? (uint)value3.GetHashCode() : 0U;
        uint input4 = value4 is not null ? (uint)value4.GetHashCode() : 0U;
        uint input5 = value5 is not null ? (uint)value5.GetHashCode() : 0U;
        uint input6 = value6 is not null ? (uint)value6.GetHashCode() : 0U;
        uint input7 = value7 is not null ? (uint)value7.GetHashCode() : 0U;
        uint input8 = value8 is not null ? (uint)value8.GetHashCode() : 0U;
        uint v1_1;
        uint v2;
        uint v3;
        uint v4;
        Initialize(out v1_1, out v2, out v3, out v4);
        uint hash = Round(v1_1, input1);
        v2 = Round(v2, input2);
        v3 = Round(v3, input3);
        v4 = Round(v4, input4);
        uint v1_2 = Round(hash, input5);
        v2 = Round(v2, input6);
        v3 = Round(v3, input7);
        v4 = Round(v4, input8);
        return (int)MixFinal(MixState(v1_2, v2, v3, v4) + 32U);
    }


#nullable disable
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4) {
        v1 = (uint)((int)s_seed - 1640531535 - 2048144777);
        v2 = s_seed + 2246822519U;
        v3 = s_seed;
        v4 = s_seed - 2654435761U;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Round(uint hash, uint input) =>
        (hash + input * 2246822519U).RotateLeft(13) * 2654435761U;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint QueueRound(uint hash, uint queuedValue) =>
        (hash + queuedValue * 3266489917U).RotateLeft(17) * 668265263U;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixState(uint v1, uint v2, uint v3, uint v4) => v1.RotateLeft(1)
      + v2.RotateLeft(7) + v3.RotateLeft(12) + v4.RotateLeft(18);

    private static uint MixEmptyState() => s_seed + 374761393U;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixFinal(uint hash) {
        hash ^= hash >> 15;
        hash *= 2246822519U;
        hash ^= hash >> 13;
        hash *= 3266489917U;
        hash ^= hash >> 16;
        return hash;
    }


#nullable enable
    /// <summary>Adds a single value to the hash code.</summary>
    /// <param name="value">The value to add to the hash code.</param>
    /// <typeparam name="T">The type of the value to add to the hash code.</typeparam>
    public void Add<T>(T value) => Add(value is not null ? value.GetHashCode() : 0);

    /// <summary>Adds a single value to the hash code, specifying the type that provides the hash code function.</summary>
    /// <param name="value">The value to add to the hash code.</param>
    /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to use to calculate the hash code.
    /// This value can be a null reference (Nothing in Visual Basic), which will use the default equality comparer for <typeparamref name="T" />.</param>
    /// <typeparam name="T">The type of the value to add to the hash code.</typeparam>
    public void Add<T>(T value, IEqualityComparer<T>? comparer) => Add(
        value is not null ? 0 : comparer?.GetHashCode(value) ?? value?.GetHashCode() ?? 0
    );

    /// <summary>Adds a span of bytes to the hash code.</summary>
    /// <param name="value">The span to add.</param>
    public void AddBytes(ReadOnlySpan<byte> value) {
        ref byte local1 = ref MemoryMarshal.GetReference<byte>(value);
        // ISSUE: variable of a reference type
        ref byte local2 = ref Unsafe.AsRef<byte>(0);
        for (local2 = ref Unsafe.Add(ref local1, value.Length);
            Unsafe.ByteOffset(ref local1, ref local2) >= (nint)new IntPtr(4);
            local1 = ref Unsafe.Add(ref local1, 4)) {
            Add(Unsafe.ReadUnaligned<int>(ref local1));
        }

        for (; Unsafe.IsAddressLessThan(ref local1, ref local2); local1 = ref Unsafe.Add<byte>(ref local1, 1)) {
            Add((int)local1);
        }
    }

    private void Add(int value) {
        uint input = (uint)value;
        uint num = _length++;
        switch (num % 4U) {
        case 0:
            _queue1 = input;
            break;
        case 1:
            _queue2 = input;
            break;
        case 2:
            _queue3 = input;
            break;
        default:
            if (num == 3U) {
                Initialize(out _v1, out _v2, out _v3, out _v4);
            }

            _v1 = Round(_v1, _queue1);
            _v2 = Round(_v2, _queue2);
            _v3 = Round(_v3, _queue3);
            _v4 = Round(_v4, input);
            break;
        }
    }

    /// <summary>Calculates the final hash code after consecutive <see cref="System.HashCode.Add" /> invocations.</summary>
    /// <returns>The calculated hash code.</returns>
    public int ToHashCode() {
        uint length = _length;
        uint num = length % 4U;
        uint hash = (length < 4U ? MixEmptyState() : MixState(_v1, _v2, _v3, _v4))
          + length * 4U;

        if (num > 0U) {
            hash = QueueRound(hash, _queue1);
            if (num > 1U) {
                hash = QueueRound(hash, _queue2);
                if (num > 2U) {
                    hash = QueueRound(hash, _queue3);
                }
            }
        }

        return (int)MixFinal(hash);
    }

#pragma warning disable CS0809
    /// <summary>This method is not supported and should not be called.</summary>
    /// <exception cref="T:System.NotSupportedException">Always thrown when this method is called.</exception>
    /// <returns>This method will always throw a <see cref="T:System.NotSupportedException" />.</returns>
    [Obsolete(
        "HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.",
        true
    )]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.");

    /// <summary>This method is not supported and should not be called.</summary>
    /// <param name="obj">Ignored.</param>
    /// <exception cref="T:System.NotSupportedException">Always thrown when this method is called.</exception>
    /// <returns>This method will always throw a <see cref="T:System.NotSupportedException" />.</returns>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException("HashCode is a mutable struct and should not be compared with other HashCodes.");
#pragma warning restore CS0809
}
#endif
