/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using ScriptEngine.Compiler;
using ScriptEngine.Machine;

namespace ScriptEngine.Hosting
{
    public interface IEngineBuilder
    {
        RuntimeEnvironment Environment { get; set; }
        ITypeManager TypeManager { get; set; }
        IGlobalsManager GlobalInstances { get; set; }
        IDebugController DebugController { get; set; }
        
        ConfigurationProviders ConfigurationProviders { get; }
        
        IServiceDefinitions Services { get; set; }
        Action<ScriptingEngine, IServiceContainer> StartupAction { get; set; }
        
        ScriptingEngine Build();
    }
}