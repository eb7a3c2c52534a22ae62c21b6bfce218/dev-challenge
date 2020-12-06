using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    public class PriceHighSortStrategy : ISortStrategy
    {
        public async Task<IEnumerable<Product>> Sort(IEnumerable<Product> records)
        {
            return await Task.FromResult(records.OrderByDescending(p => p.Price));
        }
    }
}