using System;

namespace ProjectArena.Infrastructure.Models.ErrorHandling
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string error)
            : base(error)
        {
        }
    }
}