/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Reflection;
using OneScript.Language.SyntaxAnalysis;
using ScriptEngine.Compiler;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;

namespace ScriptEngine.Hosting
{
    public static class EngineBuilderExtensions
    {
        public static IEngineBuilder WithEnvironment(this IEngineBuilder b, RuntimeEnvironment env)
        {
            b.Environment = env;
            return b;
        }

        public static IEngineBuilder WithTypes(this IEngineBuilder b, ITypeManager typeManager)
        {
            b.TypeManager = typeManager;
            return b;
        }
        
        public static IEngineBuilder WithGlobals(this IEngineBuilder b, IGlobalsManager globals)
        {
            b.GlobalInstances = globals;
            return b;
        }

        public static IEngineBuilder AddAssembly(this IEngineBuilder b, Assembly asm, Predicate<Type> filter = null)
        {
            var discoverer = new ContextDiscoverer(b.TypeManager, b.GlobalInstances);
            discoverer.DiscoverClasses(asm, filter);
            discoverer.DiscoverGlobalContexts(b.Environment, asm, filter);
            return b;
        }
        
        public static IEngineBuilder AddGlobalContext(this IEngineBuilder b, IAttachableContext context)
        {
            b.Environment.InjectObject(context);
            b.GlobalInstances.RegisterInstance(context);
            return b;
        }

        public static IEngineBuilder UseCompiler(this IEngineBuilder b, ICompilerServiceFactory factory)
        {
            b.CompilerFactory = factory;
            return b;
        }
        
        public static IEngineBuilder UseCompilerOptions(this IEngineBuilder b, Action<CompilerOptions> getOptions)
        {
            EnsureCompilerOptions(b);
            getOptions(b.CompilerOptions);
            
            b.UseCompiler(new AstBasedCompilerFactory(b.CompilerOptions));
            return b;
        }

        private static void EnsureCompilerOptions(IEngineBuilder b)
        {
            if(b.CompilerOptions == default)
                b.CompilerOptions = new CompilerOptions();
        }
        
        public static CompilerOptions UseDirectiveHandler(this CompilerOptions b, IDirectiveHandler handler)
        {
            b.PreprocessorFactory.Add(o => handler);
            return b;
        }
        
        public static CompilerOptions UseDirectiveHandler(
            this CompilerOptions b, 
            Func<ParserOptions, IDirectiveHandler> handlerFactory)
        {
            b.PreprocessorFactory.Add(handlerFactory);
            return b;
        }

        public static CompilerOptions UseConditionalCompilation(this CompilerOptions b)
        {
            return b.UseDirectiveHandler(o => new ConditionalDirectiveHandler(o.ErrorSink));
        }
        
        public static CompilerOptions UseRegions(this CompilerOptions b)
        {
            return b.UseDirectiveHandler(o => new RegionDirectiveHandler(o.ErrorSink));
        }
        
        public static CompilerOptions UseImports(this CompilerOptions opts, IDependencyResolver resolver)
        {
            opts.UseDirectiveHandler(o => new ImportDirectivesHandler(opts.NodeBuilder, opts.ErrorSink));
            opts.DependencyResolver = resolver;

            return opts;
        }
        
        public static IEngineBuilder SetDefaultOptions(this IEngineBuilder builder)
        {
            builder.UseCompilerOptions(o =>
            {
                o.UseConditionalCompilation()
                    .UseRegions();
            });

            return builder;
        }

        public static IEngineBuilder WithDebugger(this IEngineBuilder b, IDebugController debugger)
        {
            b.DebugController = debugger;
            return b;
        }
    }
}