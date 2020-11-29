using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models.DataModel.PollModel
{
    public class Answer
    {
        [Key]
        public int AccountId { get; set; }
        
        [Key]
        public int QuestionId { get; set; }
        
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }
    }
    
    public class AnswerDbContext : DbContext
    {
        public DbSet<Answer> Answers { get; set; }
    }
}