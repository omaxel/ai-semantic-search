using System.Diagnostics;
using System.Globalization;
using ExperimentsSemanticSearch.Documents.Persistence;
using ExperimentsSemanticSearch.Documents.Persistence.Entities;
using ExperimentsSemanticSearch.Documents.Seed;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SmartComponents.LocalEmbeddings;
using Spectre.Console;

var dbContext = new AppDbContextFactory().CreateDbContext([]);

Console.WriteLine("Migrate database!");
await dbContext.Migrate();
await new DocumentSeed(dbContext).Seed();
Console.WriteLine("Seed completed");

using var documentEmbedder = new LocalEmbedder();

var runDocumentsImportEmbeddings = AnsiConsole.Prompt(
    new TextPrompt<bool>("Run documents embeddings?")
        .AddChoice(true)
        .AddChoice(false)
        .DefaultValue(false)
        .WithConverter(choice => choice ? "y" : "n"));

if (runDocumentsImportEmbeddings) await ImportDocumentEmbeddings(dbContext, documentEmbedder);

await SearchDocumentInDatabase(dbContext, documentEmbedder);

static async Task SearchDocumentInDatabase(AppDbContext appDbContext, LocalEmbedder embedder)
{
    do
    {
        var prompt = AnsiConsole.Ask<string>("Enter text to search:");
        AnsiConsole.WriteLine();

        var query = embedder.Embed(prompt);
        var vectorToCompare = new Vector(query.Values);

        var stopwatch = Stopwatch.StartNew();

        var queryable = appDbContext.Set<Document>()
            .Select(item => new { item, distance = item.EmbeddingVector!.CosineDistance(vectorToCompare) })
            .OrderBy(item => item.distance)
            .Take(5);
        var matches = await queryable.ToListAsync();

        stopwatch.Stop();

        var results = matches.Select(arg => new SimilarityScore<Document>(1 - (float)arg.distance, arg.item)).ToArray();
        RenderDocumentsResults(stopwatch, results);
    } while (true);
}

static async Task ImportDocumentEmbeddings(AppDbContext appDbContext, LocalEmbedder embedder)
{
    AnsiConsole.WriteLine("Importing embeddings");
    var documents = await appDbContext.Set<Document>().ToListAsync();

    var stopwatch = Stopwatch.StartNew();

    foreach (var document in documents)
    {
        var embedding = embedder.Embed($"{document.Title} --- {document.Body}");

        //TODO: check if used
        document.Embedding = embedding;
        document.EmbeddingBuffer = embedding.Buffer.ToArray();
        document.EmbeddingVector = new Vector(embedding.Values);
    }

    stopwatch.Stop();
    AnsiConsole.WriteLine("Generated {0} embeddings in {1} ms", documents.Count, stopwatch.ElapsedMilliseconds);

    await appDbContext.SaveChangesAsync();
    AnsiConsole.WriteLine("Saved embeddings to database");
}

static void RenderDocumentsResults(Stopwatch stopwatch, SimilarityScore<Document>[] results)
{
    var table = new Table();

    table
        .AddColumn("Score")
        .AddColumn("Id")
        .AddColumn("Lang")
        .AddColumn("Result",
            column => column.Footer($"[Green]Search time: {stopwatch.ElapsedMilliseconds} Milliseconds[/]"));

    foreach (var item in results)
    {
        table.AddRow(item.Similarity.ToString(CultureInfo.CurrentCulture), item.Item.Id.ToString(),
            item.Item.CultureInfo ?? string.Empty, item.Item.Title);
    }

    AnsiConsole.WriteLine("Documents results");
    AnsiConsole.Write(table);
}
