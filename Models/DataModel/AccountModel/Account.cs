using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel.AccountModel
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string EMail { get; set; }
        
        [Required]
        [MaxLength(400)]
        public string Tags { get; set; }
        
        [Required]
        public UserType UserType { get; set; }
    }

    public class AccountDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
    }
}