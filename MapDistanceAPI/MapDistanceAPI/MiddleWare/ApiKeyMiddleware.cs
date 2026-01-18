namespace MapDistanceAPI.MiddleWare
{
    public sealed class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        private const string HeaderName = "X-Api-Key";
        private const string ReadKey = "FS_Read";
        private const string ReadWriteKey = "FS_ReadWrite";

        public ApiKeyMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Only protect /api/map/* endpoints 
            if (context.Request.Path.StartsWithSegments("/api/map", StringComparison.OrdinalIgnoreCase))
            {
                if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
                    string.IsNullOrWhiteSpace(providedKey))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Missing API key.");
                    return;
                }

                var key = providedKey.ToString().Trim();

                // SetMap requires FS_ReadWrite 
                var isSetMap = context.Request.Path.Value?.EndsWith("/SetMap", StringComparison.OrdinalIgnoreCase) == true;

                if (isSetMap)
                {
                    if (!string.Equals(key, ReadWriteKey, StringComparison.Ordinal))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("API key does not grant permission for SetMap.");
                        return;
                    }
                }
                else
                {
                    // All other endpoints require FS_Read 
                    if (!string.Equals(key, ReadKey, StringComparison.Ordinal))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("API key does not grant permission.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}