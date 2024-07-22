using MonoProfiler.Models;
using MonoProfiler.Models.Communication;

namespace Profiler.Interfaces;

public interface IWebSocketClientHandler
{
    public ValueTask ConnectAsync();

    public ValueTask DisconnectAsync();
    
    public ValueTask SendAsync(RemoteMessage remoteMessage);

    public event Action<ProfilingSample>? OnProfilingSampleReceived;
}