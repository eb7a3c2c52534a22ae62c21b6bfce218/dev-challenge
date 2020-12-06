using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery
{
    public sealed class GetSortedProductsQuery : IRequest<IEnumerable<GetSortedProductsQuery.SortedProductViewModel>>
    {
        private ProductSortOption SortOption { get; }

        public GetSortedProductsQuery(ProductSortOption productSortOption)
        {
            SortOption = productSortOption;
        }

        public class GetSortedProductsQueryHandler : IRequestHandler<GetSortedProductsQuery, IEnumerable<SortedProductViewModel>>
        {
            private readonly IRemoteProductsService _remoteProductsService;
            private readonly ISortingStrategyFactory _factory;
            private readonly ILogger<GetSortedProductsQueryHandler> _logger;

            public GetSortedProductsQueryHandler(IRemoteProductsService remoteProductsService,
                ISortingStrategyFactory factory,
                ILogger<GetSortedProductsQueryHandler> logger)
            {
                _remoteProductsService = remoteProductsService;
                _factory = factory;
                _logger = logger;
            }

            public async Task<IEnumerable<SortedProductViewModel>> Handle(GetSortedProductsQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    _logger.LogInformation("Getting products sorted by {SortOption}", request.SortOption);

                    var prod = await _remoteProductsService.GetProducts();
                    var sortingStrategy = _factory.GetSortStrategy(request.SortOption);

                    _logger.LogInformation("Sorting Strategy used: {Strategy}", sortingStrategy.GetType().Name);

                    var sortedProducts = await sortingStrategy.Sort(prod);

                    _logger.LogDebug("Sorted Products: {@Products}", sortedProducts);

                    return sortedProducts.Select(x => new SortedProductViewModel
                    {
                        Name = x.Name,
                        Quantity = x.Quantity,
                        Price = x.Price
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failure getting products sorted by {SortOption}", request.SortOption);
                    throw;
                }

            }

        }

        public class SortedProductViewModel
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public decimal Quantity { get; set; }
        }
    }
}
