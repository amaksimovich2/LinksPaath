namespace LinksGraph.Bl
{
    public class Graph
    {
        private int _nodesCount; // Number of nodes
        private Dictionary<string, List<string>> adjList; // Adjacency list

        public Graph()
        {
            _nodesCount = 0;
            adjList = new Dictionary<string, List<string>>();
        }

        public bool Contains(string node) => adjList.ContainsKey(node);

        public void AddNode(string node)
        {
            if (!adjList.ContainsKey(node))
            {
                adjList[node] = new List<string>();
                _nodesCount++;
            }
        }

        public void AddEdge(string source, string destination)
        {
            if (adjList.ContainsKey(source) && adjList.ContainsKey(destination))
            {
                adjList[source].Add(destination);
                //adjList[destination].Add(source); // For an undirected graph
            }
        }

        public List<string> ShortestPath(string source, string destination)
        {
            Dictionary<string, int> distance = new();
            Dictionary<string, string> previous = new();
            HashSet<string> visited = new();

            foreach (string node in adjList.Keys)
            {
                distance[node] = int.MaxValue;
                previous[node] = null;
            }

            distance[source] = 0;

            for (int count = 0; count < _nodesCount - 1; count++)
            {
                string nearestNode = FindNearestNode(distance, visited);
                visited.Add(nearestNode);

                foreach (string node in adjList[nearestNode])
                {
                    if (!visited.Contains(node) && distance[nearestNode] != int.MaxValue && distance[nearestNode] + 1 < distance[node])
                    {
                        distance[node] = distance[nearestNode] + 1;
                        previous[node] = nearestNode;
                    }
                }
            }

            return ReconstructPath(previous, destination);
        }

        private string FindNearestNode(Dictionary<string, int> distance, HashSet<string> visited)
        {
            int minDistance = int.MaxValue;
            string? minNode = null;

            foreach (KeyValuePair<string, int> distanceData in distance)
            {
                string node = distanceData.Key;
                int nodeDistance = distanceData.Value;

                if (!visited.Contains(node) && nodeDistance <= minDistance)
                {
                    minDistance = nodeDistance;
                    minNode = node;
                }
            }

            return minNode;
        }

        private List<string> ReconstructPath(Dictionary<string, string?> previous, string destination)
        {
            List<string> path = new();
            string current = destination;

            while (current != null)
            {
                path.Add(current);
                current = previous[current];
            }

            path.Reverse();
            return path;
        }
    }
}