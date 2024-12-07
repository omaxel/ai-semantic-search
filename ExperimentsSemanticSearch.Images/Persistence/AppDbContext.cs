using ExperimentsSemanticSearch.Images.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExperimentsSemanticSearch.Images.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<ImageDocument>().ToTable("ImageDocument");
        modelBuilder.Entity<ImageDocument>().Ignore(item => item.Embedding);
        modelBuilder.Entity<ImageDocument>().Ignore(item => item.EmbeddingBuffer);
        modelBuilder.Entity<ImageDocument>().Property(item => item.EmbeddingVector).HasColumnType("vector(512)");
        modelBuilder.Entity<ImageDocument>()
            .HasIndex(i => i.EmbeddingVector)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }

    public async Task Migrate(CancellationToken cancellationToken = default)
    {
        await Database.EnsureCreatedAsync(cancellationToken);
    }

}
