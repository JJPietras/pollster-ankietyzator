//using System.Data.Entity;

using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel
{
    public class Account
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public byte[] PasswordHash { get; set; }
        public UserType UserType { get; set; }
        
        public static byte[] GetHash(string password)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    public class AccountDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
    }
}