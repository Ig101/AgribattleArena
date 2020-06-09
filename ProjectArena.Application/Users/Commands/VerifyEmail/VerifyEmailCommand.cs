using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.Identity.Entities;
using ProjectArena.Infrastructure.Models.ErrorHandling;

namespace ProjectArena.Application.Users.Commands.VerifyEmail
{
    public class VerifyEmailCommand : IRequest
    {
        public string UserId { get; set; }

        public string Code { get; set; }

        private class Handler : IRequestHandler<VerifyEmailCommand>
        {
            private readonly IdentityUserManager _userManager;

            public Handler(IdentityUserManager userManager)
            {
                _userManager = userManager;
            }

            public async Task<Unit> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    throw new ValidationErrorsException(new[]
                    {
                        new HttpErrorInfo()
                        {
                            Key = "email",
                            Description = "User with 'Email' is not found."
                        }
                    });
                }

                var result = await _userManager.ConfirmEmailAsync(user, request.Code);
                if (!result.Succeeded)
                {
                    throw new ValidationErrorsException(result.Errors.Select(x => new HttpErrorInfo()
                    {
                    Key = x.Code,
                    Description = x.Description
                    }));
                }

                return Unit.Value;
            }
        }
    }
}