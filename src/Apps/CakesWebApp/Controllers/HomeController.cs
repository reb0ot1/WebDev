using System;
using System.Collections.Generic;
using System.Text;
using CakesWebApp.Controllers;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp
{
    public class HomeController : BaseController
    {
        public IHttpResponse Index()
        {
            return this.View("Index");
        }

        public IHttpResponse HelloUSer()
        {
            return new HtmlResult($"Hello {this.GetUserName(this.Request)}", HttpResponseStatusCode.Ok); 
        }
    }
}
