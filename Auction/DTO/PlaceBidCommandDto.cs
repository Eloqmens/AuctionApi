using Application.Commands.Bid.PlaceBid;
using Application.Commands.Lot.Create;
using Application.Mappings;
using AutoMapper;

namespace Auction.DTO
{
    public class PlaceBidCommandDto : IMapWith<PlaceBidCommand>
    {
        public int LotId { get; set; }
        public decimal Amount { get; set; }

        public void Mapping(Profile profile) 
        {
            profile.CreateMap<PlaceBidCommandDto, PlaceBidCommand>()
                .ForMember(placeCommand => placeCommand.LotId,
                    opt => opt.MapFrom(placeDto => placeDto.LotId))
                .ForMember(placeCommand => placeCommand.Amount,
                    opt => opt.MapFrom(placeDto => placeDto.Amount));
        }
    }
}
