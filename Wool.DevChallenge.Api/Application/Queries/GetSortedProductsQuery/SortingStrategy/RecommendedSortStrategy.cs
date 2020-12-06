using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    public class RecommendedSortStrategy : ISortStrategy
    {
        private readonly IServiceProvider _provider;

        public RecommendedSortStrategy(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<IEnumerable<Product>> Sort(IEnumerable<Product> records)
        {
            var customerProducts = await GetShoppingHistory();
            return SortProductsByPopularity(records, customerProducts);
        }

        private async Task<IEnumerable<ShoppingHistory>> GetShoppingHistory()
        {
            var shopperHistoryService = _provider.GetRequiredService<IRemoteShopperHistoryService>();
            return await shopperHistoryService.GetShoppingHistory();
        }

        private IEnumerable<Product> SortProductsByPopularity(IEnumerable<Product> products, IEnumerable<ShoppingHistory> customerProducts)
        {
            var quantitiesForPopularProducts = customerProducts.SelectMany(cp => cp.Products).GroupBy(g => g.Name)
                                                            .ToDictionary(key => key.Key, value => value.Sum(p => p.Quantity))
                                                            .OrderByDescending(kv => kv.Value);

            var popularProducts = quantitiesForPopularProducts
                                        .Join(products, 
                                            qp => qp.Key,
                                            product => product.Name,
                                            (qp, product) => product);

            return popularProducts.Union(products);
        }
    }



    public class ShoppingHistory
    {
        public int CustomerId { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }


}
