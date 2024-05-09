using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Authorize
{
    public class AuthorizeCommand : IRequest<ErrorOr<JwtResponse>>
    {
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
