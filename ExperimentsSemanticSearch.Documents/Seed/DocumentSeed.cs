using ExperimentsSemanticSearch.Documents.Persistence;
using ExperimentsSemanticSearch.Documents.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExperimentsSemanticSearch.Documents.Seed;

public class DocumentSeed(AppDbContext dbContext)
{
    public async Task Seed(CancellationToken cancellationToken = default)
    {
        if (await dbContext.Set<Document>().AnyAsync(cancellationToken))
            return;

        var seedSqlFile = Path.Combine(AppContext.BaseDirectory, "Seed", "document.sql");

        if (!System.IO.File.Exists(seedSqlFile))
            throw new Exception($"document.sql file not found: {seedSqlFile}");

        await dbContext.Database.ExecuteSqlRawAsync(await System.IO.File.ReadAllTextAsync(seedSqlFile, cancellationToken), cancellationToken);
    }
}
