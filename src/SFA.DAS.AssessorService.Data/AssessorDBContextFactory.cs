namespace SFA.DAS.AssessorService.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    /// <summary>
    /// Used for design time migration generation and database update only.
    /// </summary>
    public class AssessorDBContextFactory : IDesignTimeDbContextFactory<AssessorDbContext>
    {
        public AssessorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AssessorDbContext>();
            // We'll have to find a way to either inject the connection string into here or grab it.
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=AssessorDB;Integrated Security=True; MultipleActiveResultSets=True;");
            return new AssessorDbContext(optionsBuilder.Options, true);
        }
    }
}
