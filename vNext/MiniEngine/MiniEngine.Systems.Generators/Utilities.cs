using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiniEngine.Systems.Generators
{
    public static class Utilities
    {
        public static string GetNamespace(Compilation compilation, TypeDeclarationSyntax type)
        {
            var space = type.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            var model = compilation.GetSemanticModel(space.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(space) as INamespaceSymbol;

            var names = new List<string>();
            do
            {
                names.Insert(0, symbol.Name);
                symbol = symbol.ContainingNamespace;
            } while (symbol.ContainingNamespace != null);
            return string.Join(".", names);
        }

        public static IReadOnlyList<string> GetMethodParameters(Compilation compilation, MethodDeclarationSyntax method)
        {
            var model = compilation.GetSemanticModel(method.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(method) as IMethodSymbol;

            var parameters = new List<string>();
            foreach (var parameter in symbol.Parameters)
            {
                parameters.Add(parameter.Type.Name);
            }

            return parameters;
        }
    }
}
