﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using OneScript.Commons;
using OneScript.Language;

namespace ScriptEngine.Compiler
{
    [Obsolete]
    public class CompilerException : ScriptException
    {
        public CompilerException(string msg)
            : base(new ErrorPositionInfo(), msg)
        {

        }

        public static CompilerException FromCodeError(CodeError error)
        {
            var exc = new CompilerException(Locale.NStr(error.Description));
            if (error.Position != default)
                AppendCodeInfo(exc, error.Position);

            return exc;
        }
        
        public static void AppendCodeInfo(CompilerException exc, ErrorPositionInfo errorPosInfo)
        {
            exc.LineNumber = errorPosInfo.LineNumber;
            exc.ColumnNumber = errorPosInfo.ColumnNumber;
            exc.Code = errorPosInfo.Code;
            exc.ModuleName = errorPosInfo.ModuleName;
        }
    }
}
