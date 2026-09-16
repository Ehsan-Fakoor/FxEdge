using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FxEdge.DataAccess;

/// <summary>
/// Lets EF Core CLI tools (dotnet ef migrations add / database update) create an
/// FxEdgeDbContext without running the full Host application. Only used at design
/// time - the running app always gets its DbContext through
/// AddFxEdgeDataAccess -> DI, using the real connection string from appsettings.json.
/// The connection string below is only used while generating migrations; it has no
/// effect at runtime.
/// </summary>
public sealed class FxEdgeDbContextDesignTimeFactory : IDesignTimeDbContextFactory<FxEdgeDbContext>
{
    public FxEdgeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FxEdgeDbContext>();
        optionsBuilder.UseSqlite("Data Source=fxedge.db");
        return new FxEdgeDbContext(optionsBuilder.Options);
    }
}
