using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class MovieDbContextFactory
    : IDesignTimeDbContextFactory<MovieDbContext>
{
    public MovieDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MovieDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=.;Database=MovieDb;Trusted_Connection=True;TrustServerCertificate=True"); 

        return new MovieDbContext(optionsBuilder.Options);
    }
}