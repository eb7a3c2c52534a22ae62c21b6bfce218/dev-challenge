using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    public class NameAscendingSortStrategy : ISortStrategy
    {
        public async Task<IEnumerable<Product>> Sort(IEnumerable<Product> records)
        {
            return await Task.FromResult(records.OrderBy(p => p.Name));
        }
    }
}