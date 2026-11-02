namespace WellTrack.Api.Middleware;

public class RequestLoggingMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger<RequestLoggingMiddleware> logger;

	public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
	{
		this.next = next;
		this.logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var startTime = DateTime.UtcNow;
		var requestPath = context.Request.Path;
		var requestMethod = context.Request.Method;

		logger.LogInformation("Incoming request: {Method} {Path}", requestMethod, requestPath);

		await next(context);

		var duration = DateTime.UtcNow - startTime;
		var statusCode = context.Response.StatusCode;

		logger.LogInformation(
			"Completed request: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms",
			requestMethod, requestPath, statusCode, duration.TotalMilliseconds);
	}
}
