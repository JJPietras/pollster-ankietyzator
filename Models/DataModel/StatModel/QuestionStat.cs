using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel.StatModel
{
    public class QuestionStat
    {
        [Key] [ForeignKey("questions_primary_key")] public int QuestionId { get; set; }

        [Required] [MaxLength(500)] public string AnswerCounts { get; set; }
    }

    public class QuestionStatDbContext : DbContext
    {
        public DbSet<QuestionStat> QuestionStats { get; set; }
    }
}