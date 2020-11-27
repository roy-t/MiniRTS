using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiniEngine.Systems.Generators.System
{
    internal sealed class SyntaxReceiver : ISyntaxReceiver
    {
        public readonly Dictionary<ClassDeclarationSyntax, SystemClass> Targets = new Dictionary<ClassDeclarationSyntax, SystemClass>();
        private readonly Type ProcessAllType = typeof(ProcessAllAttribute);
        private readonly Type ProcessNewType = typeof(ProcessNewAttribute);
        private readonly Type ProcessChangedType = typeof(ProcessChangedAttribute);
        private readonly Type ProcessType = typeof(ProcessAttribute);

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is MethodDeclarationSyntax method)
            {
                foreach (var attributeList in method.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        if (this.MatchesAttribute(attribute, this.ProcessAllType))
                        {
                            this.GetTargetClass(method).ProcessAllMethods.Add(method);
                        }
                        else if (this.MatchesAttribute(attribute, this.ProcessChangedType))
                        {
                            this.GetTargetClass(method).ProcessChangedMethods.Add(method);
                        }
                        else if (this.MatchesAttribute(attribute, this.ProcessNewType))
                        {
                            this.GetTargetClass(method).ProcessNewMethods.Add(method);
                        }
                        else if (this.MatchesAttribute(attribute, this.ProcessType))
                        {
                            this.GetTargetClass(method).ProcessMethods.Add(method);
                        }
                    }
                }
            }
        }

        private SystemClass GetTargetClass(MethodDeclarationSyntax syntaxNode)
        {
            var parent = syntaxNode.Parent as ClassDeclarationSyntax;
            if (!this.Targets.TryGetValue(parent, out var target))
            {
                target = new SystemClass(parent);
                this.Targets.Add(parent, target);
            }

            return target;
        }

        private bool MatchesAttribute(AttributeSyntax attribute, Type attributeType)
        {
            var name = attributeType.Name;
            var shortName = attributeType.Name.Split(new[] { "Attribute" }, StringSplitOptions.RemoveEmptyEntries)[0];

            return attribute.Name.ToString() == name || attribute.Name.ToString() == shortName;
        }
    }
}
