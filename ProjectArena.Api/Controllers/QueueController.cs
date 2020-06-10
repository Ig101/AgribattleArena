using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectArena.Application.Queue.Commands.Dequeue;
using ProjectArena.Application.Queue.Commands.Enqueue;
using ProjectArena.Application.Queue.Commands.EnqueueForBot;
using ProjectArena.Application.Users.Queries.GetFullUserInfoByPrincipalQuery;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/queue")]
    public class QueueController : MediatorControllerBase
    {
        public QueueController(IMediator mediator)
         : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> EnqueueAsync()
        {
            var user = await Mediator.Send(new GetFullUserInfoByPrincipalQuery()
            {
                User = User
            });
            if (User.IsInRole("bot"))
            {
                await Mediator.Send(new EnqueueForBotCommand()
                {
                    UserId = user.Id
                });
            }
            else
            {
                await Mediator.Send(new EnqueueCommand()
                {
                    UserId = user.Id,
                    Mode = GameMode.Patrol
                });
            }

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DequeueAsync()
        {
            var user = await Mediator.Send(new GetFullUserInfoByPrincipalQuery()
            {
                User = User
            });
            await Mediator.Send(new DequeueCommand()
            {
                UserId = user.Id
            });
            return NoContent();
        }
    }
}