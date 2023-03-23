namespace BestApparel;

public interface IContainerPostprocess
{
    void Postprocess(AThingContainer container, IThingTabRenderer renderer);
}