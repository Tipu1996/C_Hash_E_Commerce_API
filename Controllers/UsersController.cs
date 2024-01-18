using eCommerceAPI.Data;
using eCommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
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
        public UsersController(ApiContext apiContext)
        {
            _usersCollection = apiContext.Users;
            _shoppingCartsCollection = apiContext.ShoppingCarts;
            _shoppingCartItemsCollection = apiContext.ShoppingCartItems;
            _completedOrdersCollection = apiContext.CompletedOrders;
            _completedOrderItemsCollection = apiContext.CompletedOrderItems;
        }

        [HttpPost]
        public IActionResult AddNewUser(Users newUser)
        {
            var newShoppingCart = new ShoppingCarts { ItemsList = new List<ShoppingCartItems>() };
            _shoppingCartsCollection.InsertOne(newShoppingCart);

            var newCompletedOrder = new CompletedOrders { ItemsList = new List<CompletedOrderItems>() };
            _completedOrdersCollection.InsertOne(newCompletedOrder);

            newUser.ShoppingCartReference = newShoppingCart.Id;
            newUser.CompletedOrdersReference = newCompletedOrder.Id;

            _usersCollection.InsertOne(newUser);
            return new JsonResult(Ok(newUser));
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var search = _usersCollection.Find(_ => true).ToList();
            if (search == null || search.Count == 0) return new JsonResult(NotFound());
            else return Ok(search);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var search = _usersCollection.Find(_ => true).FirstOrDefault();
            if (search == null) return new JsonResult(NotFound());
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