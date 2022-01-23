# HeaplessUtility

## Installing

```powershell
dotnet add package HeaplessUtility
```
## Vector
**Namespace HeaplessUtility**

This package mainly contains the list and buffer implementations

| Definition          | Description                                                                         |
| ------------------- | ----------------------------------------------------------------------------------- |
| `class Vec`         | `System.Collections.Generic.List`-like implementation allowing by-`ref` access.     |
| `ref struct RefVec` | Temporary array allocating from `ArrayPool<T>.Default`.                             |
| `class PoolVec`     | `Vec` implementation allocating from a `ArrayPool<T>` instance.                     |
| `struct TinyVec`    | Read-only list with a inline capacity of 4. Also see `readonly ref struct TinySpan` |

that aim to optimize memory management, by

- returning `struct`s by-`ref` or -`readonly ref` on access, 
- optimizing for temporary array usage by providing implementations allowing allocation from a `ArrayPool<T>`,
- and inlining tiny arrays.

## IO
**Namespace HeaplessUtility.IO**

For constructing or writing sequences of data an array is often required. Therefore, a `List<T>` has to be copied to the required layout, to circumvent this `BufWriter` allows moving the reference of the internal array to user control safely.

| Definition            | Description                           |
| --------------------- | ------------------------------------- |
| `class BufWriter`     | `IBufferWriter` similar to `Vec`.     |
| `class PoolBufWriter` | `IBufferWriter` similar to `PoolVec`. |

```csharp

var obj = [...]
PoolBufWriter<byte> writer = new();
Serializer.Serialize(writer, obj);
DoWork(writer.ToSpan(out byte[] poolArray));
ArrayPool<byte>.Return(poolArray);
```

-- or --

```csharp
var obj = [...]
PoolBufWriter<byte> writer = new();
Serializer.Serialize(writer, obj);
return writer.ToArray(dispose: true);
```

## Text
**Namespace HeaplessUtility.Text**

In addition to that `HeaplessUtility` provides the following non-allocating helpers for parsing text and more

| Definition              | Description                                                                                                                                                                                |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `ref struct StrBuilder` | `ValueStringBuilder` from [ValueStringBuilder: a stack-based string-builder](https://andrewlock.net/a-deep-dive-on-stringbuilder-part-6-vaulestringbuilder-a-stack-based-string-builder/). |
| `ref struct SplitIter`  | Iterator slicing a sequence of elements into smaller sequences, by a set of separators. Analogous in function to `string.Split`                                                            |
| `ref struct Tokenizer`  | Allows scanning and navigating a sequence of elements on a per-iteration basis. Inspired by `Utf8JsonReader` but much more generic.                                                        |

## Bithacks
**Namespace HeaplessUtility**

`BitHelper` can replace a `List<bool>` in most use cases, when calculating set unions an similar operations where marking entries in necessary. In addition to that multiple bit-hacks are implemented `static`ally, most notable `FastModulaMultiplier` & `FastModulo`, `Log2`, `Log10` and `Negate`.

`BitMarker` is more or less a duplicate I wrote for another project a notable difference is that it is a `readonly ref struct`.
