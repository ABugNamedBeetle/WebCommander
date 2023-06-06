using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using WebCommander.App.Controller;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace WebCommander.App;

public class Data
{
    private string? websocket;
    private List<Query> queries = new List<Query>();

    [JsonPropertyName("websocket") ]
    public string Websocket { get => websocket ?? string.Empty; set => websocket = value; }

    [JsonPropertyName("queries")]
    public List<Query> Queries { get => queries; set => queries = value; }

    public static Data getDataFromJSON(string fileName)
    {
        Data d = new Data();
        try
        {

            //doing a manual deser due to 
            /*
                warning IL2026: Using member 'System.Text.Json.JsonSerialize r.Deserialize<TValue>(String, JsonSerializerOptions)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when  
                trimming application code. JSON serialization and deserialization might require types that cannot be statically analyzed. Use the 
                overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved. [D:\Arpit Sha 
                rma\Downloads\ApritProj\WebCommander\WebCommander.csproj]
            */
            // using FileStream openStream = File.OpenRead(fileName);
            // Data? data = await JsonSerializer.DeserializeAsync<Data?>(openStream);
            string jsonString = File.ReadAllText(fileName);

            d = JsonSerializer.Deserialize<Data>(jsonString, WebCommanderGenerationContext.Default.Data)!;
            //@more at 
            //https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation


            //  string json = File.ReadAllText(fileName);
            // JsonDocumentOptions options = new JsonDocumentOptions { AllowTrailingCommas = true };
            // JsonDocument document = JsonDocument.Parse(json, options);
            // JsonElement root = document.RootElement;
            // d.Websocket = root.GetProperty("websocket").GetString()!;



        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Failed while reading init.json");
        }

        var sourceGenOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = WebCommanderGenerationContext.Default
        };
        // string ng = JsonSerializer.Serialize(d!, typeof(Data), sourceGenOptions );
        // Console.WriteLine(ng);

        return d;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        string queryValues = string.Join(", ", Queries.Select(query => query.ToString()));

        return string.Format("Data:[ Websocket=\"{0}\", Queries={1} ]", Websocket, queryValues);
    }
}


[JsonSourceGenerationOptions(WriteIndented = true)] // , PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
[JsonSerializable(typeof(Data))]
[JsonSerializable(typeof(Query))]
internal partial class WebCommanderGenerationContext : JsonSerializerContext
{
}

