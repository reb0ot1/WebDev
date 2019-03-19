using System;
using System.Collections.Generic;
using System.Text;
using CakesWebApp.Controllers;
using MvcFramework;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp
{
    public class HomeController : BaseController
    {
        [HttpGet("/")]
        public IHttpResponse Index()
        {
            return this.View("Index");
        }

        [HttpGet("/hello")]
        public IHttpResponse HelloUSer()
        {
            return this.View("HelloUser", 
                new Dictionary<string, string> {
                    {"UserName", this.User }
                }); 
        }
    }
}
