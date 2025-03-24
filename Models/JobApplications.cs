using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
    public class JobApplications
    {
        public int Id { get; set; }

        [Required]
        public int user_id { get; set; }
        [Required]
        public string Company_name { get; set; }

        [Required]
        public string job_title { get; set; }

        [Required]
        public string job_location { get; set; } 

        [Required]
        public string application_date { get; set; }

        [Required]
        public string status { get; set; }

        [Required]
        public string notes { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
