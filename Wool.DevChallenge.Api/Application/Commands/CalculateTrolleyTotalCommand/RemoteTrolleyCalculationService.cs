using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Wool.DevChallenge.Api.Config;
using Wool.DevChallenge.Api.Exceptions;

namespace Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand
{
    internal class RemoteTrolleyCalculationService : ITrolleyCalculationService
    {
        private readonly HttpClient _apiClient;
        private readonly IOptions<AppSettings> _appSettings;

        public RemoteTrolleyCalculationService(IHttpClientFactory apiClientFactory, IOptions<AppSettings> appSettings)
        {
            _apiClient = apiClientFactory.CreateClient();
            _appSettings = appSettings;
        }

        public async Task<decimal> GetTrolleyCalculations(CalculateTrolleyTotalCommand.RequestTrolley request, CancellationToken cancellationToken)
        {
            var apiClient = _apiClient;
            var appSettings = _appSettings;


            using var modelContent = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            var response = await apiClient.PostAsync(
                $"{appSettings.Value.Url}{UrlsConfig.TrolleyOperations.CalculateTrolleyTotal(appSettings.Value.Token)}",
                modelContent,
                cancellationToken);

            var data = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                decimal.TryParse(data, out var total);
                return total;
            }

            throw new TrolleyCalculationRemoteException(response.StatusCode, data);

        }
    }
}