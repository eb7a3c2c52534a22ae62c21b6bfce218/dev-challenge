using System;

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
                    return new PriceLowSortStrategy();
                case ProductSortOption.High:
                    return new PriceHighSortStrategy();
                case ProductSortOption.Ascending:
                    return new NameAscendingSortStrategy();
                case ProductSortOption.Descending:
                    return new NameDescendingSortStrategy();
                case ProductSortOption.Recommended:
                    return new RecommendedSortStrategy(_provider);
                default:
                    return new NoSortStrategy();

            }
        }
    }
}