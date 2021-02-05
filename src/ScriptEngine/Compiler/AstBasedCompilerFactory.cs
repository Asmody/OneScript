/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.Hosting;

namespace ScriptEngine.Compiler
{
    public class AstBasedCompilerFactory : ICompilerServiceFactory
    {
        private readonly CompilerOptions _compilerOptions;

        public AstBasedCompilerFactory(IServiceContainer services)
        {
            _compilerOptions = services.Resolve<CompilerOptions>();
        }

        public ICompilerService CreateInstance(ICompilerContext context)
        {
            return new AstBasedCompilerService(_compilerOptions, context);
        }
    }
}