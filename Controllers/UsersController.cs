using eCommerceAPI.Data;
using eCommerceAPI.Models;
using eCommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace eCommerceAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<Users> _usersCollection;
        private readonly IMongoCollection<Items> _itemsCollection;
        private readonly IMongoCollection<ShoppingCarts> _shoppingCartsCollection;
        private readonly IMongoCollection<CompletedOrders> _completedOrdersCollection;
        private readonly JwtService _jwtService;
        public UsersController(ApiContext apiContext, JwtService jwtService)
        {
            _usersCollection = apiContext.Users;
            _itemsCollection = apiContext.Items;
            _shoppingCartsCollection = apiContext.ShoppingCarts;
            _completedOrdersCollection = apiContext.CompletedOrders;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult RegisterUser(Users newUser)
        {
            _usersCollection.InsertOne(newUser);
            var newShoppingCart = new ShoppingCarts { ItemsList = new List<CartItem>(), UserReference = newUser.Id };
            _shoppingCartsCollection.InsertOne(newShoppingCart);

            var newCompletedOrder = new CompletedOrders { ItemsList = new List<BoughtItem>(), UserReference = newUser.Id };
            _completedOrdersCollection.InsertOne(newCompletedOrder);

            newUser.ShoppingCartReference = newShoppingCart.Id;
            newUser.CompletedOrdersReference = newCompletedOrder.Id;
            newUser.IsAdmin = false;


            _usersCollection.ReplaceOne(x => x.Id == newUser.Id, newUser);
            var jwtToken = _jwtService.GenerateJwtToken(newUser);
            return Ok(new { newUser, JWT = jwtToken, });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var search = _usersCollection.Find(x => x.Email == loginModel.Email && x.Password == loginModel.Password).FirstOrDefault();
            if (search == null) return NotFound("unable to retrieve user Information");
            var jwtToken = _jwtService.GenerateJwtToken(search);
            return Ok(jwtToken);
        }

        [HttpPut("update"), Authorize]
        public IActionResult UpdateUser(Users updatedUser)
        {
            var userId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest("The user's ID cannot be retrieved from the JWT");
            var filter = Builders<Users>.Filter.Eq(x => x.Id, userId);
            var updateDefinition = Builders<Users>.Update.Combine();
            if (updatedUser.Name != null)
            {
                updateDefinition = updateDefinition.Set(search => search.Name, updatedUser.Name);
            }
            if (updatedUser.Email != null)
            {
                updateDefinition = updateDefinition.Set(search => search.Email, updatedUser.Email);
            }
            if (updatedUser.Password != null)
            {
                updateDefinition = updateDefinition.Set(search => search.Password, updatedUser.Password);
            }
            if (updatedUser.UserAddress != null)
            {
                if (updatedUser.UserAddress.Street != null)
                {
                    updateDefinition = updateDefinition.Set(search => search.UserAddress.Street, updatedUser.UserAddress.Street);
                }
                if (updatedUser.UserAddress.ZipCode != null)
                {
                    updateDefinition = updateDefinition.Set(search => search.UserAddress.ZipCode, updatedUser.UserAddress.ZipCode);
                }
                if (updatedUser.UserAddress.City != null)
                {
                    updateDefinition = updateDefinition.Set(search => search.UserAddress.City, updatedUser.UserAddress.City);
                }
                if (updatedUser.UserAddress.Country != null)
                {
                    updateDefinition = updateDefinition.Set(search => search.UserAddress.Country, updatedUser.UserAddress.Country);
                }
                if (updatedUser.UserAddress.PhoneNumber != null)
                {
                    updateDefinition = updateDefinition.Set(search => search.UserAddress.PhoneNumber, updatedUser.UserAddress.PhoneNumber);
                }
            }
            if (updateDefinition != Builders<Users>.Update.Combine())
            {
                _usersCollection.UpdateOne(filter, updateDefinition);
            }
            return StatusCode(304, "No modifications provided");
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var search = _usersCollection.Find(_ => true).ToList();
            if (search == null || search.Count == 0) return NotFound("unable to retrieve user Information");
            else return Ok(search);
        }

        [HttpGet("profile"), Authorize]
        public IActionResult GetMyProfile()
        {
            var userId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest("unable to retrieve user Information");
            var search = _usersCollection.Find(x => x.Id == userId).FirstOrDefault();
            if (search == null) return NotFound("unable to retrieve user Information");
            else return Ok(search);
        }

        [HttpGet("searchbyname")]
        public IActionResult GetUserByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Name parameter is required");
            var search = _usersCollection.Find(x => x.Name.Contains(name)).ToList();
            if (search == null) return NotFound("unable to retrieve user Information");
            else return Ok(search);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var searchedUser = _usersCollection.Find(x => x.Id == id).FirstOrDefault();
            if (searchedUser == null) return NotFound("unable to retrieve user Information");
            else return Ok(searchedUser);
        }

        [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteUserById(string id)
        {
            var searchedUser = _usersCollection.Find(x => x.Id == id).FirstOrDefault();
            if (searchedUser == null) return NotFound("unable to retrieve user Information");
            var shoppingCart = _shoppingCartsCollection.Find(x => x.Id == searchedUser.ShoppingCartReference).FirstOrDefault();
            if (shoppingCart.ItemsList != null)
            {
                foreach (var item in shoppingCart.ItemsList)
                {
                    var itemInInventory = _itemsCollection.Find(x => x.Id == item.ItemId).FirstOrDefault();
                    itemInInventory.Inventory += item.Quantity;
                    _itemsCollection.ReplaceOne(x => x.Id == item.ItemId, itemInInventory);
                }
            }
            var completedOrder = _completedOrdersCollection.Find(x => x.Id == searchedUser.CompletedOrdersReference).FirstOrDefault();
            _completedOrdersCollection.DeleteOne(x => x.Id == searchedUser.CompletedOrdersReference);
            _shoppingCartsCollection.DeleteOne(x => x.Id == searchedUser.ShoppingCartReference);
            _usersCollection.DeleteOne(x => x.Id == id);
            return NoContent();
        }

        [HttpGet("shopping-cart"), Authorize]
        public IActionResult GetShoppingCart()
        {
            var userId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest("unable to retrieve user Information");
            var search = _usersCollection.Find(x => x.Id == userId).FirstOrDefault();
            if (search == null) return BadRequest("unable to retrieve user Information");
            var searchCart = _shoppingCartsCollection.Find(x => x.Id == search.ShoppingCartReference).FirstOrDefault();
            return Ok(searchCart);
        }

        [HttpGet("completed-orders"), Authorize]
        public IActionResult GetCompletedOrders()
        {
            var userId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest("unable to retrieve user Information");
            var search = _usersCollection.Find(x => x.Id == userId).FirstOrDefault();
            if (search == null) return BadRequest("unable to retrieve user Information");
            var completedOrders = _completedOrdersCollection.Find(search.CompletedOrdersReference);
            return Ok(completedOrders);
        }

        [HttpPost("add-to-cart/{id}")]
        public IActionResult AddToCart(string id)
        {
            // search for the user using the jwt and find in mongodb
            var userId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest("unable to retrieve user Information");
            var search = _usersCollection.Find(x => x.Id == userId).FirstOrDefault();
            if (search == null) return BadRequest("unable to retrieve user Information");
            // find shoppingcart in mongodb
            var shoppingCart = _shoppingCartsCollection.Find(x => x.Id == search.ShoppingCartReference).FirstOrDefault();
            var existingItem = shoppingCart.ItemsList.FirstOrDefault(x => x.ItemId == id);
            // find item in mongodb
            var itemInDb = _itemsCollection.Find(x => x.Id == id).FirstOrDefault();
            itemInDb.Inventory -= 1;
            if (existingItem == null)
            {
                var itemToAdd = new CartItem { ItemId = id, Quantity = 1 };
                shoppingCart.ItemsList.Add(itemToAdd);
            }
            else existingItem.Quantity += 1;
            _itemsCollection.ReplaceOne(x => x.Id == id, itemInDb);
            _shoppingCartsCollection.ReplaceOne(x => x.Id == shoppingCart.Id, shoppingCart);
            return Ok();
        }
        [HttpPost("remove-from-cart/{id}")]
        public IActionResult RemoveFromCart(string id)
        {
            // search for the user using the jwt and find in mongodb
            var userId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest("unable to retrieve user Information");
            var search = _usersCollection.Find(x => x.Id == userId).FirstOrDefault();
            if (search == null) return BadRequest("unable to retrieve user Information");
            // find shoppingcart in mongodb
            var shoppingCart = _shoppingCartsCollection.Find(x => x.Id == search.ShoppingCartReference).FirstOrDefault();
            var existingItem = shoppingCart.ItemsList.FirstOrDefault(x => x.ItemId == id);
            // find item in mongodb
            if (existingItem == null)
            {
                return BadRequest("The Item does not exist in your cart");
            }
            else existingItem.Quantity -= 1;
            var itemInDb = _itemsCollection.Find(x => x.Id == id).FirstOrDefault();
            itemInDb.Inventory += 1;
            _itemsCollection.ReplaceOne(x => x.Id == id, itemInDb);
            _shoppingCartsCollection.ReplaceOne(x => x.Id == shoppingCart.Id, shoppingCart);
            return Ok();
        }
    }
}