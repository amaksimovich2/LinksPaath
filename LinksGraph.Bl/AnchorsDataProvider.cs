using HtmlAgilityPack;

namespace LinksGraph.Bl
{
    public class AnchorsDataProvider: INodesProvider<string>
    {
        public async Task<IEnumerable<string>> GetLinkedNodes(string url)
        {
            var htmlDocument = await GetPageData(url);

            return htmlDocument == null ? new List<string>() : FindAnchors(htmlDocument);
        }

        private async Task<HtmlDocument?> GetPageData(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the specified URL and retrieve the response
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Ensure the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string html = await response.Content.ReadAsStringAsync();

                        // Load the HTML content into HtmlDocument
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(html);

                        return document;
                    }
                    else
                    {
                        Console.WriteLine("Request was not successful. Status code: " + response.StatusCode);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    throw;
                }
            }
        }

        private IEnumerable<string> FindAnchors(HtmlDocument document)
        {
            // Find all the anchor tags in the HTML
            IEnumerable<HtmlNode> anchorTags = document.DocumentNode.Descendants("a");

            return anchorTags.Select(x => x.GetAttributeValue("href", ""));
        }
    }
}
