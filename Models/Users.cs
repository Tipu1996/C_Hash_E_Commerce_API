using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerceAPI.Models
{
    public class Address
    {
        public string Street { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ShoppingCartReference { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string CompletedOrdersReference { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public Address UserAddress { get; set; } = new Address();
    }
}