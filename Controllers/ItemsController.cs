using eCommerceAPI.Data;
using eCommerceAPI.Models;
using eCommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace eCommerceAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly IMongoCollection<Users> _usersCollection;
        private readonly IMongoCollection<Items> _itemsCollection;
        private readonly IMongoCollection<ShoppingCarts> _shoppingCartsCollection;
        private readonly IMongoCollection<ShoppingCartItems> _shoppingCartItemsCollection;
        private readonly IMongoCollection<CompletedOrders> _completedOrdersCollection;
        private readonly IMongoCollection<CompletedOrderItems> _completedOrderItemsCollection;
        private readonly JwtService _jwtService;
        public ItemsController(ApiContext apiContext, JwtService jwtService)
        {
            _usersCollection = apiContext.Users;
            _itemsCollection = apiContext.Items;
            _shoppingCartsCollection = apiContext.ShoppingCarts;
            _shoppingCartItemsCollection = apiContext.ShoppingCartItems;
            _completedOrdersCollection = apiContext.CompletedOrders;
            _completedOrderItemsCollection = apiContext.CompletedOrderItems;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult GetAllItems()
        {
            var allItems = _itemsCollection.Find(_ => true).ToList();
            if (allItems == null || allItems.Count == 0) return NotFound();
            return Ok(allItems);
        }

        [HttpPost]
        public IActionResult AddNewItem(Items item)
        {
            _itemsCollection.InsertOne(item);
            return Ok(item);
        }

        [HttpGet("Id")]
        public IActionResult GetItemById(string id)
        {
            var search = _itemsCollection.Find(x => x.Id == id).FirstOrDefault();
            if (search == null) return NotFound();
            return Ok(search);
        }
    }
}