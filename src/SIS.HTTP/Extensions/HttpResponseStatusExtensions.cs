using System;
using System.Collections.Generic;
using System.Text;
using SIS.HTTP.Enums;

namespace SIS.HTTP.Extensions
{
    public static class HttpResponseStatusExtensions
    {

        public static string GetResponseLine(this HttpResponseStatusCode statusCode)
        {
            return $"{(int)statusCode} {GetResponseCodeText(statusCode)}";
        }

        private static string GetResponseCodeText(HttpResponseStatusCode code)
        {
            Dictionary<HttpResponseStatusCode, string> textCodes = new Dictionary<HttpResponseStatusCode, string>()
            {
                { HttpResponseStatusCode.Ok, "Ok"},
                { HttpResponseStatusCode.Created, "Created"},
                { HttpResponseStatusCode.Redirect, "Redirect"},
                { HttpResponseStatusCode.Found, "Found"},
                { HttpResponseStatusCode.SeeOther, "See Other"},
                { HttpResponseStatusCode.BadRequest, "Bad Request"},
                { HttpResponseStatusCode.Unauthorized, "Unauthorized"},
                { HttpResponseStatusCode.Forbidden, "Forbidden"},
                { HttpResponseStatusCode.NotFound, "Not Found"},
                { HttpResponseStatusCode.InternalServerErorr, "Internal Server Error"}
            };

            return textCodes[code];
        }
    }
}
