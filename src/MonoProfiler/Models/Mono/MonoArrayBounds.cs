using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public struct MonoArrayBounds
{
    public uint length;
    
    public int lower_bound;
}