using MediatR;

namespace Application.Commands.Lot.Update
{
    public class UpdateLotCommand : IRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime EndTime { get; set; }
        public int CategoryId { get; set; }
    }
}
