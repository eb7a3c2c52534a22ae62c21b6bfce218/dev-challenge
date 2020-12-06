using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    public interface IRemoteShopperHistoryService
    {
        Task<IEnumerable<ShoppingHistory>> GetShoppingHistory();
    }
}