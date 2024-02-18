using htmlSerializer;
using System.Text.RegularExpressions;

var html = await Load("https://www.discountbank.co.il//");
var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*)>").Split(html);
static HtmlElement BuildTree(List<string> htmlLines)
{
    var root = new HtmlElement();
    var currentElement = root;

    foreach (var line in htmlLines)
    {
        var firstWord = line.Split(' ')[0];

        if (firstWord == "/html")
        {
            break; // Reached end of HTML
        }
        else if (firstWord.StartsWith("/"))
        {
            if (currentElement.Parent != null) // Make sure there is a valid parent
            {
                currentElement = currentElement.Parent; // Go to previous level in the tree
            }
        }
        else if (HtmlHelper.Instance.AllHtmlTags.Contains(firstWord))
        {
            var newElement = new HtmlElement();
            newElement.Name = firstWord;

            // Handle attributes
            var restOfString = line.Remove(0, firstWord.Length);
            var attributes = Regex.Matches(restOfString, "([a-zA-Z]+)=\\\"([^\\\"]*)\\\"")
                .Cast<Match>()
                .Select(m => $"{m.Groups[1].Value}=\"{m.Groups[2].Value}\"")
                .ToList();

            if (attributes.Any(attr => attr.StartsWith("class")))
            {
                var attributesClass = attributes.First(attr => attr.StartsWith("class"));
                var classes = attributesClass.Split('=')[1].Trim('"').Split(' ');
                newElement.Classes.AddRange(classes);
            }

            newElement.Attributes.AddRange(attributes);


            var idAttribute = attributes.FirstOrDefault(a => a.StartsWith("id"));
            if (!string.IsNullOrEmpty(idAttribute))
            {
                newElement.Id = idAttribute.Split('=')[1].Trim('"');
            }

            newElement.Parent = currentElement;
            currentElement.Children.Add(newElement);

            // Check if self-closing tag
            if (line.EndsWith("/") || HtmlHelper.Instance.SelfClosingTags.Contains(firstWord))
            {
                currentElement = newElement.Parent;
            }
            else
            {
                currentElement = newElement;
            }
        }
        else
        {
            // Text content
            currentElement.InnerHtml = line;
        }
    }

    return root;
}
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
// Example usage of HtmlHelper class
var rootroot = BuildTree(htmlLines.ToList());
rootroot.DisplayHtmlStructure();    
string[] allTagsJsonPath = HtmlHelper.Instance.AllHtmlTags;
string[] selfClosingTagsJsonPath = HtmlHelper.Instance.SelfClosingTags;

HtmlElement root = new HtmlElement();
HtmlElement currentElement = root;

{   // Regular expression to match HTML tag name and attributes
    var newElement = new HtmlElement();

    // Loop over HTML lines
    foreach (var htmlLine in htmlLines)
    {
        // Extract the first word

        string firstWord = htmlLine.Split(' ')[0];
        string restOfString = htmlLine.Remove(0, firstWord.Length);

        // Check conditions based on the first word
        if (firstWord == "/html")
        {
            break;
        }
        else if (firstWord.StartsWith("/"))
        {
            // Closing tag
            if (currentElement.Parent != null)
                currentElement = currentElement.Parent;
        }
        else if (allTagsJsonPath.Contains(firstWord))
        {
            newElement = new HtmlElement();
            newElement.Name = firstWord;

            // Handle attributes
            var attributes = Regex.Matches(restOfString, "([a-zA-Z]+)=\\\"([^\\\"]*)\\\"")
                .Cast<Match>()
                .Select(m => $"{m.Groups[1].Value}=\"{m.Groups[2].Value}\"")
                .ToList();

            if (attributes.Any(attr => attr.StartsWith("class")))
            {
                // Handle class attribute
                var attributesClass = attributes.First(attr => attr.StartsWith("class"));
                var classes = attributesClass.Split('=')[1].Trim('"').Split(' ');
                newElement.Classes.AddRange(classes);
            }

            newElement.Attributes.AddRange(attributes);

            // Handle ID
            var idAttribute = attributes.FirstOrDefault(a => a.StartsWith("id"));
            if (!string.IsNullOrEmpty(idAttribute))
            {
                newElement.Id = idAttribute.Split('=')[1].Trim('"');
            }
            newElement.Parent = currentElement;

            if (currentElement != null)
            {
                currentElement.Children.Add(newElement);
            }


            if (htmlLine.EndsWith("/") || selfClosingTagsJsonPath.Contains(firstWord))
            {
                if(currentElement != null) 
                currentElement = currentElement.Parent;
            }
            else
            {
                currentElement = newElement;
            }
        }

        else
        {
            currentElement.InnerHtml = htmlLine;
        }
    }

    // Display the HTML tree structure
    root.DisplayHtmlStructure();

}


