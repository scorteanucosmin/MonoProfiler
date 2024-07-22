using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MonoProfiler.Common;
using MonoProfiler.Enums;
using MonoProfiler.Models.Communication;

namespace MonoProfiler.Handlers;

//TODO: optimize & cleanup
public class WebSocketServerHandler : Singleton<WebSocketServerHandler>
{
    private readonly byte[] _receivedMessageBuffer = new byte[1024];
    private readonly MemoryStream _memoryStream = new();
    private WebSocket? _webSocket;
    
    public async ValueTask InitializeAsync()
    {
        using HttpListener httpListener = new();
        //TODO: configurable
        httpListener.Prefixes.Add("http://localhost:5000/");
        httpListener.Start();

        while (true)
        {
            HttpListenerContext httpListenerContext = await httpListener.GetContextAsync();
            if (!httpListenerContext.Request.IsWebSocketRequest)
            {
                httpListenerContext.Response.StatusCode = 400;
                httpListenerContext.Response.Close();
                continue;
            }

            await HandleClientsAsync(httpListenerContext);
        }
    }

    public async ValueTask SendDataAsync<T>(T data)
    {
        if (_webSocket == null)
        {
            return;
        }
        
        await _webSocket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(data), WebSocketMessageType.Binary, true, CancellationToken.None);
    }
    
    private async ValueTask HandleClientsAsync(HttpListenerContext context)
    {
        WebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
        _webSocket = webSocketContext.WebSocket;
        
        ArraySegment<byte> arraySegment = new(_receivedMessageBuffer);
        try
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult receivedMessage;
                _memoryStream.SetLength(0);

                do
                {
                    receivedMessage = await _webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
                    _memoryStream.Write(arraySegment.Array, arraySegment.Offset, receivedMessage.Count);
                } while (!receivedMessage.EndOfMessage);

                _memoryStream.Seek(0, SeekOrigin.Begin);
                
                switch (receivedMessage.MessageType)
                {
                    case WebSocketMessageType.Close:
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        break;
                    }
                    case WebSocketMessageType.Binary:
                    {
                        Profiler profiler = Profiler.Instance;
                        RemoteMessage? remoteMessage = JsonSerializer.Deserialize<RemoteMessage>(_memoryStream);
                        if (remoteMessage == null)
                        {
                            break;
                        }
                        
                        switch (remoteMessage.ProfilerActionType)
                        {
                            case ProfilerActionType.Toggle:
                            {
                                profiler.Toggle(remoteMessage.Args);
                                break;
                            }
                        }
                        
                        break;
                    }
                }
            }
        }
        catch (Exception)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}