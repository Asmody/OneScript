﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Runtime.CompilerServices;
using System;
using System.Reflection;
using ScriptEngine.Machine;
using MethodInfo = System.Reflection.MethodInfo;

[assembly: InternalsVisibleTo("OneScript.Core.Tests")]

namespace OneScript.StandardLibrary.Native
{
    internal class RuntimeSymbol
    {
        public IRuntimeContextInstance Target;
        
        public string Name { get; set; }
        
        public string Alias { get; set; }
        
        public MemberInfo MemberInfo { get; set; }
    }
    
    internal class VariableSymbol : RuntimeSymbol
    {
        public virtual Type VariableType { get; set; }
    }
    
    internal class PropertySymbol : VariableSymbol
    {
        public PropertyInfo PropertyInfo => (PropertyInfo)MemberInfo;

        public override Type VariableType { get => PropertyInfo.PropertyType; set => throw new NotSupportedException(); }
    }
    
    internal class MethodSymbol : RuntimeSymbol
    {
        public MethodInfo MethodInfo => (MethodInfo)MemberInfo;
    }
}