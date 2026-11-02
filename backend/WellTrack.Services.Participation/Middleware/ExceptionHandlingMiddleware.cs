using System.Net;
using System.Text.Json;
using WellTrack.SharedKernel.Exceptions;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Participation.Middleware;

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
		try { await next(context); }
		catch (Exception ex) { await HandleExceptionAsync(context, ex); }
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		logger.LogError(exception, "An error occurred: {Message}", exception.Message);

		var response = context.Response;
		response.ContentType = "application/json";
		var apiResponse = new ApiResponse<object>();

		switch (exception)
		{
			case NotFoundException nfe:
				response.StatusCode = (int)HttpStatusCode.NotFound;
				apiResponse.Message = nfe.Message;
				break;
			case ForbiddenException fe:
				response.StatusCode = (int)HttpStatusCode.Forbidden;
				apiResponse.Message = fe.Message;
				break;
			case ValidationException ve:
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				apiResponse.Message = "Validation failed";
				apiResponse.Errors = ve.Errors;
				break;
			case UnauthorizedAccessException:
				response.StatusCode = (int)HttpStatusCode.Unauthorized;
				apiResponse.Message = exception.Message;
				break;
			default:
				response.StatusCode = (int)HttpStatusCode.InternalServerError;
				apiResponse.Message = "An internal server error occurred";
				break;
		}

		await response.WriteAsync(JsonSerializer.Serialize(apiResponse,
			new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
	}
}
