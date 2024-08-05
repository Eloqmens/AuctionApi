using MediatR;

namespace Application.Commands.Lot.Create
{
    public class CreateLotCommand : IRequest<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime EndTime { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }
    }
}
