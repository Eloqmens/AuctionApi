using Application.Commands.Lot.Update;
using Application.Mappings;
using AutoMapper;

namespace Auction.DTO
{
    public class UpdateLotCommandDto
    {
        public class CreateLotCommandDto : IMapWith<UpdateLotCommand>
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public decimal StartingPrice { get; set; }
            public DateTime EndTime { get; set; }
            public int CategoryId { get; set; }

            public void Mapping(Profile profile)
            {
                profile.CreateMap<CreateLotCommandDto, UpdateLotCommand>()
                    .ForMember(noteCommand => noteCommand.Title,
                        opt => opt.MapFrom(noteDto => noteDto.Title))
                    .ForMember(noteCommand => noteCommand.Description,
                        opt => opt.MapFrom(noteDto => noteDto.Description))
                    .ForMember(noteCommand => noteCommand.StartingPrice,
                        opt => opt.MapFrom(noteDto => noteDto.StartingPrice))
                    .ForMember(noteCommand => noteCommand.EndTime,
                        opt => opt.MapFrom(noteDto => noteDto.EndTime))
                    .ForMember(noteCommand => noteCommand.CategoryId,
                        opt => opt.MapFrom(noteDto => noteDto.CategoryId));
            }
        }
    }
}
