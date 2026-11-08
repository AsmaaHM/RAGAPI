# AIPracticeAPI

A modern .NET 9 ASP.NET Core REST API demonstrating **Microsoft Extensions AI (MEAI)** with a Retrieval-Augmented Generation (RAG) pipeline using OpenAI and Qdrant vector database.

## 🎯 Project Overview

This project showcases practical implementation of:
- **Semantic Search & Vector Embeddings** using OpenAI's `text-embedding-3-small` model
- **Retrieval-Augmented Generation (RAG)** for context-aware AI responses
- **Function Invocation** with the GPT-4o chat model
- **Document Ingestion Pipeline** with automatic chunking and embedding generation
- **Multi-turn Conversations** with persistent chat history management
- **Modern .NET Architecture** with dependency injection and clean separation of concerns

## 🚀 Getting Started

### Prerequisites
- **.NET 9 SDK** or later
- **OpenAI API Key** (for chat and embeddings)
- **Qdrant Vector Database** (cloud or local instance)
- **Visual Studio 2022** or VS Code

### Installation

1. **Clone the repository**
git clone <repository-url> cd AIPracticeAPI

2. **Set up User Secrets**

Initialize user secrets for your project:
dotnet user-secrets init

Add your credentials:
dotnet user-secrets set "OPENAI_API_KEY" "<your-openai-api-key>" 
dotnet user-secrets set "CLUSTER_ENDPOINT" "<your-qdrant-cluster-url>" 
dotnet user-secrets set "QDRANT_API_KEY" "<your-qdrant-api-key>"

3. **Install dependencies**
dotnet restore

4. **Run the application**
dotnet run

The API will be available at `https://localhost:5001` with Swagger UI at `/swagger`.

## 🔧 Key Components

### 1. **DocumentIngestionService**
Handles document processing and vector embedding generation:
- Reads Markdown files from the `Files/` directory
- Chunks documents using GPT-4o tokenizer (1000 tokens per chunk)
- Generates embeddings using OpenAI's text-embedding-3-small model
- Stores embeddings in Qdrant vector database with HNSW indexing

### 2. **ChatService**
Implements RAG pattern for intelligent conversations:
- Generates user query embeddings
- Performs semantic similarity search in Qdrant
- Augments prompts with relevant document context
- Supports function invocation for extended capabilities

### 3. **ChatHistoryService**
Manages multi-turn conversation state:
- Maintains separate conversation histories per session
- Tracks user and assistant messages
- Enables conversation continuation across multiple requests

### 4. **DocumentChunk Model**
Represents indexed document segments:
- `ID`: Unique identifier (SHA256-based from document name and chunk index)
- `Content`: Text chunk
- `Embedding`: Vector representation (1536 dimensions, cosine similarity)
- `DocumentName`: Source document identifier

## 🔑 Core Technologies

| Technology | Purpose |
|-----------|---------|
| **Microsoft.Extensions.AI** | Unified AI client abstraction |
| **OpenAI** | GPT-4o chat model & text embeddings |
| **Qdrant** | Vector database for semantic search |
| **CommunityToolkit.VectorData** | Vector data abstractions |
| **Microsoft.Extensions.DataIngestion** | Document processing pipeline |
| **ML.Tokenizers** | Token counting for chunking |
| **Swagger/OpenAPI** | API documentation & testing |

## 📡 API Endpoints

### Start a Conversation
POST /api/chat/start


# Response
{ "conversationId": "550e8400-e29b-41d4-a716-446655440000" }

### Send a Message

POST /api/chat/send Content-Type: application/json
{ "conversationId": "550e8400-e29b-41d4-a716-446655440000", "message": "What is the leave policy?" }


# Response
{ "response": "Based on our leave policy documents, ...", "conversationId": "550e8400-e29b-41d4-a716-446655440000" }

## 🧠 How RAG Works

1. **User sends query** → Converted to embedding via OpenAI
2. **Semantic search** → Query embedding compared against document embeddings in Qdrant
3. **Context retrieval** → Most relevant document chunks returned (top-k results)
4. **Prompt augmentation** → Retrieved context added to system prompt
5. **LLM response** → GPT-4o generates answer using augmented context

This ensures answers are grounded in your indexed documents while leveraging the reasoning power of GPT-4o.

## 🛠️ Development

### Adding New Documents
Place Markdown files in the `Files/` directory. On next application startup, they'll be automatically ingested and embedded.

### Modifying Chunking Strategy
Edit `DocumentIngestionService.cs`
IngestionChunkerOptions options = new(tokenizer) { MaxTokensPerChunk = 1000,  // Adjust chunk size OverlapTokens = 0          // Add overlap for context continuity };


### Extending Services
All services follow the interface-based pattern for easy testing and extension:
- Implement `IDocumentIngestionService` for custom ingestion logic
- Implement `IChatService` for alternative RAG strategies
- Implement `IChatHistoryService` for persistent storage

## 📚 Learning Outcomes

This project demonstrates practical knowledge of:
- ✅ Vector embeddings and semantic search
- ✅ Retrieval-Augmented Generation (RAG)
- ✅ LLM function invocation
- ✅ Prompt engineering with context
- ✅ Async/await patterns in .NET
- ✅ Dependency injection and SOLID principles
- ✅ RESTful API design
- ✅ Concurrent data structure usage

## 🔐 Security Notes

- **API Keys**: Never commit secrets. Use User Secrets in development, environment variables in production.
- **Qdrant Connection**: Uses HTTPS with API key authentication.
- **Input Validation**: Add validation for production use.

## 📦 NuGet Dependencies

- `Microsoft.Extensions.AI` - AI abstractions
- `Microsoft.Extensions.AI.OpenAI` - OpenAI provider
- `Microsoft.Extensions.DataIngestion` - Document ingestion
- `CommunityToolkit.VectorData.Qdrant` - Qdrant integration
- `OpenAI` - OpenAI client library
- `Microsoft.ML.Tokenizers` - Token counting
- `Swashbuckle.AspNetCore` - Swagger documentation

## 🤝 Contributing

This is a learning project. Suggestions for improvement are welcome!

## 📄 License

MIT License - Feel free to use this as a reference for your own MEAI projects.

## 🔗 Resources

- [Microsoft.Extensions.AI Docs](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai)
- [Qdrant Vector Database](https://qdrant.tech/)
- [OpenAI API Documentation](https://platform.openai.com/docs)
- [RAG Pattern Overview](https://learn.microsoft.com/en-us/ai/playbook/technology-guidance/generative-ai/rag)


