using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectArena.Application.Battle.Queries.GetFullSynchronizationInfo;
using ProjectArena.Application.Users.Queries.GetFullUserInfoByPrincipalQuery;

namespace ProjectArena.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/battle")]
    public class BattleController : MediatorControllerBase
    {
        public BattleController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetSynchronizationInfoAsync()
        {
            var user = await Mediator.Send(new GetFullUserInfoByPrincipalQuery()
            {
                User = User
            });

            return Ok(await Mediator.Send(new GetFullSynchronizationInfoQuery()
            {
                UserId = user.Id
            }));
        }

        [HttpGet("{sceneId}")]
        public async Task<IActionResult> GetSynchronizationInfoAsync(Guid sceneId)
        {
            var user = await Mediator.Send(new GetFullUserInfoByPrincipalQuery()
            {
                User = User
            });

            return Ok(await Mediator.Send(new GetFullSynchronizationInfoQuery()
            {
                UserId = user.Id,
                SceneId = sceneId
            }));
        }

        [HttpDelete("{sceneId}")]
        public async Task<IActionResult> LeaveBattleAsync(Guid sceneId)
        {
            var user = await Mediator.Send(new GetFullUserInfoByPrincipalQuery()
            {
                User = User
            });

            return Ok(await Mediator.Send(new LeaveSceneCommand()
            {
                UserId = user.Id,
                SceneId = sceneId
            }));
        }
    }
}