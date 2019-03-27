using System;
using System.Collections.Generic;
using System.Text;

namespace MvcFramework.ViewEngine
{
    public class ViewEngine : IViewEngine
    {
        public string GetHtml(string viewCode)
        {
            return viewCode;
        }
    }
}
