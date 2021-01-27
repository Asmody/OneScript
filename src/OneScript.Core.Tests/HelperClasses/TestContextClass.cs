﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Types;

namespace OneScript.Core.Tests
{
	[ContextClass("ТестовыйКласс", "TestClass")]
	public class TestContextClass : AutoContext<TestContextClass>
	{
		
		public string CreatedViaMethod { get; private set; }
		
		[ContextMethod("УстаревшийМетод", "ObsoleteMethod", IsDeprecated = true, ThrowOnUse = false)]
		public void ObsoleteMethod()
		{
			// Do nothing
		}

		[ContextMethod("ХорошийМетод", "GoodMethod")]
		[ContextMethod("ObsoleteAlias", IsDeprecated = true, ThrowOnUse = false)]
		[ContextMethod("VeryObsoleteAlias", IsDeprecated = true, ThrowOnUse = true)]
		public void GoodMethod()
		{
			// Do nothing
		}
		
		[ScriptConstructor]
		public static TestContextClass Constructor()
		{
			return new TestContextClass
			{
				CreatedViaMethod = "Constructor0"
			};
		}
		
		[ScriptConstructor(ParametrizeWithClassName = true)]
		public static TestContextClass Constructor(string typeName, IValue ctorParam)
		{
			return new TestContextClass
			{
				CreatedViaMethod = $"Constructor1-{typeName}"
			};
		}
		
		[ScriptConstructor]
		public static TestContextClass Constructor(TypeActivationContext context, IValue ctorParam1, IValue ctorParam2)
		{
			return new TestContextClass
			{
				CreatedViaMethod = $"Constructor2-{context.TypeName}"
			};
		}
	}
}