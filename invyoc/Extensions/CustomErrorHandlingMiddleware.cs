namespace invyoc.Extensions;

public class CustomErrorHandlingMiddleware(
    RequestDelegate next, 
    ILogger<CustomErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<CustomErrorHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // Proceed to next middleware
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.Redirect("/Home/Error");
        }
    }
}