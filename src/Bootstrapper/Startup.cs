namespace Bootstrapper;

public class Startup : IHarmonyModHooks
{
    public void OnLoaded(OnHarmonyModLoadedArgs args) => Interop.InitializeMonoProfiler();

    public void OnUnloaded(OnHarmonyModUnloadedArgs args) { }
}