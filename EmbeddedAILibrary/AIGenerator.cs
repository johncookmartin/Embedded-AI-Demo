using LLama;
using LLama.Common;
using LLama.Sampling;
using System.Text;
using System.Text.Json;

namespace EmbeddedAILibrary;

public class AIGenerator
{
    private readonly string _modelPath;
    public AIGenerator(string modelPath)
    {
        // usually don't throw exceptions in constructors
        // but for this example, nothing will work without a valid model file
        // so it's okay to do so here
        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException("Model file not found");
        }

        _modelPath = modelPath;
    }

    public async Task<JsonDocument> GetSampleDataAsync(int recordCount, JsonDocument sampleDocument)
    {
        ModelParams parameters = new(_modelPath)
        {
            ContextSize = 4096,
            GpuLayerCount = 99,
            BatchSize = 512,
            UseMemorymap = true
        };

        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        StatelessExecutor executor = new(model, parameters);

        InferenceParams inferenceParams = new()
        {
            MaxTokens = 4096,
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.6f
            },
            AntiPrompts = ["<|eot_id|>", "<|end_of_text|>"]
        };

        List<JsonElement> records = new();
        int batchSize = 10;
        int numberOfBatches = (int)Math.Ceiling((double)recordCount / batchSize);

        for (int batchNumber = 0; batchNumber < numberOfBatches; batchNumber++)
        {
            int recordsInThisBatch = Math.Min(batchSize, recordCount - (batchNumber * batchSize));
            int startingId = (batchNumber * batchSize) + 1;

            string prompt = BuildSampleDataPrompt(startingId, recordsInThisBatch, sampleDocument);

            // Calls the model and waits for it to be done processing
            StringBuilder fullResponse = new();
            await foreach (var text in executor.InferAsync(prompt, inferenceParams))
            {
                fullResponse.Append(text);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(fullResponse.ToString());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            string? jsonData = ExtractJson(fullResponse.ToString());

            if (jsonData is null)
            {
                // Ignore this batch and go on to the next one
                Console.WriteLine($"Batch number {batchNumber + 1} ignored - bad data.");
                continue;
            }

            try
            {
                JsonDocument jsonDoc = JsonDocument.Parse(jsonData);
                if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var record in jsonDoc.RootElement.EnumerateArray())
                    {
                        records.Add(record.Clone());
                    }
                }
            }
            catch
            {
                Console.WriteLine("No valid records found in JSON");
            }
        }

        string jsonString = JsonSerializer.Serialize(records);
        JsonDocument output = JsonDocument.Parse(jsonString);

        return output;
    }

    private string BuildSampleDataPrompt(int startingId, int recordCount, JsonDocument sampleDocument)
    {
        string sampleAsString = sampleDocument.RootElement.ToString();

        string prompt = $@"<|start_header_id|>system<|end_header_id|>

                        You are a precise JSON data generator. You must follow instructions exactly.

                        <|eot_id|><|start_header_id|>user<|end_header_id|>

                        Generate exactly {recordCount} records of realistic sample data following this structure:

                        {sampleAsString}

                        CRITICAL REQUIREMENTS:
                        1. Output ONLY a valid JSON array starting with [ and ending with ]
                        2. Generate exactly {recordCount} complete records
                        3. Each record MUST match the exact property names and types shown above
                        4. ALL fields must contain realistic, non-empty values:
                            - First Names: Use realistic first names and last names (e.g., ""John"", ""Sue"")
                            - Addresses: Use realistic street addresses, cities, provinces, and postal codes
                            - Phone numbers: Use valid formats (e.g., ""555-112-2354"")
                            - Email addresses: Use realistic emails (e.g., ""john.smith@sample.com"")
                            - Dates: Use valid date formats (e.g., ""2025-11-24"")
                            - Numbers: Use realistic non-zero numbers unless a zero is called for
                        5. Generate varied information, including using rare edge cases. Avoid repeating patterns within the response
                        6. DO NOT include any text, explanation, or markdown before or after the JSON array
                        7. Ensure valid JSON syntax with proper commas between objects
                        8. Make each record unique with different values
                        9. If there is a property name that requires a numeric ID, start with 
                           the ID of {startingId} and increment by 1 each time
                        10. If an ID or similar unique value is required, ensure that it is unique. Do not use the same value
                            for multiple records
                        11. If a GUID is required or if a random value is required, utilize the value of {startingId} plus the number of 
                            records already generated as the seed

                        Your response must start with [ and end with ]

                        <|eot_id|><|start_header_id|>assistant<|end_header_id|>
                        ";
        return prompt;
    }

    private string? ExtractJson(string response)
    {
        int startIndex = response.IndexOf('[');

        if (startIndex < 0)
        {
            // No array start found
            Console.WriteLine("No start of the array found in the response");
            return null;
        }

        int bracketCount = 0;
        int endIndex = -1;

        for (int i = startIndex; i < response.Length; i++)
        {
            if (response[i] == '[')
            {
                bracketCount++;
            }
            else if (response[i] == ']')
            {
                bracketCount--;
                if (bracketCount == 0)
                {
                    endIndex = i;
                    break;
                }
            }
        }

        if (endIndex > startIndex)
        {
            return response.Substring(startIndex, endIndex - startIndex + 1).Trim();
        }

        Console.WriteLine("The response was not a properly formatted JSON array.");
        return null;
    }
}
