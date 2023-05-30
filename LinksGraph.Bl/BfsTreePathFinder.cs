using LinksGraph.Bl.Abstractions;

namespace LinksGraph.Bl
{
    public class BfsTreePathFinder<TType> : IPathFinder<TType> where TType : notnull
    {
        private readonly INodesProvider<TType> _nodesProvider;
        private Dictionary<TType, IEnumerable<TType>> _visitedNodes;

        public BfsTreePathFinder(INodesProvider<TType> nodesProvider)
        {
            _nodesProvider = nodesProvider;
            _visitedNodes = new Dictionary<TType, IEnumerable<TType>>();
        }

        public async Task<(bool, IEnumerable<TType>?)> FindPathAsync(TType source, TType destination, 
            Func<IEnumerable<TType>, IEnumerable<TType>> mutateNodeFn, int maxDepth = 10)
        {
            //Dictionary for storing current path 
            var parentsMap = new Dictionary<TType, TType>();
            //Bfs tree queue
            var queue = new Queue<TType>();
            queue.Enqueue(source);

            int currentDepth = 0;

            while (queue.Count > 0)
            {
                int levelSize = queue.Count;

                if (currentDepth > maxDepth)
                    return (false, null);

                //Take child nodes for all nodes from current level
                for (int i = 0; i < levelSize; i++)
                {
                    TType currentNode = queue.Dequeue();
                    Console.WriteLine(currentNode);

                    //Get child nodes
                    var nodes = await _nodesProvider.GetLinkedNodes(currentNode);

                    //Since this class is generic we probably need to mutate child nodes somehow
                    nodes = mutateNodeFn(nodes);

                    //Store visited node for preventing fetching the same resources
                    _visitedNodes.TryAdd(currentNode, nodes);

                    // Add the link to the graph
                    foreach (var link in nodes)
                    {
                        //Destination element was found
                        if (link.Equals(destination))
                        {
                            var path = new List<TType>();
                            TType current = currentNode;

                            //Build path from parents map dictionary
                            while (current != null)
                            {
                                path.Insert(0, current);
                                parentsMap.TryGetValue(current, out current);
                            }

                            return (true, path);
                        }

                        //Add child node to the queue
                        if (!_visitedNodes.ContainsKey(link))
                        {
                            queue.Enqueue(link);
                            parentsMap.TryAdd(link, currentNode);
                        }
                    }
                }

                //Increase current depth
                currentDepth++;
            }

            //Destination not found
            return (false, null);
        }
    }
}
