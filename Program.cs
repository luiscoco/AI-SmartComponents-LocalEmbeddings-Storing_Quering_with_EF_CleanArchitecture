using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartComponents;
using SmartComponents.LocalEmbeddings;
using SmartComponents_Storing_Quering_with_EF_CleanArchitecture;

namespace SmartComponentsDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the database and ensure it's created
            using (var dbContext = new MyDbContext())
            {
                await dbContext.Database.EnsureCreatedAsync();
            }

            // Initialize the embedder
            using var embedder = new LocalEmbedder();

            // Seed the database with sample documents
            await EmbedderHelper.SeedDatabaseAsync(embedder);

            // Simulate a search query
            Console.WriteLine("Enter search text:");
            string searchText = Console.ReadLine() ?? string.Empty;

            // Perform the similarity search
            await EmbedderHelper.PerformSimilaritySearchAsync(embedder, searchText);
        }
    }
}
