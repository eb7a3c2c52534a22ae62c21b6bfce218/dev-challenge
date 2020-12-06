using System;
using Microsoft.Extensions.DependencyInjection;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    public class SortingStrategyFactory : ISortingStrategyFactory
    {
        private readonly IServiceProvider _provider;

        public SortingStrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }


        public ISortStrategy GetSortStrategy(ProductSortOption sortOption)
        {
            switch (sortOption)
            {
                case ProductSortOption.Low:
                    return _provider.GetRequiredService<PriceLowSortStrategy>();
                case ProductSortOption.High:
                    return _provider.GetRequiredService<PriceHighSortStrategy>();
                case ProductSortOption.Ascending:
                    return _provider.GetRequiredService<NameAscendingSortStrategy>();
                case ProductSortOption.Descending:
                    return _provider.GetRequiredService<NameDescendingSortStrategy>();
                case ProductSortOption.Recommended:
                    return _provider.GetRequiredService<RecommendedSortStrategy>();
                default:
                    return _provider.GetRequiredService<NoSortStrategy>();
            }
        }
    }
}