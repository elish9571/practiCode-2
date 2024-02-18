using htmlSerializer;
using System;
using System.Collections.Generic;
using System.Linq;


namespace htmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        public HtmlElement() { }
        public HtmlElement(string name)
        {
            Name = name;
        }

        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> q = new Queue<HtmlElement>();
            q.Enqueue(this);

            while (q.Count > 0)
            {
                var currentElement = q.Dequeue();
                yield return currentElement;

                foreach (var child in currentElement.Children)
                {
                    q.Enqueue(child);
                }
            }

        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            var currentElement = this;
            while (currentElement.Parent != null)
            {
                yield return currentElement.Parent;
                currentElement = currentElement.Parent;
            }
        }
        // Method to add a child element
        public void AddChild(HtmlElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        // Method to display the HTML structure recursively
        public void DisplayHtmlStructure(string indent = "")
        {
            Console.WriteLine($"{indent}<{Name} {GetAttributesString()} {GetClassesString()}>");

            if (!string.IsNullOrEmpty(InnerHtml))
            {
                Console.WriteLine($"{indent} {InnerHtml}");
            }

            foreach (var child in Children)
            {
                child.DisplayHtmlStructure(indent + " ");
            }

            Console.WriteLine($"{indent}</{Name}>");
        }

        // Helper method to get attributes as a string
        private string GetAttributesString()
        {
            return Attributes.Count > 0 ? " " + string.Join(" ", Attributes) : "";
        }

        // Helper method to get classes as a string
        private string GetClassesString()
        {
            return Classes.Count > 0 ? "class=\"" + string.Join(" ", Classes) + "\"" : "";
        }

        // Method to query elements based on CSS selector
        public List<HtmlElement> QuerySelector(string selector)
        {
            var selectors = selector.Split(' ');

            List<HtmlElement> matchedElements = new List<HtmlElement> { this };

            foreach (var sel in selectors)
            {
                if (sel.StartsWith("#"))
                {
                    string id = sel.Substring(1);
                    matchedElements = matchedElements.Where(e => e.Id == id).ToList();
                }
                else if (sel.StartsWith("."))
                {
                    string className = sel.Substring(1);
                    matchedElements = matchedElements.Where(e => e.Classes.Contains(className)).ToList();
                }
                else
                {
                    string tagName = sel;
                    matchedElements = matchedElements.Where(e => e.Name == tagName).ToList();
                }
            }

            return matchedElements;
        }
    }
    public static class HtmlElementExtensions
    {
        public static HashSet<HtmlElement> FindElementsBySelector(this HtmlElement element, Selector selector)
        {
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            FindElementsRecursively(element, selector, result);
            return result;
        }
        private static void FindElementsRecursively(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            if (selector == null || string.IsNullOrEmpty(selector.TagName))
                return;

            var descendants = element.Descendants();
            var matchingDescendants = descendants.Where(e => IsElementMatchSelector(e, selector));

            if (selector.Child == null)
            {
                result.UnionWith(matchingDescendants);
                return;
            }

            // Recur for each matching descendant with the next selector
            foreach (var descendant in matchingDescendants)
            {
                FindElementsRecursively(descendant, selector.Child, result);
            }
        }

        private static bool IsElementMatchSelector(HtmlElement element, Selector selector)
        {
            if (!string.IsNullOrEmpty(selector.TagName) && !string.Equals(element.Name, selector.TagName, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrEmpty(selector.Id) && !string.Equals(element.Id, selector.Id, StringComparison.OrdinalIgnoreCase))
                return false;

            if (selector.Classes != null && selector.Classes.Any())
            {
                var elementClasses = new HashSet<string>(element.Classes, StringComparer.OrdinalIgnoreCase);
                if (!selector.Classes.All(elementClasses.Contains))
                    return false;
            }

            // If all criteria match, return true
            return true;
        }
    }
}

