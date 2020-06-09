using System;

namespace ProjectArena.Infrastructure.Models.ErrorHandling
{
    public class ServiceUnreachableException : Exception
    {
        public ServiceUnreachableException(string error)
            : base(error)
        {
        }
    }
}