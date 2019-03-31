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
using System.Text.RegularExpressions;

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
    public class "+ viewTypeName + @" : IView<"+ typeof(T).FullName.Replace("+", ".") +@">
    {
        public string GetHtml("+ typeof(T).FullName.Replace("+", ".") + @" model)
        {
            var Model = model;
            StringBuilder html = new StringBuilder();
            "+csharpMethodBody+@"

            return html.ToString().TrimEnd();
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
                .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IEnumerable<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(
                    new AssemblyName("netstandard")).Location))
                .AddReferences(MetadataReference.CreateFromFile(viewModelType.Assembly.Location));
                

            var netStandardReferences = Assembly.Load(new AssemblyName("netstandard")).GetReferencedAssemblies();

            foreach (var netstandardReference in netStandardReferences)
            {
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(Assembly.Load(netstandardReference).Location));
            }

            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(csharpCode));

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
            var lines = this.GetLines(viewCode);
            var stringBuilder = new StringBuilder();

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("{") ||
                    line.Trim().EndsWith("}") ||
                    line.Trim().StartsWith("@for") ||
                    line.Trim().StartsWith("@if") ||
                    line.Trim().StartsWith("@else"))
                {
                    var firstAtSymbolIndex = line.IndexOf("@", StringComparison.InvariantCulture);
                    stringBuilder.AppendLine(this.RemoveAt(line, firstAtSymbolIndex));
                }
                else
                {
                    var htmlLine = line.Replace("\"", "\\\"");
                    while (htmlLine.Contains("@"))
                    {
                        var specialSymbolIndex = htmlLine.IndexOf("@");
                        var endOfCode = new Regex(@"[\s\<\\]+").Match(htmlLine, specialSymbolIndex).Index;
                        string expression = null;
                        if (endOfCode == 0 || endOfCode == -1)
                        {
                            expression = htmlLine.Substring(specialSymbolIndex + 1);
                            htmlLine = htmlLine.Substring(0, specialSymbolIndex) +
                            "\" +" + expression + "+ \"";
                        }
                        else
                        {
                            expression = htmlLine.Substring(specialSymbolIndex + 1, endOfCode - specialSymbolIndex - 1);
                            htmlLine = htmlLine.Substring(0, specialSymbolIndex) +
                            "\" +" + expression + "+ \"" + htmlLine.Substring(endOfCode);

                        }

                    }


                    stringBuilder.AppendLine($"html.AppendLine(\"{htmlLine}\");");
                }
            }
            return stringBuilder.ToString();
        }

        private IEnumerable<string> GetLines(string input)
        {
            var stringReader = new StringReader(input);
            var lines = new List<string>();

            string line = null;
            while((line = stringReader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            return lines;
        }

        private string RemoveAt(string input, int index)
        {
            if (index == -1)
            {
                return input;
            }

            return input.Substring(0, index) + input.Substring(index + 1);
        }
    }
}
