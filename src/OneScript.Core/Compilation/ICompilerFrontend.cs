using System;
using System.Collections.Generic;
using OneScript.Compilation.Binding;
using OneScript.Execution;
using OneScript.Language;
using OneScript.Sources;

namespace OneScript.Compilation
{
    public interface ICompilerFrontend
    {
        bool GenerateDebugCode { get; set; }
        
        bool GenerateCodeStat { get; set; }
        
        IList<string> PreprocessorDefinitions { get; }
        
        SymbolTable Symbols { get; set; }
        
        ICompileTimeSymbolsProvider ModuleSymbols { get; set; }
        
        IErrorSink ErrorSink { get; }
        
        IExecutableModule Compile(SourceCode source, Type classType = null);
        
        IExecutableModule CompileExpression(SourceCode source);
        
        IExecutableModule CompileBatch(SourceCode source);
    }
}