using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Application.Common.Exceptions;

namespace MiniECommerce.Api.Middlewares;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(
                    exception,
                    "An exception occurred after the response had already started for {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = CreateProblemDetails(context, exception);

        LogException(context, exception, problemDetails.Status ?? StatusCodes.Status500InternalServerError);

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails, context.RequestAborted);
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException => CreateValidationProblemDetails(context, validationException),
            NotFoundException => CreateProblemDetails(
                context,
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message),
            UnauthorizedException or UnauthorizedAccessException => CreateProblemDetails(
                context,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                exception.Message),
            ForbiddenAccessException => CreateProblemDetails(
                context,
                StatusCodes.Status403Forbidden,
                "Forbidden",
                exception.Message),
            ConflictException => CreateProblemDetails(
                context,
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message),
            _ => CreateProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                environment.IsDevelopment() ? exception.Message : "An unexpected error occurred while processing the request.")
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        return problemDetails;
    }

    private static ProblemDetails CreateValidationProblemDetails(
        HttpContext context,
        ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray());

        var problemDetails = CreateProblemDetails(
            context,
            StatusCodes.Status400BadRequest,
            "Validation failed",
            "One or more validation errors occurred.");

        problemDetails.Extensions["errors"] = errors;

        return problemDetails;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };
    }

    private void LogException(HttpContext context, Exception exception, int statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            return;
        }

        logger.LogWarning(
            exception,
            "Handled exception with status {StatusCode} for {Method} {Path}. TraceId: {TraceId}",
            statusCode,
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);
    }
}
