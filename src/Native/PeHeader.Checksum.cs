using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rustic.Native;

public readonly partial struct PeHeader
{
    private const ulong U32Max = UInt32.MaxValue;
    private const ulong U16Max = UInt16.MaxValue;
    private const ulong U32Top = U32Max + 1; // == (1 << 32)

    /// <summary>
    /// Computes the IMAGHELP compatible checksum of a PE image <see cref="data"/>.
    /// </summary>
    /// <param name="data">The data of the PE image.</param>
    /// <param name="checksumOffset">The byte offset at which the checksum should be, the WORD at this position is skipped.</param>
    /// <returns>The IMAGHELP compatible checksum of the PE image. The value that should be at <paramref name="checksumOffset"/>.</returns>
    private static uint ComputeImageChecksum(ReadOnlySpan<byte> data, in int checksumOffset)
    {
        // Based on https://practicalsecurityanalytics.com/pe-checksum/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void PermuteChecksum(ref ulong checksum, in ulong value)
        {
            checksum = (checksum & U32Max) + value + (checksum >> 32);
            if (checksum > U32Top)
            {
                checksum = (checksum & U32Max) + (checksum >> 32);
            }
        }

        ulong checksum = 0;
        nint start = checksumOffset / 4, stop = data.Length / 4, remainder = data.Length % 4;
        unsafe
        {
            // consume data in 4byte DWORD chunks
            fixed (uint* dwordData = &MemoryMarshal.GetReference(data))
            {
                for (nint i = 0; i < start; i++)
                {
                    ulong temp = dwordData[i];
                    PermuteChecksum(ref checksum, in temp);
                }

                for (nint i = start + 1; i < stop; i++)
                {
                    ulong temp = dwordData[i];
                    PermuteChecksum(ref checksum, in temp);
                }
            }
        }

        //Perform the same calculation on the padded remainder
        if (remainder != 0)
        {
            Span<byte> temp = stackalloc byte[4];
            nint off = data.Length - remainder;
            for (nint i = 0; i < 4; i++)
            {
                temp[(int)i] = i < remainder ? data[(int)(off + i)] : (byte)0;
            }

            ulong value = Types.ReadStruct<uint>(temp);
            PermuteChecksum(ref checksum, in value);
        }

        checksum = (checksum & U16Max) + (checksum >> 16);
        checksum = (checksum) + (checksum >> 16);
        checksum = (checksum & U16Max);
        checksum += (ulong)data.Length;
        return (uint)checksum;
    }
}
