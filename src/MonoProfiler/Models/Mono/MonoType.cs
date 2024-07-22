using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoType
{
    public MonoTypeUnion data;
    
    public fixed byte _dummyField[5];
}