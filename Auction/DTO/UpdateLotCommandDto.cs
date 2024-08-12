using Application.Commands.Lot.Update;
using Application.Mappings;
using AutoMapper;

namespace Auction.DTO
{
    public class UpdateLotCommandDto : IMapWith<UpdateLotCommand>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime EndTime { get; set; }
        public int CategoryId { get; set; }
    
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateLotCommandDto, UpdateLotCommand>()
                .ForMember(lotCommand => lotCommand.Id,
                    opt => opt.MapFrom(lotDto => lotDto.Id))
                .ForMember(lotCommand => lotCommand.Title,
                    opt => opt.MapFrom(lotDto =>lotDto.Title))
                .ForMember(lotCommand => lotCommand.Description,
                    opt => opt.MapFrom(lotDto =>lotDto.Description))
                .ForMember(lotCommand => lotCommand.StartingPrice,
                    opt => opt.MapFrom(lotDto =>lotDto.StartingPrice))
                .ForMember(lotCommand => lotCommand.EndTime,                      
                    opt => opt.MapFrom(lotDto =>lotDto.EndTime))
                .ForMember(lotCommand => lotCommand.CategoryId,
                    opt => opt.MapFrom(lotDto => lotDto.CategoryId));
        }
    }   
}
