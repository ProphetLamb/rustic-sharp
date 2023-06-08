# Version 0.6.4

- Attributes: Make IsExternalInit public

# Version 0.6.3

- Native: Add PEHeader
- Native: Add read classes and value-types from memory
- Attributes: Add IsExternalInit

# Version 0.6.2

- Memory.Specialized: Add ImmutableOrderedDictionary
- Memory.Specialized: Improve MultiDictionary usability
- Text: Fix issued caused by defensive shadow copies

# Version 0.6.1

- Target .NET Core App 3.1
- Target .NET 7.0
- Memory.Specialized: Fix MultiDictionary implementation

# Version 0.6.0

- Attributes: Fix invalid preprocessor directives for certain target frameworks.
- Common: Add UnreachableException
- Common: Return Random.Shared for .NET 6.0 or greater instead of thread static instance.
- Common: Update backport of HashCode implementation.
- Source: Add documentation
- Text: Fix various bugs in Formatter
- Text: Fix various bugs in Tokenizer
- Text: Add GetAtCursor and GetAddPosition methods to Tokenizer
- Text: Fix ref an in keyword related compiler errors introduced in C#11
- Memory.Common: Add CopyToReversed to MemoryCopyHelper
- Memory.Common: Merge Rustic.Memory.Common namespace into Rustic.Memory
- Memory.Common: Mark classes as sealed when possible
- Memory.Buffers: Mark classes as sealed when possible
- Memory.Inline: Add TinyVec list implementation
- Memory.Inline: Add implicit from tuple conversion
- Memory.Inline: Optimize ToSpan performance for newer framework versions
- Memory.Inline: Rename TinyRoSpan and TinyRoVec
- Memory.Inline: Fix ref an in keyword related compiler errors introduced in C#11
- Memory.Inline: Fix bug preferring inline data when array is backing
- Memory.Specialized: Initial release
- Memory.Specialized: Add ReversibleIndexSpan
- Memory.Specialized: Add MultiDictionary
- Memory.Vec: Mark classes as sealed when possible
