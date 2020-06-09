using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectArena.Infrastructure.Models.ErrorHandling;

namespace ProjectArena.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationErrorsException validationException)
            {
                var errorsList = validationException.Errors
                    .Select(x => x.Key + " - " + x.Description)
                    .ToList();
                var errorString = string.Join("; ", errorsList);
                _logger.LogError($"Validation error has been found in the {context.Exception.Source} with errors: {errorString}");
                foreach (var error in validationException.Errors)
                {
                    context.ModelState.AddModelError(error.Key, error.Description);
                }

                var factory = context.HttpContext.RequestServices?.GetRequiredService<ProblemDetailsFactory>();
                var details = factory.CreateValidationProblemDetails(context.HttpContext, context.ModelState, 400);
                context.Result = new ObjectResult(details)
                {
                    StatusCode = 400
                };
            }
            else
            {
                _logger.LogError(context.Exception, $"Error has been found in the {context.Exception.Source} with an error: {context.Exception.Message}");
                var factory = context.HttpContext.RequestServices?.GetRequiredService<ProblemDetailsFactory>();

                if (context.Exception is CannotPerformOperationException exception)
                {
                    var details = factory.CreateProblemDetails(context.HttpContext, (int)HttpStatusCode.BadRequest, exception.Message);
                    context.Result = new ObjectResult(details)
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                if (context.Exception is UnauthorizedException)
                {
                    var details = factory.CreateProblemDetails(context.HttpContext, (int)HttpStatusCode.Unauthorized, context.Exception.Message);
                    context.Result = new ObjectResult(details)
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized
                    };
                }

                if (context.Exception is ForbiddenException)
                {
                    var details = factory.CreateProblemDetails(context.HttpContext, (int)HttpStatusCode.Forbidden, context.Exception.Message);
                    context.Result = new ObjectResult(details)
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                }

                if (context.Exception is NotFoundException)
                {
                    var details = factory.CreateProblemDetails(context.HttpContext, (int)HttpStatusCode.NotFound, context.Exception.Message);
                    context.Result = new ObjectResult(details)
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                if (context.Exception is ServiceUnreachableException)
                {
                    var details = factory.CreateProblemDetails(context.HttpContext, (int)HttpStatusCode.ServiceUnavailable, context.Exception.Message);
                    context.Result = new ObjectResult(details)
                    {
                        StatusCode = (int)HttpStatusCode.ServiceUnavailable
                    };
                }

                context.Result = new ObjectResult(factory.CreateProblemDetails(context.HttpContext, (int)HttpStatusCode.InternalServerError, null, null, context.Exception.Message))
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}