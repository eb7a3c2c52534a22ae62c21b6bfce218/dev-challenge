using AutoMapper;
using Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand;
using Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery;
using Wool.DevChallenge.Api.Models;
using Wool.DevChallenge.Api.Models.Trolley;

namespace Wool.DevChallenge.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TrolleyProductInputModel, CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct>();
            CreateMap<TrolleyProductQuantityInputModel, CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>();
            CreateMap<TrolleySpecialInputModel, CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial>();
            CreateMap<TrolleyInputModel, CalculateTrolleyTotalCommand.RequestTrolley>();

            CreateMap<SortOption, ProductSortOption>();

        }
    }
}
