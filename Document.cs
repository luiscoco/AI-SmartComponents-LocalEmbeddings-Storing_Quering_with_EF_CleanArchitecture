using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComponents_Storing_Quering_with_EF_CleanArchitecture
{
     public class Document
    {
        [Key]
        public int DocumentId { get; set; }
        public int OwnerId { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }

        // Embedding of all properties combined
        public required byte[] EmbeddingI8Buffer { get; set; }
    }
}
