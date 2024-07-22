namespace MonoProfiler.Enums;

public enum MonoTypeKind : byte
{
    Def = 1,
    Gtd,
    Ginst,
    Gparam,
    Array,
    Pointer,
    GcFiller = 0xAC
}