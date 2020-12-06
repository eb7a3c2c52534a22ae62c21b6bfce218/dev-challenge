using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    public class NoSortStrategy : ISortStrategy
    {
        public Task<IEnumerable<Product>> Sort(IEnumerable<Product> records)
        {
            return Task.FromResult(records);
        }
    }
}