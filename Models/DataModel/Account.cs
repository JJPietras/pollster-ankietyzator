using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel
{
    public class Account
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Tags { get; set; }
        public UserType UserType { get; set; }
    }

    public class AccountDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
    }
}