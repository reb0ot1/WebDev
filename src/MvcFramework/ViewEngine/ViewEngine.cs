using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace MvcFramework.ViewEngine
{
    public class ViewEngine : IViewEngine
    {
        public string GetHtml<T>(string viewName, string viewCode, T model)
        {
            var viewTypeName = viewName + "View";
            var csharpMethodBody = this.GenerateCSharpMethodBody(viewCode);
            var viewCodeAsCSharpCode = @"
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MvcFramework.ViewEngine;
using "+typeof(T).Namespace+@";
namespace MyAppViews
{
    public class "+ viewTypeName + @" : IView<"+ typeof(T).FullName +@">
    {
        public string GetHtml("+ typeof(T).FullName +@" model)
        {
            StringBuilder html = new StringBuilder();
            "+csharpMethodBody+@"

            return html.ToString();
        }
    }
}
";

            var instanceOfViewClass = this.GetInstance(viewCodeAsCSharpCode, "MyAppViews."+viewTypeName, typeof(T)) as IView<T>;

            var html = instanceOfViewClass.GetHtml(model);

            return html;
        }

        private object GetInstance(string csharpCode, string typeName, Type viewModelType)
        {
            //Roslyn
            var compilation = CSharpCompilation.Create(Path.GetRandomFileName() + ".dll")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IEnumerable<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(
                    new AssemblyName("netstandard")).Location))
                .AddReferences(MetadataReference.CreateFromFile(viewModelType.Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(csharpCode));
            

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> errors = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in errors)
                    {
                        Console.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                    //return new ReturnValue<MethodInfo>(false, "The following compile errors were encountered: " + message.ToString(), null);
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                var viewType = assembly.GetType(typeName);

                return Activator.CreateInstance(viewType);
            }
        }

        private string GenerateCSharpMethodBody(string viewCode)
        {
            return string.Empty;
        }
    }
}
