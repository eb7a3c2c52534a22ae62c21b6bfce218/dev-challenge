using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery;
using Wool.DevChallenge.Api.Application.Queries.GetUserQuery;
using Wool.DevChallenge.Api.Controllers;
using Wool.DevChallenge.Api.Exceptions;
using Wool.DevChallenge.Api.Models;
using Xunit;

namespace Wool.DevChallenge.Api.Tests
{
    [Trait("Category", "Lean")]
    public class SortControllerTests
    {
        [Fact]
        public async Task TrolleyTotal_MapperFails_Returns_InternalServerError()
        {
            var logger = Mock.Of<ILogger<SortController>>();
            
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ProductSortOption>(It.IsAny<SortOption>()))
                .Throws(new Exception("Mapping failed"));

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ApplicationSettingsInvalidException("missing"));

            var controller = new SortController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
            };

            var response = await controller.Sort(It.IsAny<SortOption>());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe(500);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("Mapping failed");
            details.Title.ShouldBe("Internal Server Error");
            details.Status.ShouldBe(500);
        }

        [Fact]
        public async Task TrolleyTotal_MediatorFails_ShouldReturn_InternalServerError()
        {
            var logger = Mock.Of<ILogger<SortController>>();
            
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ProductSortOption>(It.IsAny<SortOption>()))
                .Returns(It.IsAny<ProductSortOption>());

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetSortedProductsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("unknown"));

            var controller = new SortController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
            };

            var response = await controller.Sort(It.IsAny<SortOption>());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe(500);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("unknown");
            details.Title.ShouldBe("Internal Server Error");
            details.Status.ShouldBe(500);
        }

        [Fact]
        public async Task TrolleyTotal_MediatorReturnsTrolleyFailed_ShouldReturn_InternalServerError()
        {
            var logger = Mock.Of<ILogger<SortController>>();
            
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ProductSortOption>(It.IsAny<SortOption>()))
                .Returns(It.IsAny<ProductSortOption>());

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetSortedProductsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ApplicationSettingsInvalidException("setting"));

            var controller = new SortController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
            };

            var response = await controller.Sort(It.IsAny<SortOption>());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("setting");
            details.Title.ShouldBe("Server configuration invalid");
            details.Status.ShouldBe((int)HttpStatusCode.InternalServerError);
        }


        [Fact]
        public async Task TrolleyTotal_WhenNoData_ShouldReturn_NoContent()
        {
            var logger = Mock.Of<ILogger<SortController>>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ProductSortOption>(It.IsAny<SortOption>()))
                .Returns(It.IsAny<ProductSortOption>());

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetSortedProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<IEnumerable<GetSortedProductsQuery.SortedProductViewModel>>());

            var controller = new SortController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
            };
            var response = await controller.Sort(It.IsAny<SortOption>());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<NoContentResult>();
        }

        [Fact]
        public async Task TrolleyTotal_WhenDataIsFound_ShouldReturn_OKSuccess()
        {
            var logger = Mock.Of<ILogger<SortController>>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<ProductSortOption>(It.IsAny<SortOption>()))
                .Returns(It.IsAny<ProductSortOption>());

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetSortedProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GetSortedProductsQuery.SortedProductViewModel>
                {
                    new GetSortedProductsQuery.SortedProductViewModel
                    {
                        Name = "a", Price = 1.0M, Quantity = 1
                    },
                    new GetSortedProductsQuery.SortedProductViewModel
                    {
                        Name = "b", Price = 1.0M, Quantity = 2
                    }

                });

            var controller = new SortController(mediator.Object, mapper.Object, logger)
            {
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
            };
            var response = await controller.Sort(It.IsAny<SortOption>());

            response.ShouldNotBeNull();
            response.ShouldBeOfType<OkObjectResult>();
            
            var data = response as OkObjectResult;
            var products = data.Value as IEnumerable<GetSortedProductsQuery.SortedProductViewModel>;
            products.ShouldNotBeNull();
            products.Count().ShouldBe(2);
        }

    }
}
