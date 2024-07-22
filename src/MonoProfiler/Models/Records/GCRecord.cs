using System.Diagnostics;
using MonoProfiler.Common;

namespace MonoProfiler.Models.Records;

public struct GCRecord
{
    public int Calls { get; set; }
    
    public long TotalTime { get; set; }
    
    public long TotalTimeMs => (long)(TotalTime * Constants.NanosecondsMultiplier);
    
    public Stopwatch Timer { get; set; }
}