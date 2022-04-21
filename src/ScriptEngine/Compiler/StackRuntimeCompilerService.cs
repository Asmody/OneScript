/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Collections.Generic;
using OneScript.Language;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Sources;
using ScriptEngine.Machine;

namespace ScriptEngine.Compiler
{
    public class StackRuntimeCompilerService : CompilerServiceBase
    {
        private readonly CompilerOptions _сompilerOptions;

        public StackRuntimeCompilerService(CompilerOptions сompilerOptions, ICompilerContext outerContext) 
            : base(outerContext)
        {
            _сompilerOptions = сompilerOptions;
        }
        
        protected override StackRuntimeModule CompileInternal(SourceCode source, IEnumerable<string> preprocessorConstants, ICompilerContext context)
        {
            var handlers = _сompilerOptions.PreprocessorHandlers;
            var lexer = CreatePreprocessor(source, preprocessorConstants, handlers);
            var moduleNode = ParseSyntaxConstruction(
                lexer,
                handlers,
                source,
                p => p.ParseStatefulModule());

            return BuildModule(context, moduleNode, source);
        }

        protected override StackRuntimeModule CompileBatchInternal(SourceCode source, IEnumerable<string> preprocessorConstants, ICompilerContext context)
        {
            var handlers = _сompilerOptions.PreprocessorHandlers;
            var lexer = CreatePreprocessor(source, preprocessorConstants, handlers);
            var moduleNode = ParseSyntaxConstruction(
                lexer,
                handlers,
                source,
                p => p.ParseCodeBatch());

            return BuildModule(context, moduleNode, source);
        }

        protected override StackRuntimeModule CompileExpressionInternal(SourceCode source, ICompilerContext context)
        {
            var handlers = _сompilerOptions.PreprocessorHandlers;
            
            var lexer = new DefaultLexer
            {
                Iterator = source.CreateIterator()
            };
            
            var moduleNode = ParseSyntaxConstruction(
                lexer,
                handlers,
                source,
                p => p.ParseExpression());

            return BuildModule(context, moduleNode, source);
        }
        
        private ModuleNode ParseSyntaxConstruction(
            ILexer lexer,
            PreprocessorHandlers handlers,
            SourceCode source,
            Func<DefaultBslParser, BslSyntaxNode> action)
        {
            var parser = new DefaultBslParser(
                lexer,
                _сompilerOptions.ErrorSink,
                handlers);

            ModuleNode moduleNode;
            
            try
            {
                moduleNode = (ModuleNode) action(parser);
            }
            catch (SyntaxErrorException e)
            {
                if (e.ModuleName == default)
                {
                    e.ModuleName = source.Name;
                }

                throw;
            }

            return moduleNode;
        }

        private StackRuntimeModule BuildModule(ICompilerContext context, ModuleNode moduleNode, SourceCode src)
        {
            var codeGen = GetCodeGenerator(context);
            codeGen.ThrowErrors = true;
            codeGen.ProduceExtraCode = GetCodeFlags();
            codeGen.DependencyResolver = _сompilerOptions.DependencyResolver;

            return codeGen.CreateModule(moduleNode, src);
        }

        private CodeGenerationFlags GetCodeFlags()
        {
            CodeGenerationFlags cs = CodeGenerationFlags.Always;
            if (GenerateDebugCode)
                cs |= CodeGenerationFlags.DebugCode;

            if (GenerateCodeStat)
                cs |= CodeGenerationFlags.CodeStatistics;

            return cs;
        }

        private PreprocessingLexer CreatePreprocessor(SourceCode source,
            IEnumerable<string> preprocessorConstants,
            PreprocessorHandlers handlers)
        {
            return CreatePreprocessor(source, preprocessorConstants, handlers, _сompilerOptions.ErrorSink);
        }

        protected virtual StackMachineCodeGenerator GetCodeGenerator(ICompilerContext context)
        {
            return new StackMachineCodeGenerator(context);
        }
    }
}