namespace LinksGraph.Bl.Abstractions
{
    public interface IPathFinder<TType>
    {
        Task<(bool, IEnumerable<TType>?)> FindPathAsync(TType source, TType destination, 
            Func<IEnumerable<TType>, IEnumerable<TType>> mutateNodeFn, int maxDepth = 10);
    }
}
