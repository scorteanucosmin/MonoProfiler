using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Explicit)]
public struct MonoClassSizes
{
    [FieldOffset(0)]
    public int class_size;
    
    [FieldOffset(0)]
    public int element_size;
    
    [FieldOffset(0)]
    public int generic_param_token;
}