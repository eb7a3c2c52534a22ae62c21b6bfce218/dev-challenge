using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    public interface ISortStrategy
    {
        Task<IEnumerable<Product>> Sort(IEnumerable<Product> records);
    }
}