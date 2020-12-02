/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;

namespace OneScript.Language.SyntaxAnalysis
{
    public class PreprocessorHandlers : IDirectiveHandler
    {
        private readonly IList<IDirectiveHandler> _handlers = new List<IDirectiveHandler>();
        
        public void Add(IDirectiveHandler handler)
        {
            _handlers.Add(handler);
        }
        
        public void Remove(IDirectiveHandler handler)
        {
            _handlers.Remove(handler);
        }
        
        public IDirectiveHandler Get(Type type)
        {
            return _handlers.FirstOrDefault(type.IsInstanceOfType);
        }

        public T Get<T>() where T : IDirectiveHandler
        {
            return (T)Get(typeof(T));
        }

        void IDirectiveHandler.OnModuleEnter(ParserContext context)
        {
            foreach (var handler in _handlers)
            {
                handler.OnModuleEnter(context);
            }
        }

        void IDirectiveHandler.OnModuleLeave(ParserContext context)
        {
            foreach (var handler in _handlers)
            {
                handler.OnModuleLeave(context);
            }
        }

        bool IDirectiveHandler.HandleDirective(ParserContext context)
        {
            return _handlers.Any(handler => handler.HandleDirective(context));
        }
    }
}