using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoMethod
{
    public ushort flags;
    
    public ushort iflags;
    
    public uint token;
    
    public MonoClass* klass;
    
    public void* signature;
    
    public char* name;
}