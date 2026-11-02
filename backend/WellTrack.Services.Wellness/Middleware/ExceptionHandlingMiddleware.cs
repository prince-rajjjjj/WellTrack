using System.Net;
using System.Text.Json;
using WellTrack.SharedKernel.Exceptions;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Wellness.Middleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger<ExceptionHandlingMiddleware> logger;

	public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	{
		this.next = next;
		this.logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		logger.LogError(exception, "An error occurred: {Message}", exception.Message);

		var response = context.Response;
		response.ContentType = "application/json";

		var apiResponse = new ApiResponse<object>();

		switch (exception)
		{
			case NotFoundException notFoundException:
				response.StatusCode = (int)HttpStatusCode.NotFound;
				apiResponse.Message = notFoundException.Message;
				break;

			case ForbiddenException forbiddenException:
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				apiResponse.Message = forbiddenException.Message;
				break;

			case ValidationException validationException:
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				apiResponse.Message = "Validation failed";
				apiResponse.Errors = validationException.Errors;
				break;

			case UnauthorizedAccessException:
				response.StatusCode = (int)HttpStatusCode.Unauthorized;
				apiResponse.Message = exception.Message;
				break;

			case KeyNotFoundException:
				response.StatusCode = (int)HttpStatusCode.NotFound;
				apiResponse.Message = exception.Message;
				break;

			case InvalidOperationException:
			case ArgumentException:
				// State-transition failures, capacity full, mode-validation, etc.
				response.StatusCode = (int)HttpStatusCode.Conflict;
				apiResponse.Message = exception.Message;
				break;

			default:
				response.StatusCode = (int)HttpStatusCode.InternalServerError;
				apiResponse.Message = "An internal server error occurred";
				break;
		}

		var result = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		});

		await response.WriteAsync(result);
	}
}
