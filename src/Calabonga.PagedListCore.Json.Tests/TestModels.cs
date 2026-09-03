using Calabonga.PagedListCore;

namespace Calabonga.PagedListCore.Json.Tests;

/// <summary>
/// Simple DTO used to exercise the converter with a reference type.
/// </summary>
public sealed record Product(int Id, string Name, decimal Price);

/// <summary>
/// DTO with a nested collection, to check that the inner element converter is invoked recursively.
/// </summary>
public sealed record Order(int Number, Product[] Lines);

/// <summary>
/// Envelope that carries an <see cref="IPagedList{T}"/> as a property, mirroring a typical API response.
/// </summary>
public sealed record ProductsResponse(IPagedList<Product> Data);
