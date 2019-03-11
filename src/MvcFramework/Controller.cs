using System;
using System.Collections.Generic;
using System.IO;
using MvcFramework.Services;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace MvcFramework
{
    public class Controller
    {
        public Controller()
        {
            this._userCookieService = new UserCookieService();
        }

        public IHttpRequest Request { get; set; }

        protected IUserCookieService _userCookieService { get; }
        
        protected string GetUserName(IHttpRequest request)
        {
            if (request.Cookies.ContainsCookie(".auth-cakes"))
            {
                HttpCookie cookie = request.Cookies.GetCookie(".auth-cakes");
                var cookieContent = cookie.Value;

                return this._userCookieService.GetUserData(cookieContent);
            }

            return null;
        }

        protected IHttpResponse View(string viewName, IDictionary<string, string> viewBag = null)
        {
            if (viewBag == null)
            {
                viewBag = new Dictionary<string, string>();
            }

            var allContent = this.GetViewContent(viewName, viewBag);
            return new HtmlResult(allContent, HttpResponseStatusCode.Ok);
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            return new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.BadRequest);
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            return new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.InternalServerErorr);
        }

        private string GetViewContent(string viewName,
            IDictionary<string, string> viewBag)
        {
            var layoutContent = File.ReadAllText("Views/_Layout.html");
            var content = File.ReadAllText("Views/" + viewName + ".html");
            foreach (var item in viewBag)
            {
                content = content.Replace("@Model." + item.Key, item.Value);
            }

            var allContent = layoutContent.Replace("@RenderBody()", content);
            return allContent;
        }
    }
}
