using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerceAPI.Models
{
    public class CompletedOrders
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserReference { get; set; }

        public List<BoughtItem>? ItemsList { get; set; }
    }
    public class BoughtItem
    {
        public ObjectId ItemId { get; set; }
        public int Quantity { get; set; }
    }
}