using eCommerceAPI.Models;
using MongoDB.Driver;

namespace eCommerceAPI.Data
{
    public class ApiContext
    {
        private readonly IConfiguration _configuration;
        public IMongoCollection<Items> Items { get; set; }
        public IMongoCollection<Users> Users { get; set; }
        public IMongoCollection<CompletedOrders> CompletedOrders { get; set; }
        public IMongoCollection<CompletedOrderItems> CompletedOrderItems { get; set; }
        public IMongoCollection<ShoppingCarts> ShoppingCarts { get; set; }
        public IMongoCollection<ShoppingCartItems> ShoppingCartItems { get; set; }
        public IMongoCollection<Categories> Categories { get; set; }
        public ApiContext(IConfiguration configuration)
        {
            _configuration = configuration;
            string? connectionString = _configuration.GetConnectionString("MongoDB");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("E-Commerce");

            Items = database.GetCollection<Items>("Items");
            Users = database.GetCollection<Users>("Users");
            CompletedOrders = database.GetCollection<CompletedOrders>("CompletedOrders");
            CompletedOrderItems = database.GetCollection<CompletedOrderItems>("CompletedOrderItems");
            ShoppingCarts = database.GetCollection<ShoppingCarts>("ShoppingCarts");
            ShoppingCartItems = database.GetCollection<ShoppingCartItems>("ShoppingCartItems");
            Categories = database.GetCollection<Categories>("Categories");
        }
    }
}