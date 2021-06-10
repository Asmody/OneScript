/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Linq;
using OneScript.Contexts;
using OneScript.DependencyInjection;
using OneScript.Language;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Native.Runtime;
using OneScript.Values;

namespace OneScript.Native.Compiler
{
    public class ModuleCompiler : ExpressionTreeGeneratorBase
    {
        private readonly IServiceContainer _runtimeServices;
        private DynamicModule _module;

        public ModuleCompiler(IErrorSink errors, IServiceContainer runtimeServices) : base(errors)
        {
            _runtimeServices = runtimeServices;
        }
        
        public DynamicModule Compile(
            ModuleInformation moduleInfo,
            BslSyntaxNode moduleNode,
            SymbolTable symbols
            )
        {
            InitContext(Errors, moduleInfo, symbols);
            
            _module = new DynamicModule
            {
                ModuleInformation = ModuleInfo
            };

            Visit(moduleNode);

            return _module;
        }

        protected override BslWalkerContext MakeContext()
        {
            var c = base.MakeContext();
            c.Services = _runtimeServices;
            return c;
        }

        /// <summary>
        /// Заранее заготавливаем символы методов, т.к. выражения могут ссылатся на методы,
        /// объявленные позже в теле модуля.
        /// </summary>
        /// <param name="module"></param>
        private void RegisterLocalMethods(BslSyntaxNode module)
        {
            var methodsSection = module.Children.FirstOrDefault(x => x.Kind == NodeKind.MethodsSection);
            if(methodsSection == default)
                return;
            
            foreach (var methodNode in methodsSection.Children.Cast<MethodNode>())
            {
                var factory = new BslMethodInfoFactory<BslNativeMethodInfo>(() => new BslNativeMethodInfo());
                var builder = factory.NewMethod();
                VisitMethodSignature(builder, methodNode.Signature);

                var methodInfo = builder.Build();
                var symbol = new MethodSymbol
                {
                    Name = methodInfo.Name,
                    MemberInfo = methodInfo
                };
                
                Symbols.TopScope().Methods.Add(symbol, methodInfo.Name);
            }
        }

        private void VisitMethodSignature(BslMethodBuilder<BslNativeMethodInfo> builder, MethodSignatureNode node)
        {
            builder
                .Name(node.MethodName)
                .ReturnType(node.IsFunction ? typeof(BslValue): typeof(void))
                .IsExported(node.IsExported);

            var parameters = node.GetParameters().Select(CreateParameterInfo);
            
            builder.SetParameters(parameters);
        }

        private BslParameterInfo CreateParameterInfo(MethodParameterNode paramNode)
        {
            var param = new BslParameterInfo(paramNode.Name);
            if(paramNode.IsByValue)
                param.SetByVal();
            
            if(paramNode.HasDefaultValue)
                param.SetDefaultValue(CompilerHelpers.ValueFromLiteral(paramNode.DefaultValue));

            var attributes = CompilerHelpers.GetAnnotations(paramNode.Annotations);
            foreach (var attribute in attributes.Cast<Attribute>())
            {
                param.AddAttribute(attribute);
            }

            return param;
        }

        protected override void VisitModule(ModuleNode node)
        {
            var moduleScope = new SymbolScope();
            Symbols.AddScope(moduleScope);
            
            RegisterLocalMethods(node);
            base.VisitModule(node);
            
            Symbols.PopScope();
        }

        protected override void VisitModuleVariable(VariableDefinitionNode varNode)
        {
            var annotations = CompilerHelpers.GetAnnotations(varNode.Annotations);
            var field = new BslNativeFieldInfo(varNode.Name);
            field.SetExportFlag(varNode.IsExported);
            field.SetAnnotations(annotations);
            
            _module.Fields.Add(field);
        }

        protected override void VisitMethod(MethodNode methodNode)
        {
            var methodSymbol = Symbols.TopScope().Methods[methodNode.Signature.MethodName];
            var methodInfo = (BslNativeMethodInfo)methodSymbol.MethodInfo;

            var context = MakeContext();
            var methCompiler = new MethodCompiler(context, methodInfo);
            methCompiler.CompileMethod(methodNode);
        }
 
        protected override void VisitModuleBody(BslSyntaxNode moduleBody)
        {
            var factory = new BslMethodInfoFactory<BslNativeMethodInfo>(() => new BslNativeMethodInfo());
            var method = factory.NewMethod()
                .Name("$entry")
                .Build();

            _module.Methods.Add(method);
            
            var context = MakeContext();
            var methCompiler = new MethodCompiler(context, method);
            methCompiler.CompileModuleBody(moduleBody);
        }
    }
}