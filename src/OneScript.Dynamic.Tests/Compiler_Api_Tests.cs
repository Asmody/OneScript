/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Collections.Generic;
using FluentAssertions;
using OneScript.Compilation.Binding;
using OneScript.Language;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Native.Compiler;
using Xunit;

namespace OneScript.Dynamic.Tests
{
    public class Compiler_Api_Tests
    {
        [Fact]
        public void CanCompile_Empty_Module()
        {
            var module = CreateModule("");
            Assert.Empty(module.Fields);
            Assert.Empty(module.Methods);
            module.ModuleBody.Should().BeNull();
        }

        [Fact]
        public void CanCompile_ModuleBody()
        {
            var module = CreateModule("А = 1");
            module.Methods.Should().HaveCount(1);
        }

        [Fact]
        public void CanCompile_Module_With_SeveralMethods()
        {
            var module = CreateModule(
                @"Процедура А()
                    Б = 1;
                КонецПроцедуры

                Процедура Б()
                    Б = 1;
                КонецПроцедуры

                В = 4;
                ");

            module.Methods.Should().HaveCount(3);
            module.ModuleBody.Should().NotBeNull();
        }
        
        [Fact]
        public void CanCompile_Module_With_AllSections()
        {
            var module = CreateModule(
                @"Перем Ы;

                Процедура А()
                    Б = 1;
                КонецПроцедуры

                Процедура Б()
                    Б = 1;
                КонецПроцедуры

                В = 4;
                ");
            
            module.Methods.Should().HaveCount(3);
            module.ModuleBody.Should().NotBeNull();
            module.Fields.Should().HaveCount(1);
        }
        
        [Fact]
        public void Detects_DuplicateVars_InModule()
        {
            List<CodeError> errors = new List<CodeError>();
            CreateModule(
                @"Перем Ы;
                Перем О;
                Перем Ы;
                ", errors);

            errors.Should().HaveCount(1);
            errors[0].ErrorId.Should().Be(nameof(LocalizedErrors.DuplicateVarDefinition));
        }
        
        [Fact]
        public void Detects_DuplicateMethods_InModule()
        {
            List<CodeError> errors = new List<CodeError>();
            CreateModule(
                @"Процедура А()
                    Б = 1;
                КонецПроцедуры

                Процедура А()
                    Б = 1;
                КонецПроцедуры
                ", errors);

            errors.Should().HaveCount(1);
            errors[0].ErrorId.Should().Be(nameof(LocalizedErrors.DuplicateMethodDefinition));
        }

        private DynamicModule CreateModule(string code)
        {
            var helper = new CompileHelper();
            helper.ParseModule(code);
            return helper.Compile(new SymbolTable());
        }
        
        private DynamicModule CreateModule(string code, List<CodeError> errors)
        {
            var helper = new CompileHelper();
            helper.ParseModule(code);
            var result = helper.Compile(new SymbolTable());
            errors.AddRange(helper.Errors);
            return result;
        }
        
        private DynamicModule CreateModule(string code, SymbolTable scopes)
        {
            var helper = new CompileHelper();
            helper.ParseBatch(code);
            return helper.Compile(scopes);
        }
    }
}