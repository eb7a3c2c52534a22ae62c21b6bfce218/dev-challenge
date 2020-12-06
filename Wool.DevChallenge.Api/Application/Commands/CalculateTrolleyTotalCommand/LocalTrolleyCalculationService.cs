using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand
{
    public class LocalTrolleyCalculationService : ITrolleyCalculationService
    {
        private readonly ILogger<LocalTrolleyCalculationService> _logger;

        public LocalTrolleyCalculationService(ILogger<LocalTrolleyCalculationService> logger)
        {
            _logger = logger;
        }

        public Task<decimal> GetTrolleyCalculations(CalculateTrolleyTotalCommand.RequestTrolley request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting trolley total calculations");
            _logger.LogDebug("Starting trolley total calculations for {@Trolley}", request);

            var productPrices = new Dictionary<string, decimal>();

            foreach (var productQuantity in request.Quantities)
            {
                decimal markedPrice = GerMarkedPrice(request, productQuantity);

                var specialsPrice = GetSpecialsPrice(request, productQuantity, markedPrice);

                decimal approvedProductPrice = ChooseLowerPrice(markedPrice, specialsPrice);

                productPrices.Add(productQuantity.Name, approvedProductPrice);
            }

            var trolleyTotal = productPrices.Sum(x => x.Value);

            _logger.LogDebug("Calculated Price Per Product: {@ProductPrices}", productPrices);
            
            _logger.LogInformation("Calculated Price For Trolley {TrolleyTotal}", trolleyTotal);

            return Task.FromResult(trolleyTotal);
        }

        private static decimal ChooseLowerPrice(decimal markedPrice, decimal specialsPrice)
        {
            return markedPrice <= specialsPrice ? markedPrice : specialsPrice;
        }

        private static decimal GerMarkedPrice(CalculateTrolleyTotalCommand.RequestTrolley request, CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity productQuantity)
        {
            return productQuantity.Quantity * request.Products.OrderBy(x => x.Price).FirstOrDefault().Price;
        }

        private static decimal GetSpecialsPrice(CalculateTrolleyTotalCommand.RequestTrolley request, 
            CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity productQuantity, decimal priceWithoutSpecials)
        {
            var productPrice = priceWithoutSpecials;

            try
            {
                var sps = FindApplicableSpecial(request, productQuantity);

                if (SpecialsIncorrectlySetup(sps))
                    return productPrice;

                var specialSetQuantity = GetSpecialsQuantity(productQuantity, sps);

                if (SpecialQuantityInvalid(specialSetQuantity))
                    return productPrice;

                var productsOverFlowingSpecial = productQuantity.Quantity % specialSetQuantity;

                var productSpecialPrice = (productQuantity.Quantity / specialSetQuantity) * sps.Total;

                var productOverFlowPrice = productsOverFlowingSpecial *
                                           request.Products.OrderBy(x => x.Price).FirstOrDefault().Price;

                productPrice = productOverFlowPrice + productSpecialPrice;

                return productPrice;
            }
            catch
            {
                return priceWithoutSpecials;
            }
        }

        private static int GetSpecialsQuantity(CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity productQuantity, CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial sps)
        {
            try
            {
                return sps.Quantities.FirstOrDefault(x => x.Name == productQuantity.Name).Quantity;
            }
            catch
            {
                return 0;
            }
        }

        private static bool SpecialQuantityInvalid(int specialSetQuantity)
        {
            return specialSetQuantity <= 0;
        }

        private static CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial FindApplicableSpecial(CalculateTrolleyTotalCommand.RequestTrolley request, CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity productQuantity)
        {
            try
            {
                return request.Specials.Where(x => x.Quantities.Any(q => q.Name == productQuantity.Name)).OrderByDescending(x => x.Total).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static bool SpecialsIncorrectlySetup(CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial sps)
        {
            return sps == null || sps.Total <= 0;
        }
    }
}