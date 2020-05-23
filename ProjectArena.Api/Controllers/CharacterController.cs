using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectArena.Application.Game.Commands.ChangeCharacterTalents;
using ProjectArena.Application.Game.Commands.HirePatron;
using ProjectArena.Application.Users.Queries.GetFullUserInfoByPrincipalQuery;

namespace ProjectArena.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user/character")]
    public class CharacterController : MediatorControllerBase
    {
        public CharacterController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpPut]
        public async Task<ActionResult> HirePatronAsync(HirePatronCommand model)
        {
            var user = await Mediator.Send(new GetFullUserInfoByPrincipalQuery()
            {
                User = User
            });
            model.UserId = user.Id;
            var result = await Mediator.Send(model);
            return Ok(result);
        }

        [HttpPost("{id}/talents")]
        public async Task<ActionResult> HirePatronAsync(string id, ChangeCharacterTalentsCommand model)
        {
            var user = await Mediator.Send(new GetFullUserInfoByPrincipalQuery()
            {
                User = User
            });
            model.UserId = user.Id;
            model.CharacterId = id;
            await Mediator.Send(model);
            return NoContent();
        }
    }
}