using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Wool.DevChallenge.Api.Config;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy
{
    internal class RemoteShopperHistoryService : IRemoteShopperHistoryService
    {
        private readonly HttpClient _apiClient;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger<RemoteShopperHistoryService> _logger;

        public RemoteShopperHistoryService(HttpClient apiClient, IOptions<AppSettings> appSettings, ILogger<RemoteShopperHistoryService> logger)
        {
            _apiClient = apiClient;
            _appSettings = appSettings;
            _logger = logger;
        }

        public async Task<IEnumerable<ShoppingHistory>> GetShoppingHistory()
        {
            var shoppingHistories = new List<ShoppingHistory>();

            _logger.LogInformation("Calling service at {Url} for shopper history", _appSettings.Value.Url);

            var response = await _apiClient.GetStringAsync(
                $"{_appSettings.Value.Url}{UrlsConfig.ProductsOperations.GetShopperHistory(_appSettings.Value.Token)}");

            if (!string.IsNullOrWhiteSpace(response))
            {
                var x = JsonConvert.DeserializeObject<IEnumerable<ShoppingHistory>>(response);
                shoppingHistories.AddRange(x);
                _logger.LogInformation("Remote service provided {Count} shopper's history", shoppingHistories.Count);
            }

            _logger.LogInformation("Returning {Count} shopper history items", shoppingHistories.Count);
            return shoppingHistories;
        }


    }
}