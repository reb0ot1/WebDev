using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CakesWebApp.Models;
using CakesWebApp.ViewModels.Account;
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
        public IHttpResponse DoRegister(DoRegisterInputVM model)
        {

            if (string.IsNullOrWhiteSpace(model.Username) || model.Username.Trim().Length < 4)
            {
                return this.BadRequestError("Please provide username with length bigger than 4 symbols");
            }

            if (Db.Users.Any(u => u.Name == model.Username.Trim()))
            {
                return this.BadRequestError("This username already exists");
            }

            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                return this.BadRequestError("Please provide password");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return this.BadRequestError("Passwords should match");
            }

            var hashedPassword = this._hashService.Hash(model.Password);

            var user = new User
            {
                Name = model.Username.Trim(),
                Username = model.Username,
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
        public IHttpResponse DoLogin(UserVM model)
        {
            var hashedPassword = this._hashService.Hash(model.Password);

            var user = this.Db.Users.FirstOrDefault(u => u.Username == model.Username.Trim() && u.Password == hashedPassword);

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