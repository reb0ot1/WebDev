using System;
using System.Collections.Generic;
using System.Text;

namespace MvcFramework.Tests.TestModels
{
    public class TestModel
    {
        public string String{ get; set; }

        public IEnumerable<string> List { get; set; }
    }
}
