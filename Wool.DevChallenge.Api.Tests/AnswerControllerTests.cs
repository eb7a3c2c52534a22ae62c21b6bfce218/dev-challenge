using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Wool.DevChallenge.Api.Application.Queries.GetUserQuery;
using Wool.DevChallenge.Api.Controllers;
using Wool.DevChallenge.Api.Exceptions;
using Xunit;

namespace Wool.DevChallenge.Api.Tests
{
    [Trait("Category", "Lean")]
    public class AnswerControllerTests
    {
        [Fact]
        public async Task GetUser_WhenAppSettingsIsMissing_ShouldReturn_InternalServerError()
        {
            var logger = Mock.Of<ILogger<AnswerController>>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ApplicationSettingsInvalidException("missing"));


            var controller = new AnswerController(logger, mediator.Object)
            {
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
            };

            var response = await controller.GetUser();

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe(500);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("missing");
            details.Title.ShouldBe("Server configuration invalid");
            details.Status.ShouldBe(500);
        }

        [Fact]
        public async Task GetUser_WhenUnknownFailure_ShouldReturn_InternalServerError()
        {
            var logger = Mock.Of<ILogger<AnswerController>>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("unknown"));

            var controller = new AnswerController(logger, mediator.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var response = await controller.GetUser();

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
        public async Task GetUser_WhenDataIsNull_ShouldReturn_InternalServerError()
        {
            var logger = Mock.Of<ILogger<AnswerController>>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserQuery.UserViewModel)null);

            var controller = new AnswerController(logger, mediator.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var response = await controller.GetUser();

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var data = response as ObjectResult;
            data.StatusCode.ShouldBe(500);
            data.Value.ShouldBeOfType<ProblemDetails>();

            var details = data.Value as ProblemDetails;
            details.Detail.ShouldBe("Unknown error reading user data");
            details.Title.ShouldBe("Internal Server Error");
            details.Status.ShouldBe(500);
        }

        [Fact]
        public async Task GetUser_WhenDataIsFound_ShouldReturn_OkSuccess()
        {
            var logger = Mock.Of<ILogger<AnswerController>>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send( It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetUserQuery.UserViewModel { Name = "a", Token = "b"});

            var controller = new AnswerController(logger, mediator.Object)
            {
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
            };

            var response = await controller.GetUser();
            response.ShouldNotBeNull();
            response.ShouldBeOfType<OkObjectResult>();

            var data = response as OkObjectResult;
            data.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            data.Value.ShouldBeOfType<GetUserQuery.UserViewModel>();

            var details = data.Value as GetUserQuery.UserViewModel;
            details.Name.ShouldBe("a");
            details.Token.ShouldBe("b");
        }

    }
}
