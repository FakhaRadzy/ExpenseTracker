using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.ErrorHandler
{
    public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
        {
            ProblemDetails problemDetails = exception switch
            {
                ValidationException validationException => new ValidationProblemDetails(validationException.Errors.GroupBy(error => error.PropertyName).ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray()))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred."
                },

                DomainException => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "The request breaks a business rule.",
                    Detail = exception.Message
                },

                ConflictException => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "The request conflicts with the current state",
                    Detail = exception.Message
                },

                // Anything else is a bug. Never send exception.Message to the client: it can leak internals.
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred."
                }

            }; 

            if (problemDetails.Status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
            }

            httpContext.Response.StatusCode = problemDetails.Status!.Value;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext { HttpContext = httpContext, ProblemDetails = problemDetails, Exception = exception });
        }
    }
}
