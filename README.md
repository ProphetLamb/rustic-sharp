# HeaplessUtility

This package mainly contains the list and buffer implementations

| Definition            | Desciption                                                                      |
| --------------------- | ------------------------------------------------------------------------------- |
| `ref struct RefVec`   | Temporary array allocating from `ArrayPool<T>.Default`.                         |
| `class Vec`           | `System.Collections.Generic.List`-like implementation allowing by-`ref` access. |
| `class PoolVec`       | `Vec` implementation allocating from a `ArrayPool<T>` instance.                 |
| `class BufWriter`     | `IBufferWriter` similar to `Vec`.                                               |
| `class PoolBufWriter` | `IBufferWriter` similar to `PoolVec`.                                           |

that aim to optimize memory management, by

- returning `struct`s by-`ref` or -`readonly ref` on access, 
- and optimizing for temporary array usage by providing implementations allowing allocation from a `ArrayPool<T>`.

In addition to that `HeaplessUtility` provides the non-allocating helpers for parsing

| Definition              | Description                                                                                                                                                                                |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `ref struct StrBuilder` | `ValueStringBuilder` from [ValueStringBuilder: a stack-based string-builder](https://andrewlock.net/a-deep-dive-on-stringbuilder-part-6-vaulestringbuilder-a-stack-based-string-builder/). |
| `ref struct SplitIter`  | Iterator slicing a sequence of elements into smaller sequences, by a set of separators. Analogous in function to `string.Split`                                                            |
| `ref struct Tokenizer`  | Allows scanning and navigating a sequence of elements on a per-iteration basis. Inspired by `Utf8JsonReader` but much more generic.                                                        |
