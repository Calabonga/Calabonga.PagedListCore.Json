using System.Text.Json;
using Calabonga.PagedListCore;

namespace Calabonga.PagedListCore.Json.Tests;

public class PageListConverterReadTests
{
    private static JsonSerializerOptions OptionsFor<T>()
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new PageListConverter<T>());
        return options;
    }

    private static IPagedList<T>? Deserialize<T>(string json)
        => JsonSerializer.Deserialize<IPagedList<T>>(json, OptionsFor<T>());

    [Fact]
    public void Read_ParsesMetadataAndItems_ForReferenceType()
    {
        const string json = """
        {
          "pageIndex": 2,
          "pageSize": 10,
          "totalCount": 25,
          "items": [
            { "id": 11, "name": "Keyboard", "price": 49.90 },
            { "id": 12, "name": "Mouse", "price": 19.50 }
          ]
        }
        """;

        var result = Deserialize<Product>(json)!;

        Assert.Equal(2, result.PageIndex);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Collection(
            result.Items,
            p => Assert.Equal(new Product(11, "Keyboard", 49.90m), p),
            p => Assert.Equal(new Product(12, "Mouse", 19.50m), p));
    }

    [Fact]
    public void Read_ParsesPrimitiveItems()
    {
        const string json = """{ "pageIndex": 1, "pageSize": 3, "totalCount": 9, "items": [1, 2, 3] }""";

        var result = Deserialize<int>(json)!;

        Assert.Equal([1, 2, 3], result.Items);
        Assert.Equal(9, result.TotalCount);
    }

    [Fact]
    public void Read_ParsesStringItems()
    {
        const string json = """{ "pageIndex": 1, "pageSize": 2, "totalCount": 2, "items": ["a", "b"] }""";

        var result = Deserialize<string>(json)!;

        Assert.Equal(["a", "b"], result.Items);
    }

    [Fact]
    public void Read_IsIndependentOfPropertyOrder()
    {
        const string json = """
        {
          "items": [10, 20],
          "totalCount": 8,
          "pageSize": 5,
          "pageIndex": 2
        }
        """;

        var result = Deserialize<int>(json)!;

        Assert.Equal(2, result.PageIndex);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(8, result.TotalCount);
        Assert.Equal([10, 20], result.Items);
    }

    [Fact]
    public void Read_HandlesEmptyItemsArray()
    {
        const string json = """{ "pageIndex": 1, "pageSize": 10, "totalCount": 0, "items": [] }""";

        var result = Deserialize<Product>(json)!;

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public void Read_IgnoresUnknownScalarProperties()
    {
        const string json = """
        {
          "pageIndex": 1,
          "extra": "ignored",
          "pageSize": 5,
          "flagged": true,
          "totalCount": 3,
          "items": [1, 2, 3]
        }
        """;

        var result = Deserialize<int>(json)!;

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal([1, 2, 3], result.Items);
    }

    [Fact]
    public void Read_InvokesElementConverterRecursively_ForNestedCollections()
    {
        const string json = """
        {
          "pageIndex": 1,
          "pageSize": 10,
          "totalCount": 1,
          "items": [
            {
              "number": 500,
              "lines": [
                { "id": 1, "name": "Cable", "price": 5.00 },
                { "id": 2, "name": "Adapter", "price": 8.25 }
              ]
            }
          ]
        }
        """;

        var result = Deserialize<Order>(json)!;

        var order = Assert.Single(result.Items);
        Assert.Equal(500, order.Number);
        Assert.Equal(
            [new Product(1, "Cable", 5.00m), new Product(2, "Adapter", 8.25m)],
            order.Lines);
    }

    [Theory]
    [InlineData(1, false, true)]
    [InlineData(2, true, true)]
    [InlineData(3, true, false)]
    public void Read_PropagatesPaginationFlags(int pageIndex, bool hasPrevious, bool hasNext)
    {
        var json = $$"""
        { "pageIndex": {{pageIndex}}, "pageSize": 10, "totalCount": 25, "items": [1] }
        """;

        var result = Deserialize<int>(json)!;

        Assert.Equal(pageIndex, result.PageIndex);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(hasPrevious, result.HasPreviousPage);
        Assert.Equal(hasNext, result.HasNextPage);
    }

    [Fact]
    public void Read_WorksWhenPagedListIsANestedProperty()
    {
        const string json = """
        {
          "data": {
            "pageIndex": 1,
            "pageSize": 10,
            "totalCount": 1,
            "items": [ { "id": 7, "name": "Lamp", "price": 12.0 } ]
          }
        }
        """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new PageListConverter<Product>());

        var response = JsonSerializer.Deserialize<ProductsResponse>(json, options)!;

        var product = Assert.Single(response.Data.Items);
        Assert.Equal(new Product(7, "Lamp", 12.0m), product);
    }

    [Fact]
    public void Read_KeyMatchingIsCaseSensitive()
    {
        // "itemS" does not match the "items" case in the converter, so the array is never read.
        const string json = """
        { "pageIndex": 2, "pageSize": 10, "totalCount": 25, "itemS": [1, 2, 3] }
        """;

        var result = Deserialize<int>(json)!;

        Assert.Empty(result.Items);
        Assert.Equal(2, result.PageIndex);
    }

    [Fact]
    public void Read_ReturnsNull_ForJsonNull()
    {
        var result = Deserialize<Product>("null");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("[1, 2, 3]")]
    [InlineData("42")]
    [InlineData("\"text\"")]
    public void Read_Throws_WhenRootIsNotAnObject(string json)
        => Assert.Throws<JsonException>(() => Deserialize<int>(json));

    [Theory]
    [InlineData("""{ "pageIndex": 1, "pageSize": 10, "totalCount": 1, "items": null }""")]
    [InlineData("""{ "pageIndex": 1, "pageSize": 10, "totalCount": 1, "items": 123 }""")]
    public void Read_Throws_WhenItemsIsNotAnArray(string json)
        => Assert.Throws<JsonException>(() => Deserialize<int>(json));

    [Fact]
    public void Read_DefaultsPageIndexToOne_WhenOmitted()
    {
        // PagedList<T> is 1-based and rejects an index below 1, so an absent "pageIndex"
        // must fall back to the first page.
        const string json = """{ "pageSize": 10, "totalCount": 25, "items": [1] }""";

        var result = Deserialize<int>(json)!;

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Read_Throws_WhenPageIndexIsExplicitlyLessThanOne(int pageIndex)
    {
        // An explicit out-of-range value is malformed input: fail fast rather than silently coerce it.
        var json = $$"""{ "pageIndex": {{pageIndex}}, "pageSize": 10, "totalCount": 25, "items": [1] }""";

        Assert.Throws<ArgumentOutOfRangeException>(() => Deserialize<int>(json));
    }
}
