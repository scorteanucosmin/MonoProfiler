using System.Collections.Generic;
using MonoProfiler.Enums;

namespace MonoProfiler.Models.Communication;

public class RemoteMessage(ProfilerActionType profilerActionType, HashSet<string>? args = null)
{
    public ProfilerActionType ProfilerActionType { get; set; } = profilerActionType;

    public HashSet<string>? Args { get; set; } = args;
}