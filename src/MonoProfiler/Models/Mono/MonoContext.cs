using System;
using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoContext
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public UIntPtr[] regs;
    
    public byte* ip;
    
    public void** sp;
    
    public void** fp;
}