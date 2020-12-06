using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Wool.DevChallenge.Api.Config;
using Wool.DevChallenge.Api.Exceptions;

namespace Wool.DevChallenge.Api.Application.Queries.GetUserQuery
{
    public class GetUserQuery : IRequest<GetUserQuery.UserViewModel>
    {

        public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserViewModel>
        {
            private readonly AppSettings _appSettings;

            public GetUserQueryHandler(IOptions<AppSettings> appSettings)
            {
                _appSettings = appSettings.Value;
            }
            public Task<UserViewModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                if (_appSettings == null)
                    throw new ApplicationSettingsInvalidException("Application settings are missing");

                return Task.FromResult<UserViewModel>(new UserViewModel
                {
                    Name = _appSettings.Name,
                    Token = _appSettings.Token
                });
            }
        }

        public class UserViewModel
        {
            public string Name { get; set; }
            public string Token { get; set; }
        }
    }
}
