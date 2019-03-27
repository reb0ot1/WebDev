using System.IO;
using MvcFramework.ViewEngine;
using Xunit;

namespace MvcFramework.Tests.ViewEngine
{
    public class ViewEngineTests
    {
        [Theory]
        [InlineData("IfForEndForeach")]
        [InlineData("ViewWithNoCode")]
        [InlineData("WorkWithViewModel")]
        public void RunTestViews(string testViewName)
        {
            var fileContent = File.ReadAllText($"TestViews/{testViewName}.html");
            var expectedResult = File.ReadAllText($"TestViews/{testViewName}.Result.html");

            IViewEngine viewEngine = new MvcFramework.ViewEngine.ViewEngine();

            var result = viewEngine.GetHtml(fileContent);

            Assert.Equal(expectedResult, result);
        }
    }
}
