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
        private readonly IMongoCollection<CompletedOrders> _completedOrdersCollection;
        private readonly JwtService _jwtService;
        public ItemsController(ApiContext apiContext, JwtService jwtService)
        {
            _usersCollection = apiContext.Users;
            _itemsCollection = apiContext.Items;
            _shoppingCartsCollection = apiContext.ShoppingCarts;
            _completedOrdersCollection = apiContext.CompletedOrders;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult GetAllItems()
        {
            var allItems = _itemsCollection.Find(_ => true).ToList();
            if (allItems == null || allItems.Count == 0) return NotFound("Did not find any items in the Database");
            return Ok(allItems);
        }

        [HttpPost]
        public IActionResult AddNewItem(Items item)
        {
            _itemsCollection.InsertOne(item);
            return Ok(item);
        }

        [HttpGet("{Id}")]
        public IActionResult GetItemById(string id)
        {
            var search = _itemsCollection.Find(x => x.Id == id).FirstOrDefault();
            if (search == null) return NotFound();
            return Ok(search);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItemDetails(string id, Items updatedItem)
        {
            var filter = Builders<Items>.Filter.Eq(x => x.Id, id);
            var updateDefinition = Builders<Items>.Update.Combine();
            if (!string.IsNullOrEmpty(updatedItem.Name))
            {
                updateDefinition = updateDefinition.Set(x => x.Name, updatedItem.Name);
            }
            if (!string.IsNullOrEmpty(updatedItem.Description))
            {
                updateDefinition = updateDefinition.Set(x => x.Description, updatedItem.Description);
            }
            if (!string.IsNullOrEmpty(updatedItem.Company))
            {
                updateDefinition = updateDefinition.Set(x => x.Company, updatedItem.Company);
            }
            if (!string.IsNullOrEmpty(updatedItem.Category))
            {
                updateDefinition = updateDefinition.Set(x => x.Category, updatedItem.Category);
            }
            if (updatedItem.Price > 0)
            {
                updateDefinition = updateDefinition.Set(x => x.Price, updatedItem.Price);
            }
            if (updatedItem.Discount >= 0)
            {
                updateDefinition = updateDefinition.Set(x => x.Discount, updatedItem.Discount);
            }
            if (updatedItem.Inventory >= 0)
            {
                updateDefinition = updateDefinition.Set(x => x.Inventory, updatedItem.Inventory);
            }
            if (!string.IsNullOrEmpty(updatedItem.Imageurl))
            {
                updateDefinition = updateDefinition.Set(x => x.Imageurl, updatedItem.Imageurl);
            }
            if (!string.IsNullOrEmpty(updatedItem.Specifications))
            {
                updateDefinition = updateDefinition.Set(x => x.Specifications, updatedItem.Specifications);
            }
            // Apply the update only if there are modifications
            if (updateDefinition != Builders<Items>.Update.Combine())
            {
                _itemsCollection.UpdateOne(filter, updateDefinition);
                return NoContent();
            }
            // No modifications, return NotModified
            return StatusCode(304, "No modifications provided");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItemById(string id)
        {
            var search = _itemsCollection.Find(x => x.Id == id).FirstOrDefault();
            if (search == null) return NotFound("Unable to delete item from database because it does not exist");
            _itemsCollection.FindOneAndDelete(x => x.Id == id);
            return NoContent();
        }
    }
}