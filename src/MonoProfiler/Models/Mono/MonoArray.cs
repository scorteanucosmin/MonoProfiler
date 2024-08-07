﻿using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoArray
{
    public MonoObject monoObject;
    
    public MonoArrayBounds bounds;
    
    public uint length;
    
    public void* data;
}