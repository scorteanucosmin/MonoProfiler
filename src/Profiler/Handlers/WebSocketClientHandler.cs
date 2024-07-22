using System.Net.WebSockets;
using System.Text.Json;
using MonoProfiler.Models;
using MonoProfiler.Models.Communication;
using Profiler.Interfaces;

namespace Profiler.Handlers;

//TODO: optimize & cleanup
public class WebSocketClientHandler : IWebSocketClientHandler
{
    private readonly byte[] _receivedMessageBuffer = new byte[8192];
    private readonly MemoryStream _memoryStream = new();
    private ClientWebSocket? _webSocketClient;
    public event Action<ProfilingSample>? OnProfilingSampleReceived;

    public async ValueTask ConnectAsync()
    {
        if (_webSocketClient != null)
        {
            return;
        }

        _webSocketClient = new ClientWebSocket();
        await _webSocketClient.ConnectAsync(new Uri("ws://localhost:5000/"), CancellationToken.None);
        await StartReceiving();
    }

    private async ValueTask StartReceiving()
    {
        ArraySegment<byte> arraySegment = new(_receivedMessageBuffer);
        
        while (_webSocketClient.State == WebSocketState.Open)
        {
            WebSocketReceiveResult receivedMessage;
            _memoryStream.SetLength(0);

            do
            {
                receivedMessage = await _webSocketClient.ReceiveAsync(arraySegment, CancellationToken.None);
                _memoryStream.Write(arraySegment.Array, arraySegment.Offset, receivedMessage.Count);
            } while (!receivedMessage.EndOfMessage);

            _memoryStream.Seek(0, SeekOrigin.Begin);
            
            switch (receivedMessage.MessageType)
            {
                case WebSocketMessageType.Close:
                {
                    await _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty,
                        CancellationToken.None);
                    break;
                }
                case WebSocketMessageType.Binary:
                {
                    ProfilingSample profilingSample = JsonSerializer.Deserialize<ProfilingSample>(_memoryStream);
                    OnProfilingSampleReceived?.Invoke(profilingSample);
                    break;
                }
            }
        }
    }

    public async ValueTask SendAsync(RemoteMessage remoteMessage)
    {
        await _webSocketClient.SendAsync(JsonSerializer.SerializeToUtf8Bytes(remoteMessage), 
            WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    public async ValueTask DisconnectAsync()
    {
        await _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, 
            CancellationToken.None);
        _webSocketClient = null;
    }
}