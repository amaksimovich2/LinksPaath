using LinksGraph.Bl.Abstractions;
using LinksGraph.Bl.Models;
using System.Collections.Concurrent;

namespace LinksGraph.Bl
{
    public class BfsTreeConcurrentPathFinder<TType> : IPathFinder<TType> where TType : notnull
    {
        private readonly INodesProvider<TType> _nodesProvider;
        private ConcurrentDictionary<TType, IEnumerable<TType>> _visitedNodes;
        private readonly int _maxConcurrentThreads;

        public BfsTreeConcurrentPathFinder(INodesProvider<TType> nodesProvider, int maxConcurrentThreads)
        {
            _nodesProvider = nodesProvider;
            _visitedNodes = new ConcurrentDictionary<TType, IEnumerable<TType>>();
            _maxConcurrentThreads = maxConcurrentThreads;
        }

        public async Task<(bool, IEnumerable<TType>?)> FindPathAsync(TType source, TType destination,
            Func<IEnumerable<TType>, IEnumerable<TType>> mutateNodeFn, int maxDepth = 10)
        {
            //Create root node from the source element
            var Root = new TreeNode<TType>(source);
            //Reassign visitedNodes before each iteration
            _visitedNodes = new ConcurrentDictionary<TType, IEnumerable<TType>>();

            //Bfs method use queue, we store tuple with node, depth and path
            var queue = new ConcurrentQueue<(TreeNode<TType> node, int depth, List<TType>? path)>();
            //Add root node to the queue
            queue.Enqueue((Root, 0, new List<TType> { Root.Value }));

            //Semaphore for concurrency control
            var semaphore = new SemaphoreSlim(_maxConcurrentThreads);

            while (!queue.IsEmpty)
            {
                var childTasks = new List<Task<(bool, List<TType>?)>>();

                while (queue.TryDequeue(out var item))
                {
                    //Check if node was already visited
                    if (!_visitedNodes.ContainsKey(item.node.Value))
                    {
                        //Add separate task for each available node
                        childTasks.Add(Task.Run<(bool, List<TType>?)>(async () =>
                        {
                            //Wait available threads from the pool
                            await semaphore.WaitAsync();

                            //Extract data from queue element
                            var currentNode = item.node;
                            var depth = item.depth;
                            var path = item.path;

                            Console.WriteLine(currentNode.Value);

                            try
                            {
                                if (depth < maxDepth)
                                {
                                    //Get child nodes
                                    var children = await _nodesProvider.GetLinkedNodes(currentNode.Value);

                                    //Since this class is generic we probably need to mutate child nodes somehow
                                    children = mutateNodeFn(children);

                                    //Store visited node for preventing fetching the same resources
                                    _visitedNodes.TryAdd(currentNode.Value, children);

                                    //Try to find destination in child nodes, cause it's quicker than waiting until we will dequeue all nodes 
                                    if (children.Contains(destination))
                                    {
                                        return (true, path);
                                    }

                                    //Add all child nodes to the queue and increase depth
                                    foreach (var childValue in children)
                                    {
                                        var childNode = new TreeNode<TType>(childValue);
                                        var childPath = new List<TType>(path) { childValue };
                                        queue.Enqueue((childNode, depth + 1, childPath));
                                    }
                                }
                                //Finish searching if depth was reached
                                return (false, null);
                            }
                            finally
                            {
                                //Release threads
                                semaphore.Release();
                            }
                        }));
                    } else
                    {
                        Console.WriteLine("Dupl");
                    }
                }

                //Check if destination node was found
                //We can await tasks result if we need consistency, but it will decrease searching time. Can be changed if we need different behavior
                var taskResult = Task.WhenAll(childTasks);
                foreach (var task in childTasks)
                {
                    if (task.Result.Item1 == true) return task.Result;
                }
            }

            //Finish without result
            return (false, null);
        }
    }
}
