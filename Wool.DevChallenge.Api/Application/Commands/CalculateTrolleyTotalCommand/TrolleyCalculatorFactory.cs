using Microsoft.Extensions.DependencyInjection;
using System;

namespace Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand
{
    public interface ITrolleyCalculatorFactory
    {
        ITrolleyCalculationService GetTrolleyCalculator(TrolleyCalculatorType calculatorType);
    }

    public class TrolleyCalculatorFactory : ITrolleyCalculatorFactory
    {
        private readonly IServiceProvider _provider;

        public TrolleyCalculatorFactory(IServiceProvider provider)
        {
            _provider = provider;
        }
        public ITrolleyCalculationService GetTrolleyCalculator(TrolleyCalculatorType calculatorType)
        {
            switch (calculatorType)
            {
                case TrolleyCalculatorType.Local:
                    return _provider.GetRequiredService<ITrolleyCalculationService>();

                default:
                        return _provider.GetRequiredService<RemoteTrolleyCalculationService>();
            }
        }
    }
}