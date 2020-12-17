﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;

namespace OneScript.StandardLibrary.Zip
{
    [EnumerationType("УровеньСжатияZIP", "ZIPCompressionLevel")]
    public enum ZipCompressionLevel
    {
        [EnumItem("Минимальный", "Minimum")]
        Minimal,
        [EnumItem("Оптимальный", "Optimal")]
        Optimal,
        [EnumItem("Максимальный", "Maximum")]
        Maximal
    }
}
