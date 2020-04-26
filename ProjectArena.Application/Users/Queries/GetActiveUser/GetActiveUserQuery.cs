using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.Identity;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.User;

namespace ProjectArena.Application.Users.Queries.GetActiveUser
{
    public class GetActiveUserQuery : IRequest<ActiveUserDto>
    {
        public ClaimsPrincipal User { get; set; }

        private class Handler : IRequestHandler<GetActiveUserQuery, ActiveUserDto>
        {
            private readonly IdentityUserManager _userManager;
            private readonly IBattleService _battleService;

            public Handler(
                IdentityUserManager userManager,
                IBattleService battleService)
            {
                _userManager = userManager;
                _battleService = battleService;
            }

            public async Task<ActiveUserDto> Handle(GetActiveUserQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(request.User);
                var statusInBattle = _battleService.IsUserInBattle(user.Id);
                return new ActiveUserDto()
                {
                    Name = user.ViewName,
                    UniqueId = user.UserName,
                    Email = user.Email,
                    State = statusInBattle ? UserState.Battle : UserState.Lobby
                };
            }
        }
    }
}