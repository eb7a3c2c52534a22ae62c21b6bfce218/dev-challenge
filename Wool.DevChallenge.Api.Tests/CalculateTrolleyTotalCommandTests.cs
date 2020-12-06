using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand;
using Wool.DevChallenge.Api.Exceptions;
using Xunit;

namespace Wool.DevChallenge.Api.Tests
{
    [Trait("Category", "Lean")]
    public class CalculateTrolleyTotalCommandTests
    {
        [Fact]
        public void CalculateTrolleyTotalCommand_WhenTrolleyIsNull_ShouldFail()
        {
            Should.Throw<ArgumentNullException>(() => 
                new CalculateTrolleyTotalCommand(null, It.IsAny<TrolleyCalculatorType>()))
                .Message.ShouldBe("Value cannot be null. (Parameter 'trolley')");
        }

        [Fact]
        public void Handle_WhenRemoteServiceFails_ShouldFail()
        {
            var logger = Mock.Of<ILogger<CalculateTrolleyTotalCommand.CalculateTrolleyTotalCommandHandler>>();

            var mockService = new Mock<ITrolleyCalculationService>();
            mockService.Setup(x => x.GetTrolleyCalculations(It.IsAny<CalculateTrolleyTotalCommand.RequestTrolley>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TrolleyCalculationRemoteException(It.IsAny<HttpStatusCode>(), "error"));

            var service = new Mock<ITrolleyCalculatorFactory>();
            service.Setup(x => x.GetTrolleyCalculator(It.IsAny<TrolleyCalculatorType>()))
                .Returns(mockService.Object);

            var handler = new CalculateTrolleyTotalCommand.CalculateTrolleyTotalCommandHandler(service.Object, logger);
            handler.Handle(new CalculateTrolleyTotalCommand
            (
                new CalculateTrolleyTotalCommand.RequestTrolley
                {
                    Products = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct>()
                    {
                    new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct{ Name = "a", Price = 1},
                    new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct{ Name = "b", Price = 1},
                    new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct{ Name = "c", Price = 1}
                },

                    Quantities = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                {
                    new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity{ Name = "a", Quantity = 1},
                    new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity{ Name = "b", Quantity = 2},
                    new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity{ Name = "c", Quantity = 1}
                }
                }, It.IsAny<TrolleyCalculatorType>()
            ), It.IsAny<CancellationToken>()).ShouldThrow<TrolleyCalculationRemoteException>().Message.ShouldBe("error");
        }

        [Fact]
        public async Task Handle_WhenRemoteServiceRespondsWithValidData_ShouldReturnTotalAmount()
        {
            var logger = Mock.Of<ILogger<CalculateTrolleyTotalCommand.CalculateTrolleyTotalCommandHandler>>();

            var mockService = new Mock<ITrolleyCalculationService>();
            mockService.Setup(x => x.GetTrolleyCalculations(It.IsAny<CalculateTrolleyTotalCommand.RequestTrolley>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((decimal)123.12);

            var service = new Mock<ITrolleyCalculatorFactory>();
            service.Setup(x => x.GetTrolleyCalculator(It.IsAny<TrolleyCalculatorType>()))
                .Returns(mockService.Object);

            var handler = new CalculateTrolleyTotalCommand.CalculateTrolleyTotalCommandHandler(service.Object, logger);
            var data = await handler.Handle(new CalculateTrolleyTotalCommand
            (
                new CalculateTrolleyTotalCommand.RequestTrolley
                {
                    Products = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct>()
                    {
                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct{ Name = "a", Price = 1},
                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct{ Name = "b", Price = 1},
                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct{ Name = "c", Price = 1}
                    },

                    Quantities = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                    {
                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity{ Name = "a", Quantity = 1},
                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity{ Name = "b", Quantity = 2},
                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity{ Name = "c", Quantity = 1}
                    }
                }, It.IsAny<TrolleyCalculatorType>()
            ), It.IsAny<CancellationToken>());
            data.ShouldBe((decimal)123.12);

        }
    }

}
