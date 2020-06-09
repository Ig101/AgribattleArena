using System;

namespace ProjectArena.Infrastructure.Models.ErrorHandling
{
    public class CannotPerformOperationException : Exception
    {
        public CannotPerformOperationException(string error)
            : base(error)
        {
        }
    }
}