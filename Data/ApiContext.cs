using eCommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace eCommerceAPI.Data
{
    public class ApiContext
    {
        private readonly IConfiguration _configuration;
        public IMongoCollection<Item> Items { get; set; }
        public IMongoCollection<User> Users { get; set; }
        public IMongoCollection<CompletedOrder> CompletedOrders { get; set; }
        public IMongoCollection<ShoppingCartItem> ShoppingCartItems { get; set; }
        public IMongoCollection<Category> Categories { get; set; }
        public ApiContext(IConfiguration configuration)
        {
            _configuration = configuration;
            string? connectionString = _configuration.GetConnectionString("MongoDB");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("E-Commerce");

            Items = database.GetCollection<Item>("Items");
            Users = database.GetCollection<User>("Users");
            CompletedOrders = database.GetCollection<CompletedOrder>("CompletedOrders");
            ShoppingCartItems = database.GetCollection<ShoppingCartItem>("ShoppingItems");
            Categories = database.GetCollection<Category>("Categories");
        }
    }
}