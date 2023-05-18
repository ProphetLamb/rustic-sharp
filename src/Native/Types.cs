using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic.Native;

public static class Types {
    /// <summary>
    /// Reads a structure of type <typeparamref name="T"/> from the specified <see cref="Stream"/>.
    /// Consumes size of the structure bytes from the stream.
    /// </summary>
    /// <param name="reader">The reader from which <see cref="Marshal.SizeOf{T}()"/> bytes can be read.</param>
    /// <returns>The structure at the current position of the <paramref name="reader"/>.</returns>
    /// <exception cref="ArgumentException">If <see cref="Marshal.SizeOf{T}()"/> bytes could not be read.</exception>
    /// <remarks>See <see cref="ReadStruct{T}"/> for more info</remarks>
    public static T FromStreamStruct<T>(Stream reader)
        where T : struct {
        int size = Marshal.SizeOf<T>();
#if NET5_0_OR_GREATER
        Span<byte> buf = stackalloc byte[size];
        int read = reader.Read(buf);
#else
        byte[] buf = ArrayPool<byte>.Shared.Rent(size);
        int read = reader.Read(buf, 0, size);
#endif
        if (read != size) {
            throw new ArgumentException("Unable to read the required amount of bytes constituting the type");
        }
#if NET5_0_OR_GREATER
        ReadOnlySpan<byte> data = buf;
#else
        ReadOnlySpan<byte> data = buf.AsSpan(0, size);
#endif
        T value = ReadStruct<T>(in data);
#if !NET5_0_OR_GREATER
        ArrayPool<byte>.Shared.Return(buf);
#endif
        return value;
    }


    /// <summary>
    /// Same as <see cref="MemoryMarshal.Read{T}"/> but without the bounds checks and RuntimeHelpers.IsReferenceOrContainsReferences.
    ///
    /// RuntimeHelpers.IsReferenceOrContainsReferences throw an invalid exception when passed a blitable type with explicit struct layout,
    /// where there is an array with [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] attribute, and other edge cases.
    ///
    /// Use with care, as this method also accepts non blittable types, which will result in segfaults
    /// once the clr/gc moves or clears the value at the referenced location,
    /// or when accessing a virtual address form a different process!
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadStruct<T>(in ReadOnlySpan<byte> data)
        where T : struct {
        return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(data));
    }

    /// <summary>
    /// Reads a structure of type <typeparamref name="T"/> from the specified <see cref="Stream"/>.
    /// Consumes size of the structure bytes from the stream.
    /// </summary>
    /// <param name="reader">The reader from which <see cref="Marshal.SizeOf{T}()"/> bytes can be read.</param>
    /// <returns>The structure at the current position of the <paramref name="reader"/>.</returns>
    /// <exception cref="ArgumentException">If <see cref="Marshal.SizeOf{T}()"/> bytes could not be read.</exception>
    /// <remarks>See <see cref="ReadBlittable{T}"/> for more info.</remarks>
    public static T FromStreamBlittable<T>(Stream reader)
        where T : class, new() {
        int size = Marshal.SizeOf<T>();
#if NET5_0_OR_GREATER
        Span<byte> buf = stackalloc byte[size];
        int read = reader.Read(buf);
#else
        byte[] buf = ArrayPool<byte>.Shared.Rent(size);
        int read = reader.Read(buf, 0, size);
#endif
        if (read != size) {
            throw new ArgumentException("Unable to read the required amount of bytes constituting the type");
        }
#if NET5_0_OR_GREATER
        ReadOnlySpan<byte> data = buf;
#else
        ReadOnlySpan<byte> data = buf.AsSpan(0, size);
#endif
        T value = ReadBlittable<T>(in data);
#if !NET5_0_OR_GREATER
        ArrayPool<byte>.Shared.Return(buf);
#endif
        return value;
    }

    /// <summary>Casts the span to a fixed size structure at runtime.</summary>
    /// <param name="data">Size of span must match size of the type</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">If <typeparamref name="T"/> is an open generic type at runtime.</exception>
    /// <exception cref="ArgumentException">If <typeparamref name="T"/> an abstract type.</exception>
    /// <exception cref="ArgumentException">If <typeparamref name="T"/> contains non-primitive or non-blittable data. Such as ANY reference types, or if the type does not have blittable data, i.e. `StructLayout` is undefined.</exception>
    /// <returns>Value representing the data of the given type.</returns>
    /// <remarks>
    /// https://sharplab.io/#v2:C4LgTgrgdgNAJiA1AHwAICYCMBYAUKgBgAJVMA6AJWmAEsBbAUzIGEB7OgBxoBsGwBlPgDcaAYwYBnANx5CJclSi1GZAJJK+rDoLAjx0vHgBGAT2AMA2gF0icAIbA7RALxEoDAO5FT560QDeBDBBIQC+MrjMREJ23BAMLkQAogAewGQAYmDsAEJmkgAKNFDucAA8zAB8ABT2jgCUETFxTOoSHAyiwNWNhpHRsfHoiQCqUBJ2AGZMFAx2cGOxNADmpRU1YAyTRACyDHSsYCY7dmASABaxZADiDMCz05tQ4mU+DDV1dvW9uM1DauMOl0ehE8BZ+MBIF0ADJ2EysCDdWHwxEAaWKcDIggAjvElDRYvUrLJhlF/HgiJSiOpgABmYZ/BKuTDoWkRKlEAD0nPkxHMaXZVO5RAAIt5uDRgI4jLxBZTULSiMwegFQng1bgwRCocBkQikXD9eioJicXjaITifhhmLybgOeq+gr5AA2EjDVLAAIUqk++WK0hugAqRCyuXyEiKJQY5SDNVm8wA8lBuCZ+Bw7FBXvlKrYHF8/RyiB5znwEkGQO6YG5PD1C3ai1SQ4zEu4vEGQYWORBil7eFBEtUe0p6p8yNCGFBlsBzgBCOVF6ATaZdqkNxtF67MAASmbgvAGLW3JsSW93Jt4ZAAgtxuKxRNVGdWz3veEGTB0yFHSj8N0XJjQKQxkQ1RvAAVEQEhgKIBSQokABkewHEcJxnJc3A3HcDxls8DC1Pm3yro265/o24G2BIwCwWAg7gfUjLHpiV5wHAYCJpM34xomRgAFadN0v6kY2YzLkwbAcCYOR3qIADWiwSqsMa1JR1HVlBMGQtW/aCUJGp/npQlEERJAAOyHvEC4aqEQA==
    /// </remarks>
    public static T ReadBlittable<T>(in ReadOnlySpan<byte> data)
        where T : class, new() {
        T value = new();
        uint len = (uint) data.Length;
        unsafe {
            GCHandle valueHnd = GCHandle.Alloc(value, GCHandleType.Pinned);
            try {
                fixed (byte* srcPtr = &MemoryMarshal.GetReference(data)) {
                    byte* dstPtr = (byte*) valueHnd.AddrOfPinnedObject();
                    Unsafe.CopyBlockUnaligned(dstPtr, srcPtr, len);
                }
            } finally {
                valueHnd.Free();
            }
        }

        return value;
    }
}
