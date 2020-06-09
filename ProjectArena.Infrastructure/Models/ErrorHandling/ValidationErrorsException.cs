using System;
using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.ErrorHandling
{
    public class ValidationErrorsException : Exception
    {
        public IEnumerable<HttpErrorInfo> Errors { get; set; }

        public ValidationErrorsException(IEnumerable<HttpErrorInfo> errors)
        {
            this.Errors = errors;
        }
    }
}