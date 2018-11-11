namespace MiniEngine.Pipeline
{
    public interface IPipelineStage<T>
    {
        void Execute(T input);
    }
}