using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjectArena.Domain.Identity.Entities;

namespace ProjectArena.Domain.Identity
{
  public class IdentityRoleManager : RoleManager<Role>
  {
    public IdentityRoleManager(
        IRoleStore<Role> store,
        IEnumerable<IRoleValidator<Role>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<RoleManager<Role>> logger)
        : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }
  }
}