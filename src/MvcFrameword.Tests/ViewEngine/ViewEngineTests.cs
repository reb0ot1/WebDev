using System.Collections.Generic;
using System.IO;
using MvcFramework.Tests.TestModels;
using MvcFramework.ViewEngine;
using Xunit;

namespace MvcFramework.Tests.ViewEngine
{
    public class ViewEngineTests
    {
        [Theory]
        [InlineData("IfForAndForeach")]
        [InlineData("ViewWithNoCode")]
        [InlineData("WorkWithViewModel")]
        public void RunTestViews(string testViewName)
        {
            var fileContent = File.ReadAllText($"TestViews/{testViewName}.html");
            var expectedResult = File.ReadAllText($"TestViews/{testViewName}.Result.html");

            IViewEngine viewEngine = new MvcFramework.ViewEngine.ViewEngine();

            var model = new TestModel
            {
                String = "Username",
                List = new List<string> { "Item1", "item2", "test", "123", "" }
            };

            var result = viewEngine.GetHtml(testViewName, fileContent, model, null);

            Assert.Equal(expectedResult, result);
        }
    }
}
