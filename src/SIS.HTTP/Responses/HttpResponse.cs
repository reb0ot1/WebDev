using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Extensions;
using SIS.HTTP.Cookies;

namespace SIS.HTTP.Responses
{
    public class HttpResponse : IHttpResponse
    {

        public HttpResponse()
        {
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            this.Content = new byte[0];
        }

        public HttpResponse(HttpResponseStatusCode statusCode) : 
            this()
        {
            this.StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; private set; }

        public IHttpCookieCollection Cookies { get; private set; }

        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header)
        {
            this.Headers.Add(header);
        }

        public void AddCookie(HttpCookie cookie)
        {
            this.Cookies.Add(cookie);
        }

        public byte[] GetBytes()
        {
            //var responseString = string.Concat(this.ToString(), this.Content);
            var responseString = Encoding.UTF8.GetBytes(this.ToString()).Concat(this.Content);
            return responseString.ToArray();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.Append($"{GlobalConstants.HttpOneProtocolFragment} {(int)this.StatusCode} {this.StatusCode.ToString()}");
            result.Append(GlobalConstants.NewLine);
            result.Append(this.Headers);
            result.Append(GlobalConstants.NewLine);

            if (this.Cookies.HasCookies())
            {
                foreach (var cookie in this.Cookies)
                {
                    result.Append($"Set-Cookie: {cookie}").Append(GlobalConstants.NewLine);
                }
            }

            result.Append(GlobalConstants.NewLine);

            return result.ToString();
        }
    }
}
