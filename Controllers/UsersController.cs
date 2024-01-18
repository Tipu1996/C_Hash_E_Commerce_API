using eCommerceAPI.Data;
using eCommerceAPI.Models;
using eCommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using MongoDB.Driver;

namespace eCommerceAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<Users> _usersCollection;
        private readonly IMongoCollection<ShoppingCarts> _shoppingCartsCollection;
        private readonly IMongoCollection<ShoppingCartItems> _shoppingCartItemsCollection;
        private readonly IMongoCollection<CompletedOrders> _completedOrdersCollection;
        private readonly IMongoCollection<CompletedOrderItems> _completedOrderItemsCollection;
        private readonly JwtService _jwtService;
        public UsersController(ApiContext apiContext, JwtService jwtService)
        {
            _usersCollection = apiContext.Users;
            _shoppingCartsCollection = apiContext.ShoppingCarts;
            _shoppingCartItemsCollection = apiContext.ShoppingCartItems;
            _completedOrdersCollection = apiContext.CompletedOrders;
            _completedOrderItemsCollection = apiContext.CompletedOrderItems;
            _jwtService = jwtService;
        }

        [HttpPost]
        public IActionResult RegisterUser(Users newUser)
        {
            var newShoppingCart = new ShoppingCarts { ItemsList = new List<ShoppingCartItems>() };
            _shoppingCartsCollection.InsertOne(newShoppingCart);

            var newCompletedOrder = new CompletedOrders { ItemsList = new List<CompletedOrderItems>() };
            _completedOrdersCollection.InsertOne(newCompletedOrder);

            newUser.ShoppingCartReference = newShoppingCart.Id;
            newUser.CompletedOrdersReference = newCompletedOrder.Id;

            _usersCollection.InsertOne(newUser);
            var jwtToken = _jwtService.GenerateJwtToken(newUser);
            return Ok(new { newUser, JWT = jwtToken });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var search = _usersCollection.Find(x => x.Email == loginModel.Email && x.Password == loginModel.Password).FirstOrDefault();
            if (search == null) return NotFound();
            var jwtToken = _jwtService.GenerateJwtToken(search);
            return Ok(jwtToken);
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var search = _usersCollection.Find(_ => true).ToList();
            if (search == null || search.Count == 0) return NotFound();
            else return Ok(search);
        }

        [HttpGet("profile"), Authorize]
        public IActionResult GetMyProfile()
        {
            var userId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest("unable to retrieve user Information");
            var search = _usersCollection.Find(x => x.Id == userId).FirstOrDefault();
            if (search == null) return NotFound();
            else return Ok(search);
        }

        [HttpGet("searchbyname")]
        public IActionResult GetUserByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Name parameter is required");
            var search = _usersCollection.Find(x => x.Name.Contains(name)).ToList();
            if (search == null) return NotFound();
            else return Ok(search);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var search = _usersCollection.Find(x => x.Id == id).FirstOrDefault();
            if (search == null) return NotFound();
            else return Ok(search);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUserById(string id)
        {
            var search = _usersCollection.Find(x => x.Id == id).FirstOrDefault();
            if (search == null) return (NotFound());
            else
            {
                _usersCollection.DeleteOne(x => x.Id == id);
                return NoContent();
            }
        }
    }
}