namespace LinksGraph.Bl.Abstractions
{
    public interface INodesProvider<T>
    {
        public Task<IEnumerable<T>> GetLinkedNodes(T sourceNode);
    }
}
