using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel.PollModel
{
    public class Question
    {
        [Key] public int QuestionId { get; set; }

        [ForeignKey("PollId")] public int Poll { get; set; }

        [Required] public uint Position { get; set; }

        [Required] [MaxLength(150)] public string Title { get; set; }

        [Required] [MaxLength(500)] public string Options { get; set; }

        [Required] public bool AllowEmpty { get; set; }

        [Required] public ushort MaxLength { get; set; }

        [Required] public QuestionType Type { get; set; }
    }

    public class QuestionDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
    }
}