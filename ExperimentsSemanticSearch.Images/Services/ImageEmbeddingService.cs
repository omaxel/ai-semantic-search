using System.Net.Http.Json;
using SmartComponents.LocalEmbeddings;

namespace ExperimentsSemanticSearch.Images.Services;

public static class ImageEmbeddingService
{
    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:8000")
    };

    public static async Task ImportImagesEmbeddings()
    {
        await _httpClient.GetAsync("/import-images-embeddings");
    }

    public static async Task<ReadOnlyMemory<float>> GetTextEmbedding(string text)
    {
        var response = await _httpClient.GetFromJsonAsync<EmbedTextResponse>("/embed?text=" + text);

        var bufferByteLength = EmbeddingF32.GetBufferByteLength(response.Embeds.Length);
        var buffer = new Memory<byte>(new byte[bufferByteLength]);
        var query = EmbeddingF32.FromModelOutput(response.Embeds, buffer);
        return query.Values;
    }
}

public class EmbedTextResponse
{
    public float[] Embeds { get; set; } = [];
}
