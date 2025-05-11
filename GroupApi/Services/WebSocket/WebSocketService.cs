// GroupApi/Services/WebSocket/WebSocketService.cs
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GroupApi.Services.WebSocket
{
    public class WebSocketService
    {
        private readonly ConcurrentDictionary<Guid, System.Net.WebSockets.WebSocket> _sockets = new();

        public async Task AddSocketAsync(System.Net.WebSockets.WebSocket socket)
        {
            var socketId = Guid.NewGuid();
            _sockets.TryAdd(socketId, socket);

            try
            {
                await HandleSocketAsync(socket, socketId);
            }
            catch
            {
                _sockets.TryRemove(socketId, out _);
            }
        }

        private async Task HandleSocketAsync(System.Net.WebSockets.WebSocket socket, Guid socketId)
        {
            while (socket.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    _sockets.TryRemove(socketId, out _);
                    break;
                }
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            foreach (var socket in _sockets.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}