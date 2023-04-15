//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System;
//using System.Diagnostics;

//namespace NetApp.SourceGenerator
//{
//    public class TrimOnReadSourceGenerator : ISourceGenerator
//    {
//        public void Execute(GeneratorExecutionContext context)
//        {
//            var syntaxTree = context.Compilation.SyntaxTrees;
//            var root = syntaxTree.;

//            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

//            foreach (var classDeclaration in classDeclarations)
//            {
//                var properties = classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();

//                foreach (var property in properties)
//                {
//                    if (property.Type.ToString() == "string")
//                    {
//                        var propertyName = property.Identifier.Text;
//                        var setAccessor = property.AccessorList.Accessors.FirstOrDefault(x => x.Kind() == SyntaxKind.SetAccessorDeclaration);

//                        if (setAccessor != null)
//                        {
//                            var newAccessor = setAccessor.WithBody(
//                                generator.Block(
//                                    generator.AssignmentStatement(
//                                        generator.IdentifierName(propertyName),
//                                        generator.InvocationExpression(
//                                            generator.MemberAccessExpression(
//                                                generator.IdentifierName(propertyName),
//                                                generator.IdentifierName("Trim")
//                                            )
//                                        )
//                                    )
//                                )
//                            );

//                            var newProperty = property.ReplaceNode(setAccessor, newAccessor);
//                            var newRoot = root.ReplaceNode(property, newProperty);

//                            context.AddSource("TrimOnRead", newRoot.ToString());
//                        }
//                    }
//                }
//            }
//        }
//    }

//        public void Initialize(GeneratorInitializationContext context)
//        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
//        }
//    }
//}
