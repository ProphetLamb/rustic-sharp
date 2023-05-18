using System.IO;

namespace Rustic.Native;

/// <summary>Extensions for <see cref="BinaryReader"/>.</summary>
public static class BinaryReaderExtensions {
    ///<inheritdoc cref="TypeMarshal.FromStreamStruct{T}"/>
    public static T ReadStruct<T>(this BinaryReader reader)
        where T : struct {
        return TypeMarshal.FromStreamStruct<T>(reader.BaseStream);
    }

    ///<inheritdoc cref="TypeMarshal.FromStreamBlittable{T}"/>
    public static T ReadBlittable<T>(this BinaryReader reader)
        where T : class, new() {
        return TypeMarshal.FromStreamBlittable<T>(reader.BaseStream);
    }
}
