namespace SFA.DAS.AssessorService.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class AssesorDBContextFactory : IDesignTimeDbContextFactory<AssessorDbContext>
    {
        public AssessorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AssessorDbContext>();
            optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=AssessorDB;Integrated Security=True; MultipleActiveResultSets=True;");
            return new AssessorDbContext(optionsBuilder.Options);
        }
    }
}
