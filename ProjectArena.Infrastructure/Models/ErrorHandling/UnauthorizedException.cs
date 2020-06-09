using System;

namespace ProjectArena.Infrastructure.Models.ErrorHandling
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string error)
            : base(error)
        {
        }
    }
}