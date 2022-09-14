using System;
using System.Collections.Generic;
using System.Linq;
using OneScript.Commons;
using OneScript.Compilation.Binding;
using OneScript.Language;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Native.Compiler;
using OneScript.Sources;

namespace OneScript.Dynamic.Tests
{
    internal class CompileHelper
    {
        private IErrorSink _errors = new ListErrorSink();
        private SourceCode _codeIndexer;
        private BslSyntaxNode _module;
        public IEnumerable<CodeError> Errors => _errors.Errors;

        public BslSyntaxNode ParseBatch(string code)
        {
            var parser = GetBslParser(code);

            _module = parser.ParseCodeBatch(true);
            ThrowOnErrors();

            return _module;
        }
            
        public BslSyntaxNode ParseModule(string code)
        {
            var parser = GetBslParser(code);

            _module = parser.ParseStatefulModule();
            ThrowOnErrors();

            return _module;
        }

        private void ThrowOnErrors()
        {
            if (_errors.HasErrors)
            {
                var prefix = Locale.NStr("ru = 'Ошибка комиляции модуля'; en = 'Module compilation error'");
                var text = string.Join('\n',
                    (new[] { prefix }).Concat(_errors.Errors.Select(x => x.ToString(CodeError.ErrorDetails.Simple))));
                throw new Exception(text);
            }
        }

        private DefaultBslParser GetBslParser(string code)
        {
            var lexer = new DefaultLexer();
            lexer.Iterator = SourceCodeBuilder.Create()
                .FromString(code)
                .WithName("<text>")
                .Build()
                .CreateIterator();
            _codeIndexer = lexer.Iterator.Source;

            var parser = new DefaultBslParser(lexer, _errors, new PreprocessorHandlers());
            return parser;
        }

        public DynamicModule Compile(SymbolTable scopes)
        {
            var compiler = new ModuleCompiler(_errors, null);
            return compiler.Compile(_codeIndexer, _module, scopes);
        }
    }
}