using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel.AccountModel
{
    public class UpgradeKey
    {
        [Key] [MaxLength(200)] public string Key { get; set; }

        [Required(AllowEmptyStrings = true)] [MaxLength(200)] public string EMail { get; set; }

        [Required] public UserType UserType { get; set; }
    }

    public class UpgradeKeyDbContext : DbContext
    {
        public DbSet<Account> UpgradeKeys { get; set; }
    }
}