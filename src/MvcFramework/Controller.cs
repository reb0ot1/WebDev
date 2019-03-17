using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MvcFramework.Services;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace MvcFramework
{
    public class Controller
    {
        public Controller()
        {
            this._userCookieService = new UserCookieService();
            this.Response = new HttpResponse();
            this.Response.StatusCode = HttpResponseStatusCode.Ok;
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

        protected IUserCookieService _userCookieService { get; }
        
        protected string GetUserName()
        {
            if (this.Request.Cookies.ContainsCookie(".auth-cakes"))
            {
                HttpCookie cookie = this.Request.Cookies.GetCookie(".auth-cakes");
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
            this.PrepareHtmlResult(allContent);

            return this.Response;
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            this.PrepareHtmlResult($"<h1>{errorMessage}</h1>");
            this.Response.StatusCode = HttpResponseStatusCode.BadRequest;

            return this.Response;
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            this.PrepareHtmlResult($"<h1>{errorMessage}</h1>");
            this.Response.StatusCode = HttpResponseStatusCode.InternalServerErorr;

            return this.Response;
        }

        protected IHttpResponse File(byte[] content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentDisposition, "inline"));
            this.Response.Content = content;

            return this.Response;
        }

        protected IHttpResponse Redirect(string location)
        {
            this.Response.StatusCode = HttpResponseStatusCode.SeeOther;
            this.Response.Headers.Add(new HttpHeader("Location", location));

            return this.Response;
        }

        protected IHttpResponse Text(string content)
        {
            this.Response.Headers.Add(new HttpHeader("Content-Type", "text/plain"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);

            return this.Response;
        }

        private string GetViewContent(string viewName,
            IDictionary<string, string> viewBag)
        {
            var layoutContent = System.IO.File.ReadAllText("Views/_Layout.html");
            var content = System.IO.File.ReadAllText("Views/" + viewName + ".html");
            foreach (var item in viewBag)
            {
                content = content.Replace("@Model." + item.Key, item.Value);
            }

            var allContent = layoutContent.Replace("@RenderBody()", content);
            return allContent;
        }

        private void PrepareHtmlResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
