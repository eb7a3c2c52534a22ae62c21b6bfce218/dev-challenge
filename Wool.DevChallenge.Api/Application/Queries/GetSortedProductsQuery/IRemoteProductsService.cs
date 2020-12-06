using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery
{
    public interface IRemoteProductsService
    {
        Task<IEnumerable<Product>> GetProducts();
    }
}