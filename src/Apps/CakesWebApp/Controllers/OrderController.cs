
namespace CakesWebApp.Controllers
{
    using CakesWebApp.Models;
    using MvcFramework;
    using SIS.HTTP.Responses;
    using System.Linq;

    public class OrderController : BaseController
    {
        [HttpPost("/orders/add")]
        public IHttpResponse Add(int productId)
        {
            var userId = this.Db.Users.FirstOrDefault(x => x.Username == this.User)?.Id;
            if (userId == null)
            {
                return this.BadRequestError("Please login first");
            }
            
            var lastUserOrder = this.Db.Orders
                .Where(x => x.User.Id == userId)
                .OrderBy(x => x.Id).FirstOrDefault();

            if (lastUserOrder == null)
            {
                lastUserOrder = new Order
                {
                    UserId = userId.Value
                };

                this.Db.Orders.Add(lastUserOrder);
                this.Db.SaveChanges();
            }

            var orderProduct = new OrderProduct
            {
                OrderId = lastUserOrder.Id,
                ProductId = productId
            };

            this.Db.OrderProduct.Add(orderProduct);
            this.Db.SaveChanges();

            return this.Redirect("/orders/byid/" + lastUserOrder.Id);
        }
    }

    public HttpResponse
}
