using System.Text.Json;
using Calabonga.PagedListCore;

namespace Calabonga.PagedListCore.Json.Tests;

public class PageListConverterWriteTests
{
    [Fact]
    public void Write_IsNotSupported()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new PageListConverter<int>());

        IPagedList<int> paged = new PagedList<int>(new[] { 1, 2, 3 }, 1, 10, 3);

        Assert.Throws<NotImplementedException>(
            () => JsonSerializer.Serialize(paged, options));
    }
}
