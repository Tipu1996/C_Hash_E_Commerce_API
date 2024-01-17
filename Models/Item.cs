using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerceAPI.Models
{
    public class Item
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Company { get; set; }
        public string? CategoryId { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public int Inventory { get; set; }
        public string? Imageurl { get; set; }
        public string? Specifications { get; set; }
    }
}