namespace MonoProfiler.Enums;

public enum MonoProfilerGCEvent
{
    PreStopWorld = 6,
    PreStopWorldLocked = 10,
    PostStopWorld = 7,
    Start = 0,
    End = 5,
    PreStartWorld = 8,
    PostStartWorldUnlocked = 11,
    PostStartWorld = 9,
}