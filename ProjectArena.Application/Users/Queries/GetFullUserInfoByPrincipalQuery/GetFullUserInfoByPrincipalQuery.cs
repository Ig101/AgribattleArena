using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.Identity.Entities;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.User;

namespace ProjectArena.Application.Users.Queries.GetFullUserInfoByPrincipalQuery
{
    public class GetFullUserInfoByPrincipalQuery : IRequest<FullUserInfoDto>
    {
        public ClaimsPrincipal User { get; set; }

        private class Handler : IRequestHandler<GetFullUserInfoByPrincipalQuery, FullUserInfoDto>
        {
            private readonly IdentityUserManager _userManager;

            public Handler(
                IdentityUserManager userManager)
            {
                _userManager = userManager;
            }

            public async Task<FullUserInfoDto> Handle(GetFullUserInfoByPrincipalQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(request.User);
                return new FullUserInfoDto()
                {
                    Id = user.Id,
                    Name = user.ViewName,
                    UniqueId = user.UserName,
                    Email = user.Email
                };
            }
        }
    }
}