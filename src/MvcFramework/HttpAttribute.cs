using System;
using System.Collections.Generic;
using System.Text;
using SIS.HTTP.Enums;

namespace MvcFramework
{
    public abstract class HttpAttribute : Attribute
    {
        public HttpAttribute(string path)
        {
            this.Path = path;
        }

        public string Path { get; set; }

        public abstract HttpRequestMethod Method { get; }
    }
}
