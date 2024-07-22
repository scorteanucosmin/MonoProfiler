using System.Runtime.InteropServices;
using MonoProfiler.Enums;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoClass
{
    public MonoClass* element_class;

    public MonoClass* cast_class;

    public MonoClass** supertypes;

    public ushort idepth;

    public byte rank;

    public MonoTypeKind class_kind;

    public int instance_size;

    public fixed byte _dummyField[4];
    
    public byte min_align;

    public fixed byte _dummyField2[4];

    public MonoClass* parent;
    
    public MonoClass* nested_in;

    public MonoImage* image;
    
    public char* name;
    
    public char* name_space;

    public uint type_token;
    
    public int vtable_size;

    public ushort interface_count;
    
    public uint interface_id;
    
    public uint max_interface_id;
    
    public ushort interface_offsets_count;
    
    public MonoClass** interfaces_packed;
    
    public ushort* interface_offsets_packed;
    
    public byte* interface_bitmap;

    public MonoClass** interfaces;
    
    public MonoClassSizes sizes;

    public MonoClassField* fields;
    
    public MonoMethod** methods;
    
    public MonoType this_arg;
    
    public MonoType _byval_arg;
}