using ErrorOr;
using MediatR;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;
using Tri_Wall.Domain.DataProviders;

namespace Tri_Wall.Application.Authorize
{
    public class AuthorizeCommandHandler : IRequestHandler<AuthorizeCommand, ErrorOr<JwtResponse>>
    {
        private readonly IJwtRegister _jwtRegister;
        private readonly IDataProviderRepository _dataProviderRepository;
        public AuthorizeCommandHandler(IJwtRegister jwtRegister, IDataProviderRepository dataProviderRepository)
        {
            _jwtRegister = jwtRegister;
            _dataProviderRepository = dataProviderRepository;
        }
        public async Task<ErrorOr<JwtResponse>> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
        {
            var dt=await _dataProviderRepository.Query(new DataProvider
            {
                StoreName="",
                DBType="JwtCheckAccount",
                Par1 = request.Account,
                Par2 = request.Password
            });
            if (dt.Rows.Count == 0)
            {
                return await Task.FromResult(new JwtResponse("404","Invalid User","","").ToErrorOr()).ConfigureAwait(false);
            }
            return await Task.FromResult((await _jwtRegister
                .GenerateToken(request.Account)
                .ConfigureAwait(false))
                .ToErrorOr())
                .ConfigureAwait(false);
        }
    }
}
