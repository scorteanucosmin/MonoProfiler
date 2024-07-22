using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoImage
{
    public int ref_count;

    public MonoImageStorage* storage;
    
    public char* raw_data;
    
    public uint raw_data_len;
    
    public fixed byte _dummyField[2];
    
    public char* name;
    
    public char* filename;
    
    public char* assembly_name;
    
    public char* module_name;
    
    public uint time_date_stamp;
    
    public char* version;
}