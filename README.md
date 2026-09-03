# Calabonga.PagedListCore.Json

Extension for package `Calabonga.PagedListCore` that is implementation of pagination for .NET Core (netstandard2.1). Nuget [Calabonga.PagedListCore.Json](https://www.nuget.org/packages/Calabonga.PagedListCore.Json)

## v3.0.1

* Fixed: `PageListConverter<T>.Read` threw `ArgumentOutOfRangeException` when `pageIndex` was
  missing from the JSON — it now defaults to `1` (`PagedList<T>` is 1-based)
* Added unit test project `Calabonga.PagedListCore.Json.Tests` (xUnit); CI now runs `dotnet test`
  before packing

## v3.0.0

* Dependency `Calabonga.PagedListCore` updated to `3.0.0`
* Dependency `System.Net.Http.Json` updated to `10.0.11`
* `PageListConverter<T>` migrated to file-scoped namespace; `LangVersion` set to `latest`

## v2.0.0

* Dependency `Calabonga.PagedListCore` updated to `2.0.0`
* Target framework moved to `netstandard2.1`
* Solution migrated to `.slnx` format; CI pipeline updated to .NET 10 SDK

## v1.0.0

* Converter created for `System.Text.Json` deserialization
* First release.
