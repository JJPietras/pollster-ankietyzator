using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ankietyzator.Models.DataModel.PollModel
{
    public class Answer
    {
        [ForeignKey("account_user_primary_key")] public int AccountId { get; set; }

        [ForeignKey("questions_primary_key")] public int QuestionId { get; set; }

        [Required] [MaxLength(2000)] public string Content { get; set; }
    }

    public class AnswerDbContext : DbContext
    {
        public DbSet<Answer> Answers { get; set; }
    }

    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(a => new {a.AccountId, a.QuestionId});
        }
    }
}