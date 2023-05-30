using LinksGraph.Bl;

Graph graph = new Graph();
int maxDepth = 3;
string baseUrlPrefix = "https://en.wikipedia.org";
string source = "https://en.wikipedia.org/wiki/Walton_Cardiff";
string destination = "https://en.wikipedia.org/wiki/Bristol";
INodesProvider<string> nodesProvider = new AnchorsDataProvider();

var queue = new Queue<string>();

await Tree(source);

//List<string> shortestPath = graph.ShortestPath(source, destination);

//Console.WriteLine("Shortest path from {0} to {1}:", source, destination);
//foreach (string node in shortestPath)
//{
//    Console.Write(node + " ");
//}

async Task BuildGraph(string url, int depth)
{
    if (depth == maxDepth) return;

    var nodes = await nodesProvider.GetLinkedNodes(url);

    // Add the link to the graph
    foreach (var link in nodes)
    {
        if (!string.IsNullOrWhiteSpace(link) && link.StartsWith("/wiki"))
        {
            graph.AddNode(url);
            graph.AddEdge(url, baseUrlPrefix + link);

            if (!graph.Contains(baseUrlPrefix + link))
            {
                // Recursively build the graph for the linked page
                await BuildGraph(baseUrlPrefix + link, depth + 1);
            }
        }
    }
}

async Task Tree(string url)
{
    var parentsMap = new Dictionary<string, string>();
    queue.Enqueue(url);

    int currentDepth = 0;

    while (queue.Count > 0)
    {
        int levelSize = queue.Count;

        if (currentDepth > maxDepth)
            break;

        for (int i = 0; i < levelSize; i++)
        {
            string currentNode = queue.Dequeue();
            //  Console.WriteLine(currentNode.Value);

            var nodes = await nodesProvider.GetLinkedNodes(currentNode);

            // Add the link to the graph
            foreach (var link in nodes)
            {
                if (!string.IsNullOrWhiteSpace(link) && link.StartsWith("/wiki"))
                {
                    if(baseUrlPrefix + link == destination)
                    {
                        Console.WriteLine("found. Level - " + currentDepth);
                        var path = new List<string> { destination };
                        string current = currentNode;

                        while (current != null)
                        {
                            path.Insert(0, current);
                            parentsMap.TryGetValue(current, out current);
                        }

                        foreach (string node in path)
                        {
                            Console.WriteLine(node);
                        }

                        return;
                    }

                    if (!graph.Contains(baseUrlPrefix + link) && baseUrlPrefix + link != currentNode)
                    {
                        graph.AddNode(currentNode);
                        graph.AddEdge(currentNode, baseUrlPrefix + link);
                        queue.Enqueue(baseUrlPrefix + link);
                        parentsMap.TryAdd(baseUrlPrefix + link, currentNode);
                    }
                }
            }
        }

        currentDepth++;
    }
}