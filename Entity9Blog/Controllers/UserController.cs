using DAL.Entities;
using DAL.Entities.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Entity9Blog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly NorthwindContext _context;
        public UserController(NorthwindContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetUserList")]
        public async Task<List<Users>> Get()
        {
            var users = await _context.Users.ToListAsync();            
            return users;
        }

        [HttpGet("GetProductByID/{productID}", Name = "GetProductByID")]
        public async Task<Products?> GetProduct(int productID)
        {
            var productQuery = EF.CompileAsyncQuery((NorthwindContext context, int id) =>
                context.Products.FirstOrDefault(p => p.ProductId == id));

            var product = await productQuery(_context, productID);
            return product;
        }

        [HttpGet("GetTop3Orders", Name = "GetTop3Orders")]
        public async Task<List<Orders>> GetTop3Orders()
        {
            return _context.Orders.OrderByDescending(o => o.OrderId).Take(3).ToList();
        }
        [HttpGet("UpdateOrders/{orderID}", Name = "UpdateOrders")]
        public Orders UpdateOrders(int orderID)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderID);
            if (order != null)
            {
                //_context.Entry(order).Property("OrderDate").CurrentValue = DateTime.Now;
                order.ShipName = "BoraShip";
                _context.SaveChanges();
                return order;
            }
            return new Orders();
        }

        [HttpGet("UpdateProducts/{productID}/{productCategory}", Name = "UpdateProducts")]
        public Products UpdateProducts(int productID, int productCategory)
        {
            var product = _context.Products.FirstOrDefault(o => o.ProductId == productID);
            if (product != null)
            {
                //_context.Entry(order).Property("OrderDate").CurrentValue = DateTime.Now;
                product.CategoryName = (ProductCategoryEnum)productCategory;
                _context.SaveChanges();
                return product;
            }
            return new Products();
        }
    }
}
