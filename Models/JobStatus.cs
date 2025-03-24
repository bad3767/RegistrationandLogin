using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
    public class JobStatus
    {
        public int Id { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        public int job_id { get; set; }

        [Required]
        public string status { get; set; }




        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
