using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct MonoTypeUnion
{
    [FieldOffset(0)]
    public MonoClass* klass;
    
    [FieldOffset(0)]
    public MonoType* type;
    
    [FieldOffset(0)]
    public MonoArrayType* array;
    
    [FieldOffset(0)]
    public MonoMethodSignature* method;
    
    [FieldOffset(0)]
    public MonoGenericParam* generic_param;
    
    [FieldOffset(0)]
    public MonoGenericClass* generic_class;
}