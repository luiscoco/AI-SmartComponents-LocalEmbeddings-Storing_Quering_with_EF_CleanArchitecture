using Microsoft.EntityFrameworkCore;
using SmartComponents.LocalEmbeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComponents_Storing_Quering_with_EF_CleanArchitecture
{
    // Helper class containing methods for embeddings
    public static class EmbedderHelper
    {
        // Method to seed the database with sample documents
        public static async Task SeedDatabaseAsync(LocalEmbedder embedder)
        {
            using var dbContext = new MyDbContext();

            // Check if the database is already seeded
            if (await dbContext.Documents.AnyAsync())
            {
                return;
            }

            // Sample documents
            var documents = new List<Document>
            {
                new Document
                {
                    OwnerId = 1,
                    Title = "Introduction to C#",
                    Body = "C# is a modern, object-oriented programming language developed by Microsoft.",
                    EmbeddingI8Buffer = ComputeDocumentEmbedding(embedder, "Introduction to C#", "C# is a modern, object-oriented programming language developed by Microsoft.")
                },
                new Document
                {
                    OwnerId = 1,
                    Title = "Entity Framework Core Guide",
                    Body = "EF Core is a lightweight, extensible, open-source, and cross-platform version of the popular Entity Framework data access technology.",
                    EmbeddingI8Buffer = ComputeDocumentEmbedding(embedder, "Entity Framework Core Guide", "EF Core is a lightweight, extensible, open-source, and cross-platform version of the popular Entity Framework data access technology.")
                },
                new Document
                {
                    OwnerId = 2,
                    Title = "Getting Started with ASP.NET Core",
                    Body = "ASP.NET Core is a cross-platform, high-performance, open-source framework for building modern, cloud-enabled, Internet-connected apps.",
                    EmbeddingI8Buffer = ComputeDocumentEmbedding(embedder, "Getting Started with ASP.NET Core", "ASP.NET Core is a cross-platform, high-performance, open-source framework for building modern, cloud-enabled, Internet-connected apps.")
                },
            };

            // Add documents to the database
            dbContext.Documents.AddRange(documents);
            await dbContext.SaveChangesAsync();
        }

        // Helper method to compute the embedding of a document
        public static byte[] ComputeDocumentEmbedding(LocalEmbedder embedder, string title, string body)
        {
            // Combine the document properties into a single string
            string combinedText = $"{title} {body}";

            // Compute the embedding of the combined text
            var embedding = embedder.Embed<EmbeddingI8>(combinedText);

            // Return the embedding buffer as byte[]
            return embedding.Buffer.ToArray();
        }

        // Method to perform similarity search
        public static async Task PerformSimilaritySearchAsync(LocalEmbedder embedder, string searchText)
        {
            using var dbContext = new MyDbContext();

            // Load embeddings and IDs for all documents
            var documents = await dbContext.Documents
                .Select(d => new { d.DocumentId, d.EmbeddingI8Buffer })
                .ToListAsync();

            // Embed the search text
            var searchEmbedding = embedder.Embed<EmbeddingI8>(searchText);

            // Prepare embeddings for similarity search
            var documentEmbeddings = documents.Select(d => (d.DocumentId, new EmbeddingI8(d.EmbeddingI8Buffer)));

            // Find closest documents
            int[] matchingDocIds = LocalEmbedder.FindClosest(
                searchEmbedding,
                documentEmbeddings,
                maxResults: 5);

            // Load full documents
            var matchingDocs = await dbContext.Documents
                .Where(d => matchingDocIds.Contains(d.DocumentId))
                .ToDictionaryAsync(d => d.DocumentId);

            // Display results
            Console.WriteLine("\nMatching Documents:");
            foreach (var docId in matchingDocIds)
            {
                if (matchingDocs.TryGetValue(docId, out var doc))
                {
                    Console.WriteLine($"- {doc.Title}");
                }
            }
        }

        // Method to update a document and recompute its embedding
        public static async Task UpdateDocumentAsync(int documentId, string newTitle, string newBody, LocalEmbedder embedder)
        {
            using var dbContext = new MyDbContext();

            var document = await dbContext.Documents.FindAsync(documentId);
            if (document != null)
            {
                document.Title = newTitle;
                document.Body = newBody;
                document.EmbeddingI8Buffer = ComputeDocumentEmbedding(embedder, newTitle, newBody);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
