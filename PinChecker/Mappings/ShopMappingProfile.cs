using AutoMapper;
using PinChecker.Models.Database;
using PinChecker.Models;

namespace PinChecker.Mappings;

public class ShopMappingProfile : Profile
{
    public ShopMappingProfile()
    {
        CreateMap<Shop, CosmosShop>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.app_type, opt => opt.MapFrom(src => Constants.CosmosShopPartition));

        CreateMap<CosmosShop, Shop>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.id));
    }
}