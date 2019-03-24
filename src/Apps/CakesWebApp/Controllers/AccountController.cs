using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CakesWebApp.Models;
using MvcFramework;
using MvcFramework.Services;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public class AccountController : BaseController
    {
        private IHashService _hashService;

        public AccountController(IHashService hashService)
        {
            this._hashService = hashService;
        }

        [HttpGet("/register")]
        public IHttpResponse Register()
        {
            return this.View("Register");
        }

        [HttpPost("/register")]
        public IHttpResponse DoRegister()
        {
            var userName = this.Request.FormData["username"].ToString().Trim();
            var password = this.Request.FormData["password"].ToString();
            var confirmPassword = this.Request.FormData["confirmPassword"].ToString();

            if (string.IsNullOrWhiteSpace(userName) || userName.Length < 4)
            {
                return this.BadRequestError("Please provide username with length bigger than 4 symbols");
            }

            if (Db.Users.Any(u => u.Name == userName))
            {
                return this.BadRequestError("This username already exists");
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                return this.BadRequestError("Please provide password");
            }

            if (password != confirmPassword)
            {
                return this.BadRequestError("Passwords should match");
            }

            var hashedPassword = this._hashService.Hash(password);

            var user = new User
            {
                Name = userName,
                Username = userName,
                Password = hashedPassword
            };

            //Tuk trqbva da byde s async zaqvka.
            this.Db.Users.Add(user);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception e)
            {
                //TODO: Log to error log
                return this.ServerError(e.Message);
            }


            return this.Redirect("/");
        }

        [HttpGet("/login")]
        public IHttpResponse Login()
        {
            return this.View("Login");
        }

        [HttpPost("/login")]
        public IHttpResponse DoLogin()
        {
            var userName = this.Request.FormData["username"].ToString().Trim();

            var password = this.Request.FormData["password"].ToString();

            var hashedPassword = this._hashService.Hash(password);

            var user = this.Db.Users.FirstOrDefault(u => u.Username == userName && u.Password == hashedPassword);

            if (user == null)
            {
                return this.BadRequestError("Invalid username and password");
            }
            
            
            var userCookie = this.UserCookieService.GetUserCookie(user.Username);
            this.Response.AddCookie(new HttpCookie(".auth-cakes", userCookie, 7));

            return this.Redirect("/");
        }

        [HttpGet("/logout")]
        public IHttpResponse Logout()
        {
            if (!this.Request.Cookies.ContainsCookie(".auth-cakes"))
            {
                return this.Redirect("/");
            }

            var cookie = this.Request.Cookies.GetCookie(".auth-cakes");

            cookie.Delete();

            this.Response.AddCookie(cookie);

            return this.Redirect("/");
        }
    }
}