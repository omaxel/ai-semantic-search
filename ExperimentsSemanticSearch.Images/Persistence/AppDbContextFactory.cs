using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExperimentsSemanticSearch.Images.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"), opt => opt.UseVector());
        
        //optionsBuilder.EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information);
    
        return new AppDbContext(optionsBuilder.Options);
    }
}
