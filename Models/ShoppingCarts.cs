using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerceAPI.Models
{
    public class ShoppingCarts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserReference { get; set; } = string.Empty;

        public List<CartItem> ItemsList { get; set; } = [];
    }
    public class CartItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ItemId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}