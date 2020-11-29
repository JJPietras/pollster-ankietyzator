using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DataModel.PollModel;
using Ankietyzator.Models.DataModel.StatModel;
using Ankietyzator.Models.DTO.PollDTOs;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models
{
    public class AnkietyzatorDbContext : DbContext
    {
        public AnkietyzatorDbContext() {}
        
        public AnkietyzatorDbContext(DbContextOptions<AnkietyzatorDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<UpgradeKey> UpgradeKeys { get; set; }
        public DbSet<PollForm> PollForms { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuestionStat> QuestionStats { get; set; }
        public DbSet<PollStat> PollStats { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AnswerConfiguration());
            base.OnModelCreating(builder);
        }
    }
}