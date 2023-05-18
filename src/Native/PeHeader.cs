using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;


// based on https://gist.github.com/augustoproiete/b51f29f74f5f5b2c59c39e47a8afc3a3
namespace Rustic.Native;

/// <summary>
/// Reads in the header information of the Portable Executable format.
/// Provides information such as the date the assembly was compiled.
/// </summary>
[CLSCompliant(false)]
public readonly partial struct PeHeader {
    /// <summary>
    /// The PE Header signature aka magic, must be ASCII "PE\0\0" = 0x50450000
    /// </summary>
    public readonly uint Signature;
    /// <summary>
    /// The DOS header
    /// </summary>
    public readonly ImageDosHeader DosHeader;
    /// <summary>
    /// The file header
    /// </summary>
    public readonly ImageFileHeader FileHeader;
    /// <summary>
    /// Optional 32 bit file header
    /// </summary>
    public readonly ImageOptionalHeader32 OptionalHeader32;
    /// <summary>
    /// Optional 64 bit file header
    /// </summary>
    public readonly ImageOptionalHeader64 OptionalHeader64;
    /// <summary>
    /// Image Section headers. Number of sections is in the file header.
    /// </summary>
    public readonly ImmutableArray<ImageSectionHeader> ImageSectionHeaders;

    /// <summary>
    /// The **optional** path to the file that contains the header.
    /// </summary>
    public readonly string? FilePath;


    private PeHeader(
        uint peHeaderSignature,
        ImageDosHeader dosHeader,
        ImageFileHeader fileHeader,
        ImageOptionalHeader32 optionalHeader32,
        ImageOptionalHeader64 optionalHeader64,
        ImmutableArray<ImageSectionHeader> imageSectionHeaders,
        string? filePath) {
        Signature = peHeaderSignature;
        DosHeader = dosHeader;
        FileHeader = fileHeader;
        OptionalHeader32 = optionalHeader32;
        OptionalHeader64 = optionalHeader64;
        ImageSectionHeaders = imageSectionHeaders;
        FilePath = filePath;
    }

    /// <summary>Opens the file for reading, reads and returns the PE header.</summary>
    /// <param name="filePath">The path to the file to read.</param>
    /// <returns>The PE header read.</returns>
    public static PeHeader FromFile(string filePath) {
        return FromFile(new FileInfo(filePath));
    }

    /// <summary>Opens the file for reading, reads and returns the PE header.</summary>
    /// <param name="info">The information of the file to read.</param>
    /// <returns>The PE header read.</returns>
    public static PeHeader FromFile(FileInfo info) {
        // Read in the DLL or EXE and get the timestamp
        using FileStream stream = info.OpenRead();
        return FromStreamInternal(stream, info.FullName);
    }


    /// <summary>Reads the PE header in binary from the stream.</summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The PE header read.</returns>
    public static PeHeader FromStream(Stream stream) {
        return FromStreamInternal(stream, null);
    }

    private static PeHeader FromStreamInternal(Stream stream, string? filePath) {
        // See https://0xrick.github.io/win-internals/pe3/ for more information on how the PE header is found.
        BinaryReader reader = new(stream);
        ImageDosHeader dosHeader = reader.ReadStruct<ImageDosHeader>();

        // Follow pointer to PE header
        stream.Seek(dosHeader.NewHeaderAddress, SeekOrigin.Begin);

        // Read PE header signature
        // [0x0000, 0x0018) - COFF header: Consists of the Signature + IMAGE_FILE_HEADER
        uint peHeadersSignature = reader.ReadUInt32();
        ImageFileHeader fileHeader = reader.ReadStruct<ImageFileHeader>();
        // [0x0018, 0x0034) - Standard COFF Fields
        // [0x0034, 0x0080) - Windows platform specific fields
        // [0x0080, 0x00F0) - Data Directories
        ImageOptionalHeader32 optionalHeader32;
        ImageOptionalHeader64 optionalHeader64;
        if (fileHeader.Is32Bit) {
            optionalHeader32 = reader.ReadStruct<ImageOptionalHeader32>();
            optionalHeader64 = default;
        } else {
            optionalHeader32 = default;
            optionalHeader64 = reader.ReadStruct<ImageOptionalHeader64>();
        }

        ImmutableArray<ImageSectionHeader>.Builder imageSectionHeaders =
            ImmutableArray.CreateBuilder<ImageSectionHeader>(fileHeader.NumberOfSections);

        for (int headerNo = 0; headerNo < imageSectionHeaders.Count; ++headerNo) {
            imageSectionHeaders.Add(reader.ReadStruct<ImageSectionHeader>());
        }

        return new(
            peHeadersSignature,
            dosHeader,
            fileHeader,
            optionalHeader32,
            optionalHeader64,
            imageSectionHeaders.MoveToImmutable(),
            filePath
        );
    }

    /// <summary>
    /// Gets the header of the .NET assembly that called this function
    /// </summary>
    /// <returns></returns>
    public static PeHeader GetCallingAssemblyHeader() {
        // Get the path to the calling assembly, which is the path to the
        return GetAssemblyHeader(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Gets the header of the specified assembly
    /// </summary>
    /// <exception cref="InvalidOperationException">Could not get the assembly path</exception>
    public static PeHeader GetAssemblyHeader(Assembly assembly) {
        // Get the path to the own assembly, which is the path to the
        // DLL or EXE that we want the time of
        string? filePath = assembly?.Location;
        if (filePath is null) {
            throw new InvalidOperationException("Could not get the assembly path");
        }

        return FromFile(filePath);
    }

    public bool IsSignatureValid => Signature == 0x50450000; // == "PE\0\0"

    /// <summary>
    /// Gets if the file header is 32 bit or not
    /// </summary>
    public bool Is32BitHeader => FileHeader.Is32Bit;

    /// <summary>
    /// Gets the timestamp from the file header
    /// </summary>
    /// <remarks>
    /// Stored as seconds since 1970/1/1 at UTC time, is converted to local time.
    /// </remarks>
    public DateTimeOffset TimeStamp => FileHeader.TimeStamp;

    /// <summary>
    /// Gets the <see cref="ImageVersion"/> parsed from the optional 32bit-, or 64bit-header in the format {MajorImage, MinorImage, MajorSubsystem, MinorSubsystem}.
    /// </summary>
    public Version ImageVersion => Is32BitHeader ? OptionalHeader32.ImageVersion : OptionalHeader64.ImageVersion;
}
