using CakesWebApp.ViewModels.User;
using MvcFramework;
using SIS.HTTP.Responses;
using System.Linq;

namespace CakesWebApp.Controllers
{
    public class UserController : BaseController
    {
        [HttpGet("/user/profile")]
        public IHttpResponse UserProfile()
        {
            var user = this.Db.Users
                .Where(x => x.Username == this.User)
                .Select(x => new ProfileViewModel
                            { Username = x.Username,
                                RegisteredOn = x.DateOfRegistration,
                                OrdersCount = x.Orders.Count()
                            }).FirstOrDefault();

            if (user == null)
            {
                return this.BadRequestError("Profile page not accessible for this user.");
            }
            // TODO: Read profile data
            // TODO: Create view model
            return this.View("Profile", user);
        }
    }
}
