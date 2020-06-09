using System;

namespace ProjectArena.Infrastructure.Models.ErrorHandling
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string error)
            : base(error)
        {
        }
    }
}