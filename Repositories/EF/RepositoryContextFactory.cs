using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Repositories.EF;

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    // configurations ve configurations.json yüklemeli
    public RepositoryContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<RepositoryContext>()
            .UseMySql(
                configuration.GetConnectionString("sqlConnection"),
                new MySqlServerVersion(new Version(10, 4, 28)),
                prj => prj.MigrationsAssembly("MVC")
            );
        return new RepositoryContext(builder.Options);
    }
}