﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Text;

namespace ScriptEngine.Environment
{
    public class ScriptSourceFactory
    {
        public ScriptSourceFactory()
        {
            ReaderEncoding = Encoding.UTF8;
        }
        
        public ICodeSource FromString(string code)
        {
            return new StringBasedSource(code);
        }

        public ICodeSource FromFile(string path)
        {
            return new FileBasedSource(path, ReaderEncoding);
        }

        public Encoding ReaderEncoding { get; set; }
    }
}
