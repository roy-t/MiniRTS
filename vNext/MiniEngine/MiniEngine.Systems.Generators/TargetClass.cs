using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiniEngine.Systems.Generators
{
    internal sealed class TargetClass
    {
        public TargetClass(ClassDeclarationSyntax parent)
        {
            this.Class = parent;
            this.ProcessAllMethods = new List<MethodDeclarationSyntax>();
            this.ProcessNewMethods = new List<MethodDeclarationSyntax>();
            this.ProcessChangedMethods = new List<MethodDeclarationSyntax>();
            this.ProcessMethods = new List<MethodDeclarationSyntax>();
        }

        public ClassDeclarationSyntax Class;

        public List<MethodDeclarationSyntax> ProcessAllMethods { get; }
        public List<MethodDeclarationSyntax> ProcessNewMethods { get; }
        public List<MethodDeclarationSyntax> ProcessChangedMethods { get; }

        public List<MethodDeclarationSyntax> ProcessMethods { get; }

        public IReadOnlyList<MethodDeclarationSyntax> Methods
            => this.ProcessAllMethods
                .Union(this.ProcessNewMethods)
                .Union(this.ProcessChangedMethods)
                .Union(this.ProcessMethods).ToList();
    }
}
