using System;
using System.IO;
using System.Text.Json;

class HtmlHelper
{
    private readonly static HtmlHelper _instance = new HtmlHelper();
    public static HtmlHelper Instance=>_instance;
    // Properties to store HTML tags and self-closing tags
    public string[] AllHtmlTags { get; private set; }
    public string[] SelfClosingTags { get; private set; }

    // Constructor
    private HtmlHelper()
    {
        string htmlTagsJson = File.ReadAllText("htmlTags.json");
        AllHtmlTags = JsonSerializer.Deserialize<string[]>(htmlTagsJson);

        // Load self-closing tags from JSON file
        string selfClosingTagsJson = File.ReadAllText("HtmlVoidTags.json");
        SelfClosingTags = JsonSerializer.Deserialize<string[]>(selfClosingTagsJson);
    }

}

