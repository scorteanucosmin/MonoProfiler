namespace Profiler.Common;

public static class Constants
{
    public static readonly string LocalStoragePath =  
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "profiler\\storage.json");
}