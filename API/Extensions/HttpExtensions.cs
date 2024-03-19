using System.Text.Json;
using API.Helpers;

namespace API.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
    {
        var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

        // Attach custom headers
        response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));

        // Expose the custom header
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}
