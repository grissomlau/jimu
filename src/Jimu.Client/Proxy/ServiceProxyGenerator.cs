using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Jimu.Logger;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyModel;

namespace Jimu.Client.Proxy.CodeAnalysisIntegration
{
    class ServiceProxyGenerator : IServiceProxyGenerator, IDisposable
    {
        private readonly ILogger _logger;
        private IEnumerable<Type> _generatedServiceProxyTypes;

        public ServiceProxyGenerator(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<Type> GetGeneratedServiceProxyTypes()
        {
            return _generatedServiceProxyTypes;
        }
        public IEnumerable<Type> GenerateProxy(IEnumerable<Type> interfaceTypes)
        {
#if NET
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
#else

            var assemblies = DependencyContext.Default.RuntimeLibraries.SelectMany(i => i.GetDefaultAssemblyNames(DependencyContext.Default).Select(z => Assembly.Load(new AssemblyName(z.Name)))).ToList();
#endif
            //IList<Assembly> assemblies = new List<Assembly>();
            foreach (var t in interfaceTypes)
            {
                assemblies.Add(t.Assembly);
            }
            var trees = interfaceTypes.Select(GenerateProxyTree).ToList();
            var stream = CompilationUtilitys.CompileClientProxy(trees,
                assemblies.Select(x => MetadataReference.CreateFromFile(x.Location))
                .Concat(new[]
                {
                    MetadataReference.CreateFromFile(typeof(Task).GetTypeInfo().Assembly.Location)
                }),
                _logger
                );
            using (stream)
            {
#if NET
                var assembly = Assembly.Load(stream.ToArray());
#else
                var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
#endif
                _generatedServiceProxyTypes = assembly.GetExportedTypes();
                return _generatedServiceProxyTypes;

            }
        }

        public SyntaxTree GenerateProxyTree(Type interfaceType)
        {
            var className = interfaceType.Name.StartsWith("I") ?
                interfaceType.Name.Substring(1) : interfaceType.Name;
            className += "ClientProxy";
            var members = new List<MemberDeclarationSyntax>
            {
                GetConstructorDeclaration(className)
            };

            members.AddRange(GenerateMethodDeclarations(interfaceType.GetMethods()));
            return SyntaxFactory.CompilationUnit()
                  .WithUsings(GetUsings())
                  .WithMembers(
                      SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                          SyntaxFactory.NamespaceDeclaration(
                              SyntaxFactory.QualifiedName(
                                  SyntaxFactory.QualifiedName(
                                      SyntaxFactory.IdentifierName("Jimu"),
                                      SyntaxFactory.IdentifierName("Proxy")),
                                  SyntaxFactory.IdentifierName("ClientProxy")))
                  .WithMembers(
                      SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                          SyntaxFactory.ClassDeclaration(className)
                              .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                              .WithBaseList(
                                  SyntaxFactory.BaseList(
                                      SyntaxFactory.SeparatedList<BaseTypeSyntax>(
                                          new SyntaxNodeOrToken[]
                                          {
                                            SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("ServiceProxyBase")),
                                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                                            SyntaxFactory.SimpleBaseType(GetQualifiedNameSyntax(interfaceType))
                                          })))
                              .WithMembers(SyntaxFactory.List(members))))))
                  .NormalizeWhitespace().SyntaxTree;
        }
        private static SyntaxList<UsingDirectiveSyntax> GetUsings()
        {
            return SyntaxFactory.List(
                new[]
                {
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax("System.Threading.Tasks")),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax("System.Collections.Generic")),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax(typeof(IRemoteServiceCaller).Namespace)),
                    //SyntaxFactory.UsingDirective(GetQualifiedNameSyntax(typeof(ISerializer).Namespace)),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax(typeof(ServiceProxyBase).Namespace))
                });
        }

        private IEnumerable<MemberDeclarationSyntax> GenerateMethodDeclarations(IEnumerable<MethodInfo> methods)
        {
            var arry = methods.ToArray();
            return arry.Select(GenerateMethodDeclaration).ToArray();
        }
        private static TypeSyntax GetTypeSyntax(Type type)
        {
            if (type == null)
                return null;

            if (!type.GetTypeInfo().IsGenericType)
                return GetQualifiedNameSyntax(type.FullName);

            var list = new List<SyntaxNodeOrToken>();

            foreach (var genericTypeArgument in type.GenericTypeArguments)
            {
                list.Add(genericTypeArgument.GetTypeInfo().IsGenericType
                    ? GetTypeSyntax(genericTypeArgument)
                    : GetQualifiedNameSyntax(genericTypeArgument.FullName));
                list.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
            }

            var array = list.Take(list.Count - 1).ToArray();
            var typeArgumentListSyntax = SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(array));
            return SyntaxFactory.GenericName(type.Name.Substring(0, type.Name.IndexOf('`')))
                .WithTypeArgumentList(typeArgumentListSyntax);
        }

        private static ConstructorDeclarationSyntax GetConstructorDeclaration(string className)
        {
            return SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier(className))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(
                   SyntaxFactory.ParameterList(
                       SyntaxFactory.SeparatedList<ParameterSyntax>(
                           new SyntaxNodeOrToken[]
                           {
                               SyntaxFactory.Parameter(SyntaxFactory.Identifier("remoteServiceCaller")).WithType(SyntaxFactory.IdentifierName("IRemoteServiceCaller"))
                           }
                           )
                       )
                )
                .WithInitializer(SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName("remoteServiceCaller"))
                        }
                        )
                    )
                ))
                .WithBody(SyntaxFactory.Block());
        }
        private static QualifiedNameSyntax GetQualifiedNameSyntax(string fullName)
        {
            return GetQualifiedNameSyntax(fullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static QualifiedNameSyntax GetQualifiedNameSyntax(IReadOnlyCollection<string> names)
        {
            var ids = names.Select(SyntaxFactory.IdentifierName).ToArray();

            var index = 0;
            QualifiedNameSyntax left = null;
            while (index + 1 < names.Count)
            {
                left = left == null ? SyntaxFactory.QualifiedName(ids[index], ids[index + 1]) : SyntaxFactory.QualifiedName(left, ids[index + 1]);
                index++;
            }
            return left;
        }
        private static QualifiedNameSyntax GetQualifiedNameSyntax(Type type)
        {
            var fullName = type.Namespace + "." + type.Name;
            return GetQualifiedNameSyntax(fullName);
        }
        private MemberDeclarationSyntax GenerateMethodDeclaration(MethodInfo method)
        {
            var serviceId = JimuHelper.GenerateServiceId(method);
            var returnDeclaration = GetTypeSyntax(method.ReturnType);

            var parameterList = new List<SyntaxNodeOrToken>();
            var parameterDeclarationList = new List<SyntaxNodeOrToken>();

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType.IsGenericType)
                {
                    parameterDeclarationList.Add(SyntaxFactory.Parameter(
                                     SyntaxFactory.Identifier(parameter.Name))
                                     .WithType(GetTypeSyntax(parameter.ParameterType)));
                }
                else
                {
                    parameterDeclarationList.Add(SyntaxFactory.Parameter(
                                        SyntaxFactory.Identifier(parameter.Name))
                                        .WithType(GetQualifiedNameSyntax(parameter.ParameterType)));

                }
                parameterDeclarationList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));

                parameterList.Add(SyntaxFactory.InitializerExpression(
                    SyntaxKind.ComplexElementInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(
                        new SyntaxNodeOrToken[]{
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(parameter.Name)),
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                            SyntaxFactory.IdentifierName(parameter.Name)})));
                parameterList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
            }
            if (parameterList.Any())
            {
                parameterList.RemoveAt(parameterList.Count - 1);
                parameterDeclarationList.RemoveAt(parameterDeclarationList.Count - 1);
            }

            MethodDeclarationSyntax declaration;
            if (method.ReturnType == typeof(void))
            {

                declaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), SyntaxFactory.Identifier(method.Name));
            }
            else
            {
                declaration = SyntaxFactory.MethodDeclaration(
                   returnDeclaration,
                   SyntaxFactory.Identifier(method.Name));
            }

            if (method.ReturnType.Namespace == typeof(Task).Namespace)
            {

                declaration = declaration.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.AsyncKeyword)));
            }
            else
            {
                declaration = declaration.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
            }

            declaration = declaration.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(parameterDeclarationList)));

            ExpressionSyntax expressionSyntax;
            StatementSyntax statementSyntax;

            if (method.ReturnType.Namespace != typeof(Task).Namespace)
            {
                if (method.ReturnType == typeof(void))
                {
                    expressionSyntax = SyntaxFactory.IdentifierName("InvokeVoid");
                }
                else
                {
                    expressionSyntax = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("Invoke"))
                    //.WithTypeArgumentList(((GenericNameSyntax)returnDeclaration).TypeArgumentList);
                    .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(returnDeclaration)))
                   ;
                }
                expressionSyntax =
                    SyntaxFactory.InvocationExpression(expressionSyntax)
                    .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {  SyntaxFactory.Argument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(serviceId))),
                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.ObjectCreationExpression(
                                                SyntaxFactory.GenericName(
                                                    SyntaxFactory.Identifier("Dictionary"))
                                                    .WithTypeArgumentList(
                                                        SyntaxFactory.TypeArgumentList(
                                                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
                                                                }))))
                                                .WithInitializer(
                                                    SyntaxFactory.InitializerExpression(
                                                        SyntaxKind.CollectionInitializerExpression,
                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                            parameterList))))

                                    })));
                //expressionSyntax = SyntaxFactory.express
            }
            else
            {
                if (method.ReturnType == typeof(Task))
                {

                    expressionSyntax = SyntaxFactory.IdentifierName("InvokeVoidAsync");
                }
                else
                {
                    expressionSyntax = SyntaxFactory.GenericName(
             SyntaxFactory.Identifier("InvokeAsync"))
             .WithTypeArgumentList(((GenericNameSyntax)returnDeclaration).TypeArgumentList);
                    //.WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(returnDeclaration)))
                }
                expressionSyntax = SyntaxFactory.AwaitExpression(
                    SyntaxFactory.InvocationExpression(expressionSyntax)
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {  SyntaxFactory.Argument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(serviceId))),
                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.ObjectCreationExpression(
                                                SyntaxFactory.GenericName(
                                                    SyntaxFactory.Identifier("Dictionary"))
                                                    .WithTypeArgumentList(
                                                        SyntaxFactory.TypeArgumentList(
                                                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
                                                                }))))
                                                .WithInitializer(
                                                    SyntaxFactory.InitializerExpression(
                                                        SyntaxKind.CollectionInitializerExpression,
                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                            parameterList))))

                                    }))));
            }

            if (method.ReturnType != typeof(Task) && method.ReturnType != typeof(void))
            {
                statementSyntax = SyntaxFactory.ReturnStatement(expressionSyntax);
            }
            else
            {
                statementSyntax = SyntaxFactory.ExpressionStatement(expressionSyntax);
            }

            declaration = declaration.WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.SingletonList(statementSyntax)));

            return declaration;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


    }
}
