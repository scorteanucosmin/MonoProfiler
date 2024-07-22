using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoVTable
{
    public MonoClass* klass;
    
    public void* gc_descr;
    
    public MonoDomain* domain;
    
    public void* mtype;
    
    public byte* interface_bitmap;
    
    public uint max_interface_id;
    
    public byte rank;
    
    public byte initialized;
    
    public byte flags;
    
    public uint remote;
    
    public uint init_failed;
    
    public uint has_static_fields;
    
    public uint gc_bits;
    
    public uint imt_collisions_bitmap;
    
    public MonoRuntimeGenericContext* runtime_generic_context;
    
    public void* interp_vtable;
    
    public void* vtable;
}