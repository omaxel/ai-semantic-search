using Pgvector;
using SmartComponents.LocalEmbeddings;

namespace ExperimentsSemanticSearch.Images.Persistence.Entities;

public class ImageDocument
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public byte[] ImageData { get; set; } = null!;
    public EmbeddingF32 Embedding { get; set; }
    public byte[] EmbeddingBuffer { get; set; } = null!;
    public Vector EmbeddingVector { get; set; } = null!;
}
