using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIS.HTTP.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string text)
        {
            string newText = text.ToLower();

            return $"{newText.First().ToString().ToUpper()}{newText.Substring(1)}";
        }
    }
}
