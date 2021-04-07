/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq.Expressions;

namespace OneScript.Native.Compiler
{
    public class StatementBlocksWriter
    {
        private readonly Stack<StatementsBlockRecord> _blocks = new Stack<StatementsBlockRecord>();
        
        public void EnterBlock(JumpInformationRecord newJumpStates)
        {
            var current = GetCurrentBlock();
            newJumpStates.MethodReturn ??= current.MethodReturn;
            newJumpStates.LoopBreak ??= current.LoopBreak;
            newJumpStates.LoopContinue ??= current.LoopContinue;

            var block = new StatementsBlockRecord(newJumpStates);
            _blocks.Push(block);
        }

        public StatementsBlockRecord LeaveBlock() => _blocks.Pop();

        public StatementsBlockRecord GetCurrentBlock() => _blocks.Peek();

        public void Add(Expression statement) => GetCurrentBlock().Add(statement);
    }
}