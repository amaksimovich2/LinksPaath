using LinksGraph.Bl;
using LinksGraph.Bl.Abstractions;
using System.Diagnostics;

int maxDepth = 10;
int maxThreads = 10;
string baseUrlPrefix = "https://en.wikipedia.org";
string source = "https://en.wikipedia.org/wiki/Walton_Cardiff";
string destination = "https://en.wikipedia.org/wiki/Bristol";
INodesProvider<string> nodesProvider = new AnchorsDataProvider();
IPathFinder<string> pathFinder = new BfsTreeConcurrentPathFinder<string>(nodesProvider, maxThreads);
//IPathFinder<string> pathFinder = new BfsTreePathFinder<string>(nodesProvider, maxThreads);
Stopwatch stopwatch = new Stopwatch();

stopwatch.Start();
var (found, path) = await pathFinder.FindPathAsync(source, destination, NodeMutationFn);
stopwatch.Stop();

PrintResults((found, path));

Console.WriteLine("Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");


//Skip empty links and select only internal wiki links
IEnumerable<string> NodeMutationFn (IEnumerable<string> nodes) => nodes.Where(x => !string.IsNullOrWhiteSpace(x) && x.StartsWith("/wiki")).Select(x => baseUrlPrefix + x);

void PrintResults((bool, IEnumerable<string>) result)
{
    if (result.Item1)
    {
        Console.WriteLine("Path found:");
        foreach (var value in result.Item2)
        {
            Console.WriteLine(value);
        }
        Console.WriteLine(destination);
    }
    else
    {
        Console.WriteLine("Element not found.");
    }
}