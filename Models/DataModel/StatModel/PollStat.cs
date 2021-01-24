using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel.StatModel
{
    public class PollStat
    {
        [Key] [ForeignKey("poll_forms_primary_key")] public int PollId { get; set; }

        [Required] public int Completions { get; set; }

        [Required] public double Percentage { get; set; }
    }

    public class PollStatDbContext : DbContext
    {
        public DbSet<PollStat> PollStats { get; set; }
    }
}