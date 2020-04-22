using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectArena.Domain.Email;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.Identity.Entities;
using ProjectArena.Infrastructure.Models.ErrorHandling;
using ProjectArena.Mediation.Users.Commands.SendEmailVerification;

namespace ProjectArena.Mediation.Users.Commands.SignUp
{
  public class SignUpCommand : IRequest
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        private class Handler : IRequestHandler<SignUpCommand>
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

            public async Task<Unit> Handle(SignUpCommand request, CancellationToken cancellationToken)
            {
                var user = new User()
                {
                    UserName = Guid.NewGuid().ToString(),
                    Email = request.Email,
                    ViewName = request.Name
                };
                var result = await _userManager.CreateAsync(
                    user, request.Password).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new ValidationErrorsException()
                    {
                        Errors = result.Errors.Select(x => new HttpErrorInfo()
                        {
                            Key = x.Code,
                            Description = x.Description
                        })
                    };
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