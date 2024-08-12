using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public int LotId { get; set; }
        public Lot Lot { get; set; }
        public Guid UserId { get; set; }
    }
}
