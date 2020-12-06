using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand;
using Wool.DevChallenge.Api.Application.Queries.GetUserQuery;
using Wool.DevChallenge.Api.Controllers;
using Wool.DevChallenge.Api.Exceptions;
using Wool.DevChallenge.Api.Models.Trolley;
using Xunit;

namespace Wool.DevChallenge.Api.Tests
{
    [Trait("Category", "Lean")]
    public class TrolleyTotalControllerTests
    {
        [Fact]
        public async Task TrolleyTotal_MapperFails_Returns_BadRequest()
        {
            var logger = Mock.Of<ILogger<TrolleyTotalController>>();

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<CalculateTrolleyTotalCommand>(It.IsAny<TrolleyInputModel>()))
                .Throws(new Exception("Mapping failed"));

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ApplicationSettingsInvalidException("missing"));

            var controller = new TrolleyTotalController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var response = await controller.TrolleyTotal(It.IsAny<TrolleyInputModel>());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("Value cannot be null. (Parameter 'trolley')");
            details.Title.ShouldBe("Invalid Trolley State");
            details.Status.ShouldBe((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task TrolleyTotal_ProductsAreNull_ShouldReturn_InternalServerError()
        {
            var logger = Mock.Of<ILogger<TrolleyTotalController>>();

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<CalculateTrolleyTotalCommand.RequestTrolley>(It.IsAny<TrolleyInputModel>()))
                .Returns(new CalculateTrolleyTotalCommand.RequestTrolley());

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CalculateTrolleyTotalCommand.RequestTrolley>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Exception("unknown"));

            var controller = new TrolleyTotalController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var response = await controller.TrolleyTotal(new TrolleyInputModel());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("Value cannot be null. (Parameter 'Products')");
            details.Title.ShouldBe("Invalid Trolley State");
            details.Status.ShouldBe((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task TrolleyTotal_QuantitiesAreNull_ShouldReturn_InternalServerError()
        {
            var logger = Mock.Of<ILogger<TrolleyTotalController>>();

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<CalculateTrolleyTotalCommand.RequestTrolley>(It.IsAny<TrolleyInputModel>()))
                .Returns(new CalculateTrolleyTotalCommand.RequestTrolley
                {
                    Products = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct>(),
                    Quantities = null
                });

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CalculateTrolleyTotalCommand.RequestTrolley>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Exception("unknown"));

            var controller = new TrolleyTotalController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var response = await controller.TrolleyTotal(new TrolleyInputModel());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("Value cannot be null. (Parameter 'Quantities')");
            details.Title.ShouldBe("Invalid Trolley State");
            details.Status.ShouldBe((int)HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task TrolleyTotal_MediatorReturnsInvalidTrolley_ShouldReturn_BadRequest()
        {
            var logger = Mock.Of<ILogger<TrolleyTotalController>>();

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<CalculateTrolleyTotalCommand.RequestTrolley>(It.IsAny<TrolleyInputModel>()))
                .Returns(new CalculateTrolleyTotalCommand.RequestTrolley
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
                });

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CalculateTrolleyTotalCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidTrolleyStateException("bad trolley"));

            var controller = new TrolleyTotalController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var response = await controller.TrolleyTotal(new TrolleyInputModel());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("bad trolley");
            details.Title.ShouldBe("Invalid Trolley State");
            details.Status.ShouldBe((int)HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task TrolleyTotal_MediatorReturnsTrolleyFailed_ShouldReturn_InternalServerError()
        {
            var logger = Mock.Of<ILogger<TrolleyTotalController>>();

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<CalculateTrolleyTotalCommand.RequestTrolley>(It.IsAny<TrolleyInputModel>()))
                .Returns(new CalculateTrolleyTotalCommand.RequestTrolley
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
                });

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CalculateTrolleyTotalCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TrolleyCalculationRemoteException(HttpStatusCode.Ambiguous, "trolley"));

            var controller = new TrolleyTotalController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var response = await controller.TrolleyTotal(new TrolleyInputModel());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe((int)HttpStatusCode.Ambiguous);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("trolley");
            details.Title.ShouldBe("Remote Error");
            details.Status.ShouldBe((int)HttpStatusCode.Ambiguous);
        }


        [Fact]
        public async Task TrolleyTotal_WhenDataIsFound_ShouldReturn_OkSuccess()
        {
            var logger = Mock.Of<ILogger<TrolleyTotalController>>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<CalculateTrolleyTotalCommand.RequestTrolley>(It.IsAny<TrolleyInputModel>()))
                .Returns(new CalculateTrolleyTotalCommand.RequestTrolley
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
                });


            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CalculateTrolleyTotalCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((decimal)123.1);

            var controller = new TrolleyTotalController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            var response = await controller.TrolleyTotal(new TrolleyInputModel());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<OkObjectResult>();

            var data = response as OkObjectResult;
            data.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            data.Value.ShouldBeOfType<decimal>();

            var details = data.Value;
            details.ShouldBe(123.1);
        }

    }
}
