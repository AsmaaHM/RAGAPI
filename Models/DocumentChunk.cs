using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace RAGAPI.Models
{
    public record DocumentChunk
    {
        public DocumentChunk()
        {
                
        }
        public DocumentChunk(Guid Id, string text, ReadOnlyMemory<float> embedding, string documentName)
        {
            this.ID = Id; 
            this.Content = text; 
            this.Embedding = embedding;
            this.DocumentName = documentName;
        }
        [VectorStoreKey]
        public Guid ID { get; set; }
        [VectorStoreData]
        public string Content { get; init; }

        [VectorStoreVector(dimensions: 1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? Embedding { get; set; }

        [VectorStoreData]
        public string DocumentName { get; set; }
    }
}