using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel.PollModel
{
    public class Question
    {
        [Key] public int QuestionId { get; set; }

        [ForeignKey("questions_poll_foreign_key")] public int Poll { get; set; }

        [Required] public int Position { get; set; }

        [Required] [MaxLength(150)] public string Title { get; set; }

        [Required] [MaxLength(500)] public string Options { get; set; }

        [Required] public bool AllowEmpty { get; set; }

        [Required] public short MaxLength { get; set; }

        [Required] public int Type { get; set; }
    }

    public class QuestionDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
    }
}