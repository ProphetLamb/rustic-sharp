using System;
using System.Runtime.InteropServices;
// ReSharper disable UnassignedReadonlyField
#pragma warning disable CS1591 // Variable never initialized

namespace Rustic.Native;

public readonly partial struct PeHeader
{
    // For the following enum definitions see https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-image_optional_header32 and related pages.
    // The definitions may be incomplete!
    // Also see: https://0xrick.github.io/win-internals/pe3/
    // Also see: https://practicalsecurityanalytics.com/pe-checksum/

    public readonly struct ImageDosHeader
    {
        // DOS .EXE header
        /// <summary>
        /// Magic number, must be ASCII "MZ" = 0x5A4D
        /// </summary>
        public readonly ushort Magic;
        /// <summary>
        /// Bytes on last page of file
        /// </summary>
        public readonly ushort BytesInLastPage;
        /// <summary>
        /// Number of 512-byte pages (including last page) in file
        /// </summary>
        public readonly ushort PageCount;
        /// <summary>
        /// Number of entries in the relocation table
        /// </summary>
        public readonly ushort RelocationTableCount;
        /// <summary>
        /// Size/Width of headers in paragraphs (16-bytes)
        /// </summary>
        public readonly ushort ParagraphHeaderSize;
        /// <summary>
        /// Minimum number of allocated paragraphs necessary.
        /// </summary>
        public readonly ushort ParagraphMinimumAllocCount;
        /// <summary>
        /// Maximum number of allocated paragraphs (ever) required.
        /// </summary>
        public readonly ushort ParagraphMaximumAllocCount;
        /// <summary>
        /// Initial value of the SS (Stack Segment Register) relative to the program memory.
        /// </summary>
        public readonly ushort StackSegmentRegisterPointer;
        /// <summary>
        /// Initial value of the Stack Pointer.
        /// </summary>
        public readonly ushort StackPointer;
        /// <summary>
        /// File checksum or zero.
        /// </summary>
        public readonly ushort Checksum;
        /// <summary>
        /// Initial value of the IP (Instruction Pointer)
        /// </summary>
        public readonly ushort InstructionPointer;
        /// <summary>
        /// Initial value of the CS (Code Segment) relative to the program memory.
        /// </summary>
        public readonly ushort CodeSegmentNumber;
        /// <summary>
        /// File address of the relocation table. Should be 0x40 for PE DOS Stub.
        /// </summary>
        public readonly ushort RelocationTableAddress;
        /// <summary>
        /// Overlay number or zero
        /// </summary>
        public readonly ushort OverlayNumber;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved1_0;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved1_1;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved1_2;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved1_3;
        /// <summary>
        /// Reserved
        /// </summary>
        /// <summary>
        /// OEM identifier for <see cref="OemInfo"/>.
        /// </summary>
        public readonly ushort OemId;
        /// <summary>
        /// OEM information, <see cref="OemId"/> specific.
        /// </summary>
        public readonly ushort OemInfo;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_0;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_1;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_2;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_3;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_4;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_5;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_6;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_7;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_8;
        /// <summary>
        /// Reserved
        /// </summary>
        public readonly ushort Reserved2_9;
        /// <summary>
        /// Reserved
        /// </summary>
        /// <summary>
        /// File address of the new PE header. Should be 0x100 to account for the standard DOS Stub.
        /// </summary>
        public readonly uint NewHeaderAddress;

        public bool IsSignatureValid => Magic == 0x5a4d;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ImageDataDirectory
    {
        public readonly uint VirtualAddress;
        public readonly uint Size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ImageOptionalHeader32
    {
        public readonly OptionalMagicValue Magic;
        public readonly byte MajorLinkerVersion;
        public readonly byte MinorLinkerVersion;
        public readonly uint SizeOfCode;
        public readonly uint SizeOfInitializedData;
        public readonly uint SizeOfUninitializedData;
        public readonly uint AddressOfEntryPoint;
        public readonly uint BaseOfCode;
        public readonly uint BaseOfData;
        public readonly uint ImageBase;
        public readonly uint SectionAlignment;
        public readonly uint FileAlignment;
        public readonly ushort MajorOperatingSystemVersion;
        public readonly ushort MinorOperatingSystemVersion;
        public readonly ushort MajorImageVersion;
        public readonly ushort MinorImageVersion;
        public readonly ushort MajorSubsystemVersion;
        public readonly ushort MinorSubsystemVersion;
        public readonly uint Win32VersionValue;
        public readonly uint SizeOfImage;
        public readonly uint SizeOfHeaders;
        public readonly uint CheckSum;
        public readonly SubsystemValue Subsystem;
        public readonly DllCharacteristicsFlags DllCharacteristics;
        public readonly uint SizeOfStackReserve;
        public readonly uint SizeOfStackCommit;
        public readonly uint SizeOfHeapReserve;
        public readonly uint SizeOfHeapCommit;
        public readonly uint LoaderFlags;
        public readonly uint NumberOfRvaAndSizes;

        public readonly ImageDataDirectory ExportTable;
        public readonly ImageDataDirectory ImportTable;
        public readonly ImageDataDirectory ResourceTable;
        public readonly ImageDataDirectory ExceptionTable;
        public readonly ImageDataDirectory CertificateTable;
        public readonly ImageDataDirectory BaseRelocationTable;
        public readonly ImageDataDirectory Debug;
        public readonly ImageDataDirectory Architecture;
        public readonly ImageDataDirectory GlobalPtr;
        public readonly ImageDataDirectory TLSTable;
        public readonly ImageDataDirectory LoadConfigTable;
        public readonly ImageDataDirectory BoundImport;
        public readonly ImageDataDirectory IAT;
        public readonly ImageDataDirectory DelayImportDescriptor;
        public readonly ImageDataDirectory CLRRuntimeHeader;
        public readonly ImageDataDirectory Reserved;

        /// <summary>
        /// The version of this image and subsystem.
        /// </summary>
        public Version ImageVersion =>  new(MajorImageVersion, MinorImageVersion, MajorSubsystemVersion, MinorSubsystemVersion);

        /// <summary>
        /// The required minimum operating system version.
        /// </summary>
        public Version OsVersion => new(MajorOperatingSystemVersion, MinorOperatingSystemVersion);

        /// <summary>
        /// The version of the linker that produced this image.
        /// </summary>
        public Version LinkerVersion => new(MajorLinkerVersion, MinorLinkerVersion);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ImageOptionalHeader64
    {
        public readonly OptionalMagicValue Magic;
        public readonly byte MajorLinkerVersion;
        public readonly byte MinorLinkerVersion;
        public readonly uint SizeOfCode;
        public readonly uint SizeOfInitializedData;
        public readonly uint SizeOfUninitializedData;
        public readonly uint AddressOfEntryPoint;
        public readonly uint BaseOfCode;
        public readonly ulong ImageBase;
        public readonly uint SectionAlignment;
        public readonly uint FileAlignment;
        public readonly ushort MajorOperatingSystemVersion;
        public readonly ushort MinorOperatingSystemVersion;
        public readonly ushort MajorImageVersion;
        public readonly ushort MinorImageVersion;
        public readonly ushort MajorSubsystemVersion;
        public readonly ushort MinorSubsystemVersion;
        public readonly uint Win32VersionValue;
        public readonly uint SizeOfImage;
        public readonly uint SizeOfHeaders;
        public readonly uint CheckSum;
        public readonly SubsystemValue Subsystem;
        public readonly DllCharacteristicsFlags DllCharacteristics;
        public readonly ulong SizeOfStackReserve;
        public readonly ulong SizeOfStackCommit;
        public readonly ulong SizeOfHeapReserve;
        public readonly ulong SizeOfHeapCommit;
        public readonly uint LoaderFlags;
        public readonly uint NumberOfRvaAndSizes;

        public readonly ImageDataDirectory ExportTable;
        public readonly ImageDataDirectory ImportTable;
        public readonly ImageDataDirectory ResourceTable;
        public readonly ImageDataDirectory ExceptionTable;
        public readonly ImageDataDirectory CertificateTable;
        public readonly ImageDataDirectory BaseRelocationTable;
        public readonly ImageDataDirectory Debug;
        public readonly ImageDataDirectory Architecture;
        public readonly ImageDataDirectory GlobalPtr;
        public readonly ImageDataDirectory TLSTable;
        public readonly ImageDataDirectory LoadConfigTable;
        public readonly ImageDataDirectory BoundImport;
        public readonly ImageDataDirectory IAT;
        public readonly ImageDataDirectory DelayImportDescriptor;
        public readonly ImageDataDirectory CLRRuntimeHeader;
        public readonly ImageDataDirectory Reserved;

        /// <summary>
        /// The version of this image and subsystem.
        /// </summary>
        public Version ImageVersion =>  new(MajorImageVersion, MinorImageVersion, MajorSubsystemVersion, MinorSubsystemVersion);

        /// <summary>
        /// The required minimum operating system version.
        /// </summary>
        public Version OsVersion => new(MajorOperatingSystemVersion, MinorOperatingSystemVersion);

        /// <summary>
        /// The version of the linker that produced this image.
        /// </summary>
        public Version LinkerVersion => new(MajorLinkerVersion, MinorLinkerVersion);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ImageFileHeader
    {
        public readonly FileMachineValue Machine;
        public readonly ushort NumberOfSections;
        public readonly uint TimeDateStamp;
        public readonly uint PointerToSymbolTable;
        public readonly uint NumberOfSymbols;
        public readonly ushort SizeOfOptionalHeader;
        public readonly FileCharacteristicFlags Characteristics;

        public bool Is32Bit => (FileCharacteristicFlags.Machine32Bit & Characteristics) != 0;

        public DateTimeOffset TimeStamp
        {
            get
            {
                // Timestamp is a date offset from 1970 at UTC TZ
                DateTimeOffset baseTime = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

                // Add in the number of seconds since 1970/1/1
                return baseTime.AddSeconds(TimeDateStamp);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public readonly struct ImageSectionHeader
    {
        [FieldOffset(0)] [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly char[] Name;

        [FieldOffset(8)] public readonly uint VirtualSize;
        [FieldOffset(12)] public readonly uint VirtualAddress;
        [FieldOffset(16)] public readonly uint SizeOfRawData;
        [FieldOffset(20)] public readonly uint PointerToRawData;
        [FieldOffset(24)] public readonly uint PointerToRelocations;
        [FieldOffset(28)] public readonly uint PointerToLinenumbers;
        [FieldOffset(32)] public readonly ushort NumberOfRelocations;
        [FieldOffset(34)] public readonly ushort NumberOfLinenumbers;
        [FieldOffset(36)] public readonly DataSectionFlags Characteristics;

        public string NameString => new(Name);
    }

    public enum OptionalMagicValue : ushort
    {
        /// <summary>
        /// This file is an 32 bit executable image.
        /// </summary>
        Header32 = 0x10b,

        /// <summary>
        /// This file is an 64 bit executable image.
        /// </summary>
        Header64 = 0x20b,

        /// <summary>
        /// This file is an 32 bit ROM image.
        /// </summary>
        Rom32 = 0x107,
    }

    [Flags]
    public enum DataSectionFlags : uint
    {
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        TypeReg = 0x00000000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        TypeDsect = 0x00000001,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        TypeNoLoad = 0x00000002,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        TypeGroup = 0x00000004,

        /// <summary>
        /// The section should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid only for object files.
        /// </summary>
        TypeNoPadded = 0x00000008,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        TypeCopy = 0x00000010,

        /// <summary>
        /// The section contains executable code.
        /// </summary>
        ContentCode = 0x00000020,

        /// <summary>
        /// The section contains initialized data.
        /// </summary>
        ContentInitializedData = 0x00000040,

        /// <summary>
        /// The section contains uninitialized data.
        /// </summary>
        ContentUninitializedData = 0x00000080,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        LinkOther = 0x00000100,

        /// <summary>
        /// The section contains comments or other information. The .drectve section has this type. This is valid for object files only.
        /// </summary>
        LinkInfo = 0x00000200,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        TypeOver = 0x00000400,

        /// <summary>
        /// The section will not become part of the image. This is valid only for object files.
        /// </summary>
        LinkRemove = 0x00000800,

        /// <summary>
        /// The section contains COMDAT data. For more information, see section 5.5.6, COMDAT Sections (Object Only). This is valid only for object files.
        /// </summary>
        LinkComDat = 0x00001000,

        /// <summary>
        /// Reset speculative exceptions handling bits in the TLB entries for this section.
        /// </summary>
        NoDeferSpecExceptions = 0x00004000,

        /// <summary>
        /// The section contains data referenced through the global pointer (GP).
        /// </summary>
        RelativeGP = 0x00008000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemPurgeable = 0x00020000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Memory16Bit = 0x00020000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemoryLocked = 0x00040000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemoryPreload = 0x00080000,

        /// <summary>
        /// Align data on a 1-byte boundary. Valid only for object files.
        /// </summary>
        Align1Bytes = 0x00100000,

        /// <summary>
        /// Align data on a 2-byte boundary. Valid only for object files.
        /// </summary>
        Align2Bytes = 0x00200000,

        /// <summary>
        /// Align data on a 4-byte boundary. Valid only for object files.
        /// </summary>
        Align4Bytes = 0x00300000,

        /// <summary>
        /// Align data on an 8-byte boundary. Valid only for object files.
        /// </summary>
        Align8Bytes = 0x00400000,

        /// <summary>
        /// Align data on a 16-byte boundary. Valid only for object files.
        /// </summary>
        Align16Bytes = 0x00500000,

        /// <summary>
        /// Align data on a 32-byte boundary. Valid only for object files.
        /// </summary>
        Align32Bytes = 0x00600000,

        /// <summary>
        /// Align data on a 64-byte boundary. Valid only for object files.
        /// </summary>
        Align64Bytes = 0x00700000,

        /// <summary>
        /// Align data on a 128-byte boundary. Valid only for object files.
        /// </summary>
        Align128Bytes = 0x00800000,

        /// <summary>
        /// Align data on a 256-byte boundary. Valid only for object files.
        /// </summary>
        Align256Bytes = 0x00900000,

        /// <summary>
        /// Align data on a 512-byte boundary. Valid only for object files.
        /// </summary>
        Align512Bytes = 0x00A00000,

        /// <summary>
        /// Align data on a 1024-byte boundary. Valid only for object files.
        /// </summary>
        Align1024Bytes = 0x00B00000,

        /// <summary>
        /// Align data on a 2048-byte boundary. Valid only for object files.
        /// </summary>
        Align2048Bytes = 0x00C00000,

        /// <summary>
        /// Align data on a 4096-byte boundary. Valid only for object files.
        /// </summary>
        Align4096Bytes = 0x00D00000,

        /// <summary>
        /// Align data on an 8192-byte boundary. Valid only for object files.
        /// </summary>
        Align8192Bytes = 0x00E00000,

        /// <summary>
        /// The section contains extended relocations.
        /// </summary>
        LinkExtendedRelocationOverflow = 0x01000000,

        /// <summary>
        /// The section can be discarded as needed.
        /// </summary>
        MemoryDiscardable = 0x02000000,

        /// <summary>
        /// The section cannot be cached.
        /// </summary>
        MemoryNotCached = 0x04000000,

        /// <summary>
        /// The section is not pageable.
        /// </summary>
        MemoryNotPaged = 0x08000000,

        /// <summary>
        /// The section can be shared in memory.
        /// </summary>
        MemoryShared = 0x10000000,

        /// <summary>
        /// The section can be executed as code.
        /// </summary>
        MemoryExecute = 0x20000000,

        /// <summary>
        /// The section can be read.
        /// </summary>
        MemoryRead = 0x40000000,

        /// <summary>
        /// The section can be written to.
        /// </summary>
        MemoryWrite = 0x80000000
    }

    public enum SubsystemValue : ushort
    {
        /// <summary>
        /// Unknown subsystem.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// No subsystem required (device drivers and native system processes).
        /// </summary>
        Native = 1,

        /// <summary>
        /// Windows graphical user interface (GUI) subsystem.
        /// </summary>
        WindowsGui = 2,

        /// <summary>
        /// Windows character-mode user interface (CUI) subsystem.
        /// </summary>
        WindowsCui = 3,

        /// <summary>
        /// OS/2 CUI subsystem.
        /// </summary>
        OS2Cui = 5,

        /// <summary>
        /// POSIX CE subsystem.
        /// </summary>
        PosixCui = 7,

        /// <summary>
        /// Windows CE system.
        /// </summary>
        WindowsCeGui = 9,

        /// <summary>
        /// Extensible Firmware Interface (EFI) application.
        /// </summary>
        EfiApplication = 10,

        /// <summary>
        /// EFI driver with boot services.
        /// </summary>
        EfiBootServiceDriver = 11,

        /// <summary>
        /// EFI driver with run-time services.
        /// </summary>
        EfiRuntimeDriver = 12,

        /// <summary>
        /// EFI ROM image.
        /// </summary>
        EfiRom = 13,

        /// <summary>
        /// Xbox system.
        /// </summary>
        Xbox = 14,

        /// <summary>
        /// Boot application.
        /// </summary>
        WindowsBootApplication = 16,
    }

    [Flags]
    public enum DllCharacteristicsFlags : ushort
    {
        /// <summary>
        /// Reserved.
        /// </summary>
        Reserved1 = 0x0001,

        /// <summary>
        /// Reserved.
        /// </summary>
        Reserved2 = 0x0002,

        /// <summary>
        /// Reserved.
        /// </summary>
        Reserved3 = 0x0004,

        /// <summary>
        /// Reserved.
        /// </summary>
        Reserved4 = 0x0008,

        /// <summary>
        /// ASLR with 64 bit addess space.
        /// </summary>
        HightEntropyVA = 0x0020,

        /// <summary>
        /// The DLL can be relocated at load time.
        /// </summary>
        DynamicBase = 0x0040,

        /// <summary>
        /// Code integrity checks are forced, if you set this flag and a section contains only uninitialized data, set the <see cref="ImageSectionHeader.PointerToRawData"/> for that section to zero; otherwise, the image will fail to load because the digital signature cannot be verified.
        /// </summary>
        ForceIntegrity = 0x0080,

        /// <summary>
        /// The image is compatible with data execution prevention (DEP).
        /// </summary>
        NxCompatible = 0x0100,

        /// <summary>
        /// The image is isolation aware, but should not be isolated.
        /// </summary>
        NoIsolation = 0x0200,

        /// <summary>
        /// The image does not use structured exception handling (SEH). No handlers can be called in this image.
        /// </summary>
        NoSeh = 0x0400,

        /// <summary>
        /// Do not bind the image.
        /// </summary>
        NoBind = 0x0800,

        /// <summary>
        /// A WDM driver.
        /// </summary>
        WdmDriver = 0x2000,

        /// <summary>
        /// Image supports Control Flow Guard.
        /// </summary>
        GuardCf = 0x4000,

        /// <summary>
        /// The image is terminal server aware.
        /// </summary>
        TerminalServerAware = 0x8000
    }

    public enum FileMachineValue: ushort
    {
        /// <summary>
        /// x86
        /// </summary>
        I386 = 0x014c,
        /// <summary>
        /// Intel Itanium
        /// </summary>
        IA64 = 0x0200,
        /// <summary>
        /// x64
        /// </summary>
        Amd64 = 0x8664,
    }

    [Flags]
    public enum FileCharacteristicFlags : ushort
    {
        /// <summary>
        /// Relocation information was stripped from the file. The file must be loaded at its preferred base address. If the base address is not available, the loader reports an error.
        /// </summary>
        RelocsStripped = 0x0001,
        /// <summary>
        ///  The file is executable (there are no unresolved external references).
        /// </summary>
        ExecutableImage = 0x0002,
        /// <summary>
        ///  COFF line numbers were stripped from the file.
        /// </summary>
        LineNumsStripped = 0x0004,
        /// <summary>
        ///  COFF symbol table entries were stripped from file.
        /// </summary>
        LocalSymsStripped = 0x008,
        /// <summary>
        /// Aggressively trim the working set. This value is obsolete.
        /// </summary>
        AggressiveWsTrim = 0x0010,
        /// <summary>
        /// The application can handle addresses larger than 2 GB.
        /// </summary>
        LargeAddressAware = 0x0020,
        /// <summary>
        ///  The bytes of the word are reversed. This flag is obsolete.
        /// </summary>
        BytesReversedLo = 0x0080,
        /// <summary>
        ///  The computer supports 32-bit words.
        /// </summary>
        Machine32Bit = 0x0100,
        /// <summary>
        ///  Debugging information was removed and stored separately in another file.
        /// </summary>
        DebugStripped = 0x0200,
        /// <summary>
        ///  If the image is on removable media, copy it to and run it from the swap file.
        /// </summary>
        RemovableRunFromSwap = 0x0400,
        /// <summary>
        ///  If the image is on the network, copy it to and run it from the swap file.
        /// </summary>
        NetRunFromSwap = 0x0800,
        /// <summary>
        ///  The image is a system file.
        /// </summary>
        FileSystem = 0x1000,
        /// <summary>
        ///  The image is a DLL file. While it is an executable file, it cannot be run directly.
        /// </summary>
        FileDll = 0x2000,
        /// <summary>
        ///  The file should be run only on a uniprocessor computer.
        /// </summary>
        FileUpSystemOnly = 0x4000,
        /// <summary>
        ///  The bytes of the word are reversed. This flag is obsolete.
        /// </summary>
        FileBytesReveredHi = 0x8000,
    }
}

#pragma warning restore CS1591 // Variable never initialized
