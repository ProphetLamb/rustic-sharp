using System.IO;

namespace Rustic.Native;

/// <summary>Extensions for <see cref="BinaryReader"/>.</summary>
public static class BinaryReaderExtensions {
    ///<inheritdoc cref="Types.FromStreamStruct{T}"/>
    public static T ReadStruct<T>(this BinaryReader reader)
        where T : struct {
        return Types.FromStreamStruct<T>(reader.BaseStream);
    }

    ///<inheritdoc cref="Types.FromStreamBlittable{T}"/>
    public static T ReadBlittable<T>(this BinaryReader reader)
        where T : class, new() {
        return Types.FromStreamBlittable<T>(reader.BaseStream);
    }
}
