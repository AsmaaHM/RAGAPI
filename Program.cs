using RAGAPI.Models;
using RAGAPI.Services.Implementations;
using RAGAPI.Services.Interfaces;
using CommunityToolkit.VectorData.InMemory;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using OpenAI;
using Qdrant.Client;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();


var config = new ConfigurationBuilder()
               .AddUserSecrets<Program>()
               .Build();

var key = config["OPENAI_API_KEY"];

IChatClient chatClient =
    new ChatClientBuilder(new OpenAIClient(key).GetChatClient("gpt-4o").AsIChatClient())
    .UseFunctionInvocation()
    .Build();

IEmbeddingGenerator<string, Embedding<float>> generator = 
    new OpenAIClient(key)
    .GetEmbeddingClient("text-embedding-3-small")
    .AsIEmbeddingGenerator();

//var vectorStore = new InMemoryVectorStore();

string clusterEndpoint = config.GetValue<string>("CLUSTER_ENDPOINT");
string qdrantApiKey = config.GetValue<string>("QDRANT_API_KEY");

var qdrantClient = new QdrantClient(
    host: clusterEndpoint,
    port: 6334,
    https: true,
    apiKey: qdrantApiKey);

builder.Services.AddSingleton(qdrantClient);
builder.Services.AddQdrantVectorStore();
builder.Services.AddSingleton(chatClient);
builder.Services.AddSingleton(generator); 
builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<IChatHistoryService, ChatHistoryService>();
builder.Services.AddSingleton<IDocumentIngestionService, DocumentIngestionService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var documentIngestionService = scope.ServiceProvider.GetRequiredService<IDocumentIngestionService>();
    await documentIngestionService.IngestDocumentsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();