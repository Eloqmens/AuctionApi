using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Lot
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }
        
        // Для оптимистической блокировки
        [Timestamp]
        public byte[] RowVersion { get; set; }
        
        public Category Category { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public ICollection<Bid> Bids { get; set; }
        public ICollection<LotImage> Images { get; set; } = new List<LotImage>();
    }
}
