using ExperimentsSemanticSearch.Documents.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExperimentsSemanticSearch.Documents.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Document>().ToTable("Document");
        modelBuilder.Entity<Document>().Ignore(item => item.Embedding);
        modelBuilder.Entity<Document>().Ignore(item => item.EmbeddingBuffer);
        modelBuilder.Entity<Document>().Property(item => item.EmbeddingVector).HasColumnType("vector(384)");
        modelBuilder.Entity<Document>()
            .HasIndex(i => i.EmbeddingVector)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }

    public async Task Migrate(CancellationToken cancellationToken = default)
    {
        await Database.EnsureCreatedAsync(cancellationToken);
    }
}
