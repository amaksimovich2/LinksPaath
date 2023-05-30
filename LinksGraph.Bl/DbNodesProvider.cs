using LinksGraph.Bl.Abstractions;

namespace LinksGraph.Bl
{
    internal class DbNodesProvider : INodesProvider<string>
    {
        public Task<IEnumerable<string>> GetLinkedNodes(string sourceNode)
        {
            throw new NotImplementedException();
        }
    }
}
