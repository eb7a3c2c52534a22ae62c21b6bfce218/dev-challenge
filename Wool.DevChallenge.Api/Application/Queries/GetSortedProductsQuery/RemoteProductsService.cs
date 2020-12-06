using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Wool.DevChallenge.Api.Config;

namespace Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery
{
    internal class RemoteProductsService : IRemoteProductsService
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<RemoteProductsService> _logger;
        private readonly AppSettings _appSettings;

        public RemoteProductsService(HttpClient apiClient, IOptions<AppSettings> appSettings, ILogger<RemoteProductsService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var products = new List<Product>();
            _logger.LogInformation("Calling service at {Url} for products", _appSettings.Url);
            var response =
                await _apiClient.GetStringAsync(
                    $"{_appSettings.Url}{UrlsConfig.ProductsOperations.GetProducts(_appSettings.Token)}");

            if (!string.IsNullOrWhiteSpace(response))
            {
                var x = JsonConvert.DeserializeObject<IEnumerable<Product>>(response);
                products.AddRange(x);
                _logger.LogInformation("Remote service provided {Count} products", products.Count);
            }

            _logger.LogInformation("Returning {Count} products", products.Count);
            return products;
        }
    }
}
