# Rustic

# Common

```powershell
dotnet add package Rustic.Common
```

**Namespace Rustic.Common**

| Definition                 | Description                                                                                                                                                                                |
| -------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `ref struct StrBuilder`    | `ValueStringBuilder` from [ValueStringBuilder: a stack-based string-builder](https://andrewlock.net/a-deep-dive-on-stringbuilder-part-6-vaulestringbuilder-a-stack-based-string-builder/). |
| `static class ThrowHelper` | Centralized functionality related to validation and throwing exceptions.                                                                                                                   |
| `static class Arithmetic`  | Collection of extension methods and utility functions related to integer arithmetic. Most functions are ported from [bithacks](https://graphics.stanford.edu/~seander/bithacks.html)       |

# Memory

```powershell
dotnet add package Rustic.Memory
```

**Namespace Rustic.Memory**

| Definition                     | Description                                                     |
| ------------------------------ | --------------------------------------------------------------- |
| `struct TinyVec`               | Read-only list with a inline capacity of 4. Also see `TinySpan` |
| `readonly ref struct TinySpan` | Partially inlined immutable collection of function parameters.  |
| `readonly ref struct BitSet`   | Enables unaligned marking of bits in a memory area.             |
| `readonly ref struct BitVec`   | Partially inlined immutable collection of function parameters.  |

## Vector
**Namespace Rustic.Memory.Vector**

This package mainly contains the list and buffer implementations

| Definition          | Description                                                                     |
| ------------------- | ------------------------------------------------------------------------------- |
| `class Vec`         | `System.Collections.Generic.List`-like implementation allowing by-`ref` access. |
| `ref struct RefVec` | Temporary array allocating from `ArrayPool<T>.Default`.                         |

that aim to optimize memory management, by

- returning `struct`s by-`ref` or -`readonly ref` on access,
- optimizing for temporary array usage by providing implementations allowing allocation from a `ArrayPool<T>`,
- and inlining tiny arrays.

## IO
**Namespace Rustic.Memory.IO**

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

# Text

```powershell
dotnet add package Rustic.Text
```

**Namespace Rustic.Text**

In addition to that the package provides the following non-allocating helpers for parsing text and more

| Definition             | Description                                                                                                                         |
| ---------------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| `ref struct SplitIter` | Iterator slicing a sequence of elements into smaller sequences, by a set of separators. Analogous in function to `string.Split`     |
| `ref struct Tokenizer` | Allows scanning and navigating a sequence of elements on a per-iteration basis. Inspired by `Utf8JsonReader` but much more generic. |
