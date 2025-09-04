using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class LotImage
    {
        public int Id { get; set; }
        
        [Required]
        public int LotId { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string FileUrl { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;
        
        public long FileSize { get; set; }
        
        public bool IsMain { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}
