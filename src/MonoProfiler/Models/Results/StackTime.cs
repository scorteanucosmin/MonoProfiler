using System.Diagnostics;

namespace MonoProfiler.Models.Results;

public struct StackTime
{
    public Stopwatch Timer { get; set; }
    
    public long OtherMethodsDuration { get; set; }

    public StackTime()
    {
        Timer = new Stopwatch();
        Timer.Start();
    }
}