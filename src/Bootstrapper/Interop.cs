using System.Runtime.InteropServices;

namespace Bootstrapper;

public static class Interop
{
    [DllImport(@"MonoProfiler\MonoProfilerNE.dll", EntryPoint = "Initialize")]
    public static extern void InitializeMonoProfiler();
}