using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel
{
    public class AnkietyzatorDBContext : DbContext
    {
        public AnkietyzatorDBContext() {}
        
        public AnkietyzatorDBContext(DbContextOptions<AnkietyzatorDBContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
    }
}