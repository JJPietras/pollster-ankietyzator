using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ankietyzator.Models.DataModel.AccountModel;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel.PollModel
{
    public class PollForm
    {
        [Key] public int PollId { get; set; }

        [ForeignKey("account_user_primary_key")] public int AuthorId { get; set; }
        
        [Required] [MaxLength(200)] public string Title { get; set; }

        [Required] [MaxLength(1000)] public string Tags { get; set; }

        [Required] [MaxLength(1000)] public string Emails { get; set; }

        [Required] public bool NonAnonymous { get; set; }

        [Required] public bool Archived { get; set; }
    }

    public class PollFormDbContext : DbContext
    {
        public DbSet<Account> PollForms { get; set; }
    }
}