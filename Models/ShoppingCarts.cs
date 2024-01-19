using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerceAPI.Models
{
    public class ShoppingCarts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserReference { get; set; }

        public List<CartItem>? ItemsList { get; set; }
    }
    public class CartItem
    {
        public ObjectId ItemId { get; set; }
        public int Quantity { get; set; }
    }
}