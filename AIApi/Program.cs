using EmbeddedAILibrary;
using Scalar.AspNetCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

string modelPath = builder.Configuration.GetValue<string>("ModelPath") ?? string.Empty;
builder.Services.AddSingleton(new AIGenerator(modelPath));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/generate-sample-data", async (
    int recordCount,
    string sampleJson,
    AIGenerator generator) =>
{
    using var jsonDocument = JsonDocument.Parse(sampleJson);
    JsonDocument result = await generator.GetSampleDataAsync(recordCount, jsonDocument);

    return Results.Ok(result);
});

app.Run();
