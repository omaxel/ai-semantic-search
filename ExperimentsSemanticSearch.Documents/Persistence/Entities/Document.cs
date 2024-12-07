using Pgvector;
using SmartComponents.LocalEmbeddings;

namespace ExperimentsSemanticSearch.Documents.Persistence.Entities;

public class Document
{
    
    public int Id { get; set; }

    public string? CultureInfo { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }
    
    public EmbeddingF32 Embedding { get; set; }

    public byte[] EmbeddingBuffer { get; set; }

    public Vector? EmbeddingVector { get; set; }
}