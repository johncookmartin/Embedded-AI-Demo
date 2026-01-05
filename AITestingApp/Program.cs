using EmbeddedAILibrary;
using System.Text.Json;

Console.WriteLine("This is the AI Generator Test App");
Console.ReadLine();
Console.WriteLine("Running...");

string modelName = @"D:\models\Llama-3.2-3B-Instruct-Q4_K_M.gguf";
int numberOfRecords = 50;
string sampleString = """
    {
        "Id": 0,
        "FirstName": "",
        "LastName": "",
        "EmailAddress": ""
    }
    """;
JsonDocument sampleDocument = JsonDocument.Parse(sampleString);

AIGenerator generator = new(modelName);

var output = await generator.GetSampleDataAsync(numberOfRecords, sampleDocument);

JsonSerializerOptions options = new() { WriteIndented = true };
string finalJson = JsonSerializer.Serialize(output, options);

Console.WriteLine();
Console.WriteLine(finalJson);
Console.WriteLine();

Console.WriteLine("Press any key to close...");
Console.ReadLine();