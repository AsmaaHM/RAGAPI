using RAGAPI.Models;
using RAGAPI.Services.Interfaces;
using CommunityToolkit.VectorData.Qdrant;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DataIngestion;
using Microsoft.ML.Tokenizers;
using Qdrant.Client;
using System.Security.Cryptography;
using System.Text;


namespace RAGAPI.Services.Implementations
{
    public class DocumentIngestionService : IDocumentIngestionService
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private QdrantCollection<Guid, DocumentChunk> _collection;
        private readonly QdrantClient _qdrantClient;
        private List<DocumentChunk> _documentChunks = new List<DocumentChunk>(); 

        public DocumentIngestionService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, QdrantClient qdrantClient)
        {
            _embeddingGenerator = embeddingGenerator;
            _qdrantClient = qdrantClient;
        }
        public async Task IngestDocumentsAsync()
        {
            Tokenizer tokenizer = TiktokenTokenizer.CreateForModel("gpt-4o");
            IngestionChunkerOptions options = new(tokenizer)
            {
                MaxTokensPerChunk = 1000,
                OverlapTokens = 0
            };
            IngestionChunker<string> chunker = new HeaderChunker(options);
      

            var documentsPath = Path.Combine(
                            AppContext.BaseDirectory,
                            "Files");

            var files = Directory.GetFiles(documentsPath, "*.md");

            IngestionDocumentReader reader = new MarkdownReader();

            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                var ingestionDocument = await reader.ReadAsync(fileInfo);
                int index = 0; 
                await foreach (var chunk in chunker.ProcessAsync(ingestionDocument))
                {
     
                    var embedding = await _embeddingGenerator.GenerateAsync(chunk.Content);
                    _documentChunks.Add(new DocumentChunk
                    {
                        ID = CreateChunkId(fileInfo.Name, index),
                        Content = chunk.Content,
                        Embedding = embedding.Vector,
                        DocumentName = fileInfo.Name
                    });
                    index++; 
                }


            }
            _collection = new QdrantCollection<Guid, DocumentChunk>(
                             _qdrantClient,
                             "documentChunks",
                             ownsClient: true);

            await _collection.EnsureCollectionExistsAsync();
            await _collection.UpsertAsync(_documentChunks);

        }

        private static Guid CreateChunkId(string documentName, int chunkIndex)
        {
            var input = $"{documentName}:{chunkIndex}";

            var hash = SHA256.HashData(
                Encoding.UTF8.GetBytes(input));

            return new Guid(hash[..16]);
        }
    }
}
