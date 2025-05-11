// GroupApi/Middleware/WebSocketMiddleware.cs
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using GroupApi.Services.WebSocket;

namespace GroupApi.Middleware
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
            {
                // Create a scope to resolve scoped services
                using var scope = context.RequestServices.CreateScope();
                var webSocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();

                var socket = await context.WebSockets.AcceptWebSocketAsync();
                await webSocketService.AddSocketAsync(socket);
            }
            else
            {
                await _next(context);
            }
        }
    }
}