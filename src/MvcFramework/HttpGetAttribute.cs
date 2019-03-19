using System;
using System.Collections.Generic;
using System.Text;
using SIS.HTTP.Enums;

namespace MvcFramework
{
    public class HttpGetAttribute : HttpAttribute
    {
        public HttpGetAttribute(string path)
            : base(path)
        {
        }

        public override HttpRequestMethod Method => HttpRequestMethod.Get;
    }
}
