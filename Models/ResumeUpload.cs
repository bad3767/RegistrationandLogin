using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
    public class ResumeUpload
    {
        public int Id { get; set; }

        [Required]
        public int user_id { get; set; }
        [Required]
        public string file_name { get; set; }

        [Required]
        public string file_url { get; set; }

      

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
