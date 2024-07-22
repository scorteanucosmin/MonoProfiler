using System;

namespace MonoProfiler.Enums;

[Flags]
public enum CallInstrumentationFlags : byte
{
    None = 0,
    Enter = 1 << 1,
    EnterContext = 1 << 2,
    Leave = 1 << 3,
    LeaveContext = 1 << 4,
    TailCall = 1 << 5,
    ExceptionLeave = 1 << 6
}