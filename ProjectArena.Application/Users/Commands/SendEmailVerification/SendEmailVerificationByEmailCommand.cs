using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectArena.Domain.Email;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.Identity.Entities;
using ProjectArena.Infrastructure.Models.Email;
using ProjectArena.Infrastructure.Models.ErrorHandling;

namespace ProjectArena.Application.Users.Commands.SendEmailVerification
{
    public class SendEmailVerificationByEmailCommand : IRequest
    {
        public string Email { get; set; }

        private class Handler : IRequestHandler<SendEmailVerificationByEmailCommand>
        {
            private readonly IdentityUserManager _userManager;
            private readonly IMediator _mediator;

            public Handler(
                IdentityUserManager userManager,
                IMediator mediator)
            {
                _mediator = mediator;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(SendEmailVerificationByEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    throw new ValidationErrorsException(new[]
                    {
                        new HttpErrorInfo()
                        {
                            Key = "email",
                            Description = $"User with email {request.Email} is not found."
                        }
                    });
                }

                await _mediator.Send(new SendEmailVerificationCommand()
                {
                    User = user
                });
                return Unit.Value;
            }
        }
    }
}