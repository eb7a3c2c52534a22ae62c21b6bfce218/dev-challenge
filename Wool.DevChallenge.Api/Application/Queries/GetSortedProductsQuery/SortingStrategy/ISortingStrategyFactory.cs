namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    public interface ISortingStrategyFactory
    {
        ISortStrategy GetSortStrategy(ProductSortOption sortOption);
    }
}