using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public struct MonoString
{
    public MonoObject monoObject;
    
    public int length;
    
    public ushort str;
}