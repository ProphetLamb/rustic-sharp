# Rustic

## [Releasenotes](RELEASENOTES.md)

## Packages

| Name                                                  | Description                                                                                               | Nuget                                                                                                                                                                |
|-------------------------------------------------------|-----------------------------------------------------------------------------------------------------------| -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [`Rustic.Attributes`](doc/Rustic.Attributes/index.md) | Attributes not delivered with the mscorelib.                                                              | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Attributes)](https://www.nuget.org/packages/Rustic.Attributes/) <br/> `dotnet add package Rustic.Common`        |
| [`Rustic.Common`](doc/Rustic.Common/index.md)         | Packages types and functionality usable across most projects.                                             | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Common)](https://www.nuget.org/packages/Rustic.Common/) <br/> `dotnet add package Rustic.Common`                |
| ~~`Rustic.DataEnumGen`~~                              | Deprecated in favour of [.variant](https://github.com/mknejp/dotvariant)                                  | [![NuGet Badge](https://buildstats.info/nuget/Rustic.DataEnumGen)](https://www.nuget.org/packages/Rustic.DataEnumGen/) <br/> `dotnet add package Rustic.DataEnumGen` |
| ~~`Rustic.Functional`~~                               | Deprecated in favour of [c#functionalextensions](https://github.com/vkhorikov/CSharpFunctionalExtensions) | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Functional)](https://www.nuget.org/packages/Rustic.Functional/) <br/> `dotnet add package Rustic.Functional`    |
| [`Rustic.Json`](doc/Rustic.Json/index.md)             | Json converters &amp; reader, writer utility functionality.                                               | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Json)](https://www.nuget.org/packages/Rustic.Json/) <br/> `dotnet add package Rustic.Json`                      |
| [`Rustic.Memory`](doc/Rustic.Memory/index.md)         | Commonly used collection implementation allowing by-ref access to the storage.                            | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Memory)](https://www.nuget.org/packages/Rustic.Memory/) <br/> `dotnet add package Rustic.Memory`                |
| [`Rustic.Memory.Bit`](doc/Rustic.Memory.Bit/index.md) | Packed bit collections. Storing sequences of boolean values in integers.                                  | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Memory.Bit)](https://www.nuget.org/packages/Rustic.Memory.Bit/) <br/> `dotnet add package Rustic.Memory.Bit`    |
| [`Rustic.Reflect`](doc/Rustic.Reflect/index.md)       | Il based delegate emitter and commonly used reflection utility.                                           | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Reflect)](https://www.nuget.org/packages/Rustic.Reflect/) <br/> `dotnet add package Rustic.Reflect`             |
| [`Rustic.Source`](doc/Rustic.Source/index.md)         | Packages types and functionality related to extending source generators.                                  | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Source)](https://www.nuget.org/packages/Rustic.Source/) <br/> `dotnet add package Rustic.Source`                |
| [`Rustic.Text`](doc/Rustic.Text/index.md)             | Types and extensions improving string access and formatting functionality.                                | [![NuGet Badge](https://buildstats.info/nuget/Rustic.Text)](https://www.nuget.org/packages/Rustic.Text/) <br/> `dotnet add package Rustic.Source`                    |

## Continues Integration

| Build                                                                                                                                                                                              | Test                                                                                                                                                                          | Coverage                                                                                                                                                                                      | Code quality                                                                                                                                                                                                                                                                                       |
| -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| <sup>Appveyor</sup> [![Build status](https://ci.appveyor.com/api/projects/status/26phbh7xqhxet8fn/branch/master?svg=true)](https://ci.appveyor.com/project/ProphetLamb/rustic-sharp/branch/master) | <sup>Appveyor</sup> [![AppVeyor tests](https://img.shields.io/appveyor/tests/ProphetLamb/rustic-sharp)](https://ci.appveyor.com/project/ProphetLamb/rustic-sharp/build/tests) | <sup>Coveralls</sup> [![Coverage Status](https://coveralls.io/repos/github/ProphetLamb/rustic-sharp/badge.svg?branch=HEAD)](https://coveralls.io/github/ProphetLamb/rustic-sharp?branch=HEAD) | <sup>Codacy</sup> [![Codacy Badge](https://app.codacy.com/project/badge/Grade/316ddf1a416949c290607666c875b861)](https://www.codacy.com/gh/ProphetLamb/rustic-sharp/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ProphetLamb/rustic-sharp&amp;utm_campaign=Badge_Grade) |
| ![Build history](https://buildstats.info/appveyor/chart/ProphetLamb/rustic-sharp/?branch=master)                                                                                                   |                                                                                                                                                                               |                                                                                                                                                                                               |                                                                                                                                                                                                                                                                                                    |

## Source acknowledgements

| Source                                                                                                                                                          | License |
| --------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| [.Net Foundation](https://dotnetfoundation.org/)                                                                                                                | `MIT`   |
| [ValueStringBuilder: a stack-based string-builder](https://andrewlock.net/a-deep-dive-on-stringbuilder-part-6-vaulestringbuilder-a-stack-based-string-builder/) |         |
| [bithacks](https://graphics.stanford.edu/~seander/bithacks.html)                                                                                                |         |
| [`Fast.Reflection`](https://github.com/vexe/Fast.Reflection)                                                                                                    | `MIT`   |
