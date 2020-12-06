using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wool.DevChallenge.Api.Exceptions;

namespace Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand
{
    public sealed class CalculateTrolleyTotalCommand : IRequest<decimal>
    {
        public CalculateTrolleyTotalCommand(RequestTrolley trolley, TrolleyCalculatorType calculatorType)
        {
            if(trolley == null ) throw new ArgumentNullException(nameof(trolley));

            if (trolley.Products == null) throw new ArgumentNullException(nameof(trolley.Products));

            if (trolley.Quantities == null) throw new ArgumentNullException(nameof(trolley.Quantities));

            if (trolley.Quantities.Select(x => x.Name).Except(trolley.Products.Select(x => x.Name)).Any())
                throw new InvalidTrolleyStateException("Products and Quantities are out of sync");

            if (trolley.Specials != null 
                && trolley.Specials.Any() 
                && trolley.Specials.SelectMany(x => x.Quantities.Select(q=>q.Name))
                    .Except(trolley.Products.Select(x => x.Name)).Any())
                throw new InvalidTrolleyStateException("Products and Specials are out of sync");

            if (trolley.Quantities.Any(x => x.Quantity <= 0))
                throw new InvalidTrolleyStateException("Quantities cannot be negative");

            Trolley = trolley;

            TrolleyCalculatorType = calculatorType;
        }

        private RequestTrolley Trolley { get; set; }

        private TrolleyCalculatorType TrolleyCalculatorType { get; }

        public class RequestTrolley
        {
            public IEnumerable<TrolleyProduct> Products { get; set; }
            public IEnumerable<TrolleySpecial> Specials { get; set; }
            public IEnumerable<TrolleyProductQuantity> Quantities { get; set; }

            public class TrolleyProduct
            {
                public string Name { get; set; }
                public decimal Price { get; set; }
            }

            public class TrolleyProductQuantity
            {
                public string Name { get; set; }
                public int Quantity { get; set; }
            }

            public class TrolleySpecial
            {
                public IEnumerable<TrolleyProductQuantity> Quantities { get; set; }
                public int Total { get; set; }
            }

        }


        public sealed class CalculateTrolleyTotalCommandHandler : IRequestHandler<CalculateTrolleyTotalCommand, decimal>
        {
            private readonly ITrolleyCalculatorFactory _trolleyCalculatorFactory;
            private readonly ILogger<CalculateTrolleyTotalCommandHandler> _logger;

            public CalculateTrolleyTotalCommandHandler(ITrolleyCalculatorFactory trolleyCalculatorFactory, ILogger<CalculateTrolleyTotalCommandHandler> logger)
            {
                _trolleyCalculatorFactory = trolleyCalculatorFactory;
                _logger = logger;
            }

            public async Task<decimal> Handle(CalculateTrolleyTotalCommand request, CancellationToken cancellationToken)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (request.Trolley == null) throw new ArgumentNullException(nameof(request.Trolley));

                _logger.LogInformation("Calling service to calculate trolley total");

                _logger.LogDebug("Calling service to calculate trolley total for {@Trolley}", request.Trolley);

                var calculator = _trolleyCalculatorFactory.GetTrolleyCalculator(request.TrolleyCalculatorType);

                var total = await calculator.GetTrolleyCalculations(request.Trolley, cancellationToken);

                return Convert.ToDecimal(total);
            }
        }
    }
}
