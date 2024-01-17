using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerceAPI.Models
{
    public class CompletedOrder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? ReferenceItemId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public DateOnly DateofPurchase { get; set; } // YYYY-MM-DD
    }
}