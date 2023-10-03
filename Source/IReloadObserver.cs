namespace BestApparel;

public interface IReloadObserver
{
    void OnDataProcessorReloaded(ReloadPhase phase);
}

public enum ReloadPhase
{
    Changed,
    Filtered,
    Sorted,
    Rendered
}