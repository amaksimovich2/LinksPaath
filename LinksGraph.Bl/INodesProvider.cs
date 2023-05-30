namespace LinksGraph.Bl
{
    public interface INodesProvider<T>
    {
        public Task<IEnumerable<T>> GetLinkedNodes(T sourceNode);
    }
}
