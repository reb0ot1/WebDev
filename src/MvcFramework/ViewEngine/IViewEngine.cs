using System;
using System.Collections.Generic;
using System.Text;

namespace MvcFramework.ViewEngine
{
    public interface IViewEngine
    {
        string GetHtml(string viewCode);

    }
}
