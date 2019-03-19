using System;
using SIS.HTTP.Enums;

namespace MvcFramework
{
    public class HttpPostAttribute : HttpAttribute
    {
        public HttpPostAttribute(string path) 
            : base(path)
        {
        }

        public override HttpRequestMethod Method => HttpRequestMethod.Post;
    }
}
