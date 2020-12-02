/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;

namespace OneScript.Language.SyntaxAnalysis
{
    public class ImportDirectivesHandler : IDirectiveHandler
    {
        private readonly ILexer _importClauseLexer;
        private bool _enabled;

        public ImportDirectivesHandler()
        {
            var builder = new LexerBuilder();
            builder.Detect((cs, i) => !char.IsWhiteSpace(cs))
                .HandleWith(new NonWhitespaceLexerState());

            _importClauseLexer = builder.Build();
        }
        
        public void OnModuleEnter(ParserContext context)
        {
            _enabled = true;
        }

        public void OnModuleLeave(ParserContext context)
        {
            _enabled = false;
        }

        public bool HandleDirective(ParserContext context)
        {
            if(!_enabled)
                throw new SyntaxErrorException(
                    context.Lexer.GetErrorPosition(),
                    LocalizedErrors.DirectiveNotSupported(context.LastExtractedLexem.Content)
                    );
                
            var lastExtractedLexem = context.LastExtractedLexem;
            var lexemStream = context.Lexer;
            var nodeBuilder = context.NodeBuilder;
            if (!DirectiveSupported(lastExtractedLexem.Content))
            {
                return default;
            }
            
            var node = nodeBuilder.CreateNode(NodeKind.Preprocessor, lastExtractedLexem);
            _importClauseLexer.Iterator = lexemStream.Iterator;
            
            var lex = _importClauseLexer.NextLexem();
            if (lex.Type == LexemType.EndOfText)
            {
                throw new SyntaxErrorException(lexemStream.GetErrorPosition(),
                    "Ожидается имя библиотеки");
            }

            var argumentNode = nodeBuilder.CreateNode(NodeKind.Unknown, lex);
            nodeBuilder.AddChild(node, argumentNode);
            nodeBuilder.AddChild(context.NodeContext.Peek(), node);

            lex = _importClauseLexer.NextLexemOnSameLine();
            if (lex.Type != LexemType.EndOfText)
            {
                throw new SyntaxErrorException(lexemStream.GetErrorPosition(),
                    LocalizedErrors.UnexpectedOperation());
            }

            context.LastExtractedLexem = lexemStream.NextLexem(); 

            return true;
        }
        
        private bool DirectiveSupported(string directive)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(directive, "использовать") == 0
                   || StringComparer.InvariantCultureIgnoreCase.Compare(directive, "use") == 0;
        }
    }
}