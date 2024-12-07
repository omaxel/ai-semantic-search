using System.Diagnostics;
using System.Globalization;
using ExperimentsSemanticSearch.Images.Persistence;
using ExperimentsSemanticSearch.Images.Persistence.Entities;
using ExperimentsSemanticSearch.Images.Services;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SmartComponents.LocalEmbeddings;
using Spectre.Console;

var dbContext = new AppDbContextFactory().CreateDbContext([]);

Console.WriteLine("Migrate database!");
await dbContext.Migrate();
Console.WriteLine("Migration completed");

var runImagesImportEmbeddings = AnsiConsole.Prompt(
    new TextPrompt<bool>("Run images embeddings?")
        .AddChoice(true)
        .AddChoice(false)
        .DefaultValue(false)
        .WithConverter(choice => choice ? "y" : "n"));

if (runImagesImportEmbeddings)
{
    AnsiConsole.WriteLine("Importing embeddings");
    var stopwatch = Stopwatch.StartNew();
    
    await ImageEmbeddingService.ImportImagesEmbeddings();
    AnsiConsole.WriteLine("Generated embeddings in {0} ms", stopwatch.ElapsedMilliseconds);
    AnsiConsole.WriteLine("Saved embeddings to database");
}

await SearchImagesInDatabase(dbContext);

static async Task SearchImagesInDatabase(AppDbContext appDbContext)
{
    do
    {
        var prompt = AnsiConsole.Ask<string>("Enter text to search:");
        AnsiConsole.WriteLine();
        var stopwatch = Stopwatch.StartNew();

        var queryValues = await ImageEmbeddingService.GetTextEmbedding(prompt);

        var vectorToCompare = new Vector(queryValues);


        var queryable = appDbContext.Set<ImageDocument>()
            .Select(item => new { item, distance = item.EmbeddingVector!.CosineDistance(vectorToCompare) })
            .OrderBy(item => item.distance)
            .Take(5);
        var matches = await queryable.ToListAsync();

        stopwatch.Stop();

        var results = matches.Select(arg => new SimilarityScore<ImageDocument>(1 - (float)arg.distance, arg.item))
            .ToArray();
        RenderImagesResults(stopwatch, results);
    } while (true);
}

static void RenderImagesResults(Stopwatch stopwatch, SimilarityScore<ImageDocument>[] results)
{
    var table = new Table();

    table
        .AddColumn("Score")
        .AddColumn("Id")
        .AddColumn("File name",
            column => column.Footer($"[Green]Search time: {stopwatch.ElapsedMilliseconds} Milliseconds[/]"));

    foreach (var item in results)
    {
        var filePath = Path.Combine(@"C:\dev\experiments-semantic-search-dotnet-ef-core\images-embedder\images", item.Item.Title);
        table.AddRow(
            new Text(item.Similarity.ToString(CultureInfo.CurrentCulture)),
            new Text(item.Item.Id.ToString()),
            new Markup($"[link={filePath}]{item.Item.Title}[/]")
        );
    }

    AnsiConsole.WriteLine("Images results");
    AnsiConsole.Write(table);
}
