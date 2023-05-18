using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Rustic.Native;

public readonly partial struct PeHeader {
    private const ulong U32Max = uint.MaxValue;
    private const ulong U16Max = ushort.MaxValue;
    private const ulong U32Top = U32Max + 1; // == (1 << 32)

    /// <summary>Computes the IMAGHELP compatible checksum of a PE image at <paramref name="filePath"/>.</summary>
    public static (PeHeader, uint) FromFileComputeChecksum(string filePath) {
        return FromFileComputeChecksum(new FileInfo(filePath));
    }

    /// <summary>Computes the IMAGHELP compatible checksum of a PE image at <paramref name="info"/>.</summary>
    public static (PeHeader, uint) FromFileComputeChecksum(FileInfo info) {
        using FileStream fs = info.OpenRead();
        // Read the header
        PeHeader header = FromStreamInternal(fs, info.FullName);
        // Reset the stream, this should be cheap, as the default buffer for file streams is 1 page = 4096bytes,
        // and the header is well short of that.
        fs.Seek(0, SeekOrigin.Begin);
        // Determine checksum offset
        // PeHeader Address + COFF header size + Standard COFF Fields size + offset in Windows Specific Fields,
        // see https://i0.wp.com/practicalsecurityanalytics.com/wp-content/uploads/2019/10/1024px-Portable_Executable_32_bit_Structure_in_SVG_fixed.svg_.jpg?w=1024&ssl=1
        nuint checksumPos = header.DosHeader.NewHeaderAddress + 0x0058;
        nuint length = (nuint) info.Length;
        // Compute the checksum
        uint checksum = ComputeImageChecksum(fs, in checksumPos, in length);
        return (header, checksum);
    }

    /// <summary>
    /// Computes the IMAGHELP compatible checksum of a PE image <see cref="data"/>.
    /// </summary>
    /// <param name="data">The data of the PE image.</param>
    /// <param name="checksumPos">The byte offset at which the checksum should be, the WORD at this position is skipped.</param>
    /// <param name="length">The total number of bytes in the file, MUST also be equal to the total number of bytes readable from the <paramref name="data"/>-<see cref="Stream"/>.</param>
    /// <returns>The IMAGHELP compatible checksum of the PE image. The value that should be at <paramref name="checksumPos"/>.</returns>
    /// <remarks>
    /// The checksum is similar to CRC32.
    /// </remarks>
    private static uint ComputeImageChecksum(Stream data, in nuint checksumPos, in nuint length) {
        // Based on https://practicalsecurityanalytics.com/pe-checksum/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void PermuteChecksum(ref ulong checksum, in ulong value) {
            checksum = (checksum & U32Max) + value + (checksum >> 32);
            if (checksum > U32Top) {
                checksum = (checksum & U32Max) + (checksum >> 32);
            }
        }

        BinaryReader reader = new(data);
        ulong checksum = 0;
        nuint start = checksumPos / 4, stop = length / 4, remainder = length % 4;
        for (nuint i = 0; i < start; i++) {
            ulong temp = reader.ReadUInt32();
            PermuteChecksum(ref checksum, in temp);
        }

        // Discard DWORD at checksum
        _ = reader.ReadUInt32();

        for (nuint i = start + 1; i < stop; i++) {
            ulong temp = reader.ReadUInt32();
            PermuteChecksum(ref checksum, in temp);
        }

        if (remainder != 0) {
            // Pad remainder and permute checksum
            Span<byte> temp = stackalloc byte[8];
            for (nuint i = 0; i < 8; i++) {
                temp[(int) i] = i < remainder ? reader.ReadByte() : (byte) 0;
            }

            // the value may actually never exceed 2^32-1, but to reduce the number of casts we consume 64bit.
            ulong value = TypeMarshal.ReadStruct<ulong>(temp);
            PermuteChecksum(ref checksum, in value);
        }

        checksum = (checksum & U16Max) + (checksum >> 16);
        checksum = checksum + (checksum >> 16);
        checksum = checksum & U16Max;
        checksum += (ulong) data.Length;
        return (uint) checksum;
    }
}
