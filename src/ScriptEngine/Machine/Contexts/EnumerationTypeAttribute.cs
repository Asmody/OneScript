﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;

namespace ScriptEngine
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumerationTypeAttribute : Attribute, INameAndAliasProvider
    {
        public EnumerationTypeAttribute (string name, string alias = null, bool createProperty = true)
		{
			Name = name;
			Alias = alias;
			CreateGlobalProperty = createProperty;
		}

		public string Name { get; }
		public string Alias { get; }
		public bool CreateGlobalProperty { get; }
    }
}
