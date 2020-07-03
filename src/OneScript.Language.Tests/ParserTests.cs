/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Linq;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
using Xunit;
using FluentAssertions;

namespace OneScript.Language.Tests
{
    public class ParserTests
    {
        [Fact]
        public void CheckBuild_Of_VariablesSection()
        {
            var code = @"
            Перем П1;
            Перем П2 Экспорт;
            &Аннотация
            Перем П3;
            Перем П4 Экспорт, П5 Экспорт;";
            
            var treeValidator = ParseModuleAndGetValidator(code);

            treeValidator.Is(NodeKind.VariablesSection);

            var child = treeValidator.NextChild();
            child.Is(NodeKind.VariableDefinition)
                .WithNode("Identifier")
                .Equal("П1");

            child = treeValidator.NextChild();
            child.Is(NodeKind.VariableDefinition)
                .WithNode("Identifier")
                .Equal("П2");
            child.HasNode(nameof(NodeKind.ExportFlag));
            
            child = treeValidator.NextChild();
            child.Is(NodeKind.VariableDefinition)
                .WithNode("Annotation")
                .Equal("Аннотация");
            
            child.HasNode("Identifier")
                .Equal("П3");
            
            child = treeValidator.NextChild();
            child.Is(NodeKind.VariableDefinition).WithNode("Identifier").Equal("П4");
            child.HasNode(nameof(NodeKind.ExportFlag));
            
            child = treeValidator.NextChild();
            child.Is(NodeKind.VariableDefinition).WithNode("Identifier").Equal("П5");
            child.HasNode(nameof(NodeKind.ExportFlag));
        }

        [Fact]
        public void CheckBuild_Of_Methods_Section()
        {
            var code = "Процедура А() КонецПроцедуры Функция Б() КонецФункции";
            var node = ParseModuleAndGetValidator(code);

            node.Is(NodeKind.MethodsSection);
            node.CurrentNode.ChildrenList.Should().HaveCount(2, "two methods in code");

            var methodNode = node.NextChild();
            methodNode.Is(NodeKind.Method)
                .NextChildIs(NodeKind.MethodSignature)
                .DownOneLevel()
                    .NextChildIs(NodeKind.Procedure)
                    .NextChildIs(NodeKind.Identifier).ChildItself()
                    .Equal("А");
            
            methodNode = node.NextChild();
            methodNode.Is(NodeKind.Method)
                .NextChildIs(NodeKind.MethodSignature)
                .DownOneLevel()
                .NextChildIs(NodeKind.Function)
                    .HasNode("Identifier")
                    .Equal("Б");
        }

        [Fact]
        public void Check_Annotation_Parameters()
        {
            var code = @"
            &БезПараметров
            &СИменемПараметра(Имя)
            &НесколькоПараметров(Имя, Имя2)
            &Литерал(""Привет"")
            &ИмяИЗначение(А = ""Привет"", М = 1)
            Перем УзелВладелец;";

            var variable = ParseModuleAndGetValidator(code).NextChild();

            var anno = variable.NextChild();
            anno.Is(NodeKind.Annotation)
                .NoMoreChildren();
            anno.Equal("БезПараметров");

            anno = variable.NextChild()
                .Is(NodeKind.Annotation);
            anno.Equal("СИменемПараметра");
            anno.DownOneLevel().Is(NodeKind.AnnotationParameter)
                    .NextChildIs(NodeKind.AnnotationParameterName)
                    .NoMoreChildren();
            anno.NoMoreChildren();

            anno = variable.NextChild().Is(NodeKind.Annotation);
            anno.Equal("НесколькоПараметров");
            anno.HasChildNodes(2);
            anno.NextChild().Is(NodeKind.AnnotationParameter)
                .NextChildIs(NodeKind.AnnotationParameterName)
                .NoMoreChildren();
            
            anno.NextChild().Is(NodeKind.AnnotationParameter)
                .NextChildIs(NodeKind.AnnotationParameterName)
                .NoMoreChildren();

            anno = variable.NextChild();
            anno.Equal("Литерал");
            var param = anno.NextChild().Is(NodeKind.AnnotationParameter);
            anno.NoMoreChildren();
            param.NextChildIs(NodeKind.AnnotationParameterValue).Equals("Привет");
            param.NoMoreChildren();
            
            anno = variable.NextChild();
            anno.Equal("ИмяИЗначение");
            anno.NextChild().Is(NodeKind.AnnotationParameter)
                .NextChildIs(NodeKind.AnnotationParameterName)
                .NextChildIs(NodeKind.AnnotationParameterValue)
                .NoMoreChildren();
            anno.NextChild().Is(NodeKind.AnnotationParameter)
                .NextChildIs(NodeKind.AnnotationParameterName)
                .NextChildIs(NodeKind.AnnotationParameterValue)
                .NoMoreChildren();
                
        }

        [Fact]
        public void Check_Method_Parameters()
        {
            var code = @"
            Процедура П(А, Знач А, Б = 1, Знач Д = -10) Экспорт КонецПроцедуры";

            var proc = ParseModuleAndGetValidator(code).NextChild();

            var signature = proc.NextChild().Is(NodeKind.MethodSignature);
            signature
                .NextChildIs(NodeKind.Procedure)
                .NextChildIs(NodeKind.Identifier)
                .NextChildIs(NodeKind.MethodParameters)
                .NextChildIs(NodeKind.ExportFlag)
                .NoMoreChildren();

            var paramList = signature.HasNode("MethodParameters");
            paramList.NextChild().Is(NodeKind.MethodParameter)
                .NextChildIs(NodeKind.Identifier).ChildItself()
                .Equal("А");
            
            paramList.NextChild().Is(NodeKind.MethodParameter)
                .NextChildIs(NodeKind.ByValModifier)
                .NextChildIs(NodeKind.Identifier)
                .NoMoreChildren();
            
            paramList.NextChild().Is(NodeKind.MethodParameter)
                .NextChildIs(NodeKind.Identifier)
                .NextChildIs(NodeKind.ParameterDefaultValue)
                .ChildItself().Equal("1");
            
            paramList.NextChild().Is(NodeKind.MethodParameter)
                .NextChildIs(NodeKind.ByValModifier)
                .NextChildIs(NodeKind.Identifier)
                .NextChildIs(NodeKind.ParameterDefaultValue)
                .ChildItself().Equal("-10");
        }

        [Fact]
        public void Check_Statement_GlobalFunctionCall()
        {
            var batch = ParseBatchAndGetValidator("Proc();");
            batch.Is(NodeKind.CodeBatch);
            var node = batch.NextChild();
            node.Is(NodeKind.Statement)
                .NextChild().Is(NodeKind.Call)
                .HasNode("Identifier")
                .Equal("Proc");

        }
        
        [Fact]
        public void Check_Statement_ObjectMethod_Call()
        {
            var code = @"Target.Call();
            Target().Call();
            Target[0].Call()";
            var batch = ParseBatchAndGetValidator(code);
            batch.Is(NodeKind.CodeBatch);
            
            var node = batch.NextChild();
            node.Is(NodeKind.Statement)
                .NextChild().Is(NodeKind.DereferenceOperation)
                    .NextChildIs(NodeKind.Identifier)
                    .NextChildIs(NodeKind.Call);
        }
        
        [Fact]
        public void Check_Assignment_OnVariable()
        {
            var code = @"Target = 1";
            
            var batch = ParseBatchAndGetValidator(code);
            batch.Is(NodeKind.CodeBatch);
            
            var node = batch.NextChild();
            node.Is(NodeKind.Statement)
                .NextChild().Is(NodeKind.Assignment)
                .NextChildIs(NodeKind.Identifier)
                .NextChildIs(NodeKind.Constant);
        }
        
        [Fact]
        public void Check_Assignment_OnProperty()
        {
            var code = @"Target.Prop = 1";
            
            var batch = ParseBatchAndGetValidator(code);
            batch.Is(NodeKind.CodeBatch);
            
            var node = batch.NextChild();
            node.Is(NodeKind.Statement)
                .NextChild().Is(NodeKind.Assignment)
                .NextChildIs(NodeKind.DereferenceOperation)
                .NextChildIs(NodeKind.Constant);
        }
        
        [Fact]
        public void Check_Assignment_OnIndex()
        {
            var code = @"Target[0] = 1";
            
            var batch = ParseBatchAndGetValidator(code);
            batch.Is(NodeKind.CodeBatch);
            
            var node = batch.NextChild();
            node.Is(NodeKind.Statement)
                .NextChild().Is(NodeKind.Assignment)
                .NextChildIs(NodeKind.IndexAccess)
                .NextChildIs(NodeKind.Constant);
        }
        
        [Fact]
        public void Check_Assignment_OnComplex_Chain()
        {
            var code = @"Target[0].SomeProp.Method(Object.Prop[3*(8-2)].Data).Prop = ?(Data = True, Object[0], Object.Method()[12]);";
            
            var batch = ParseBatchAndGetValidator(code);
            batch.Is(NodeKind.CodeBatch);
            
            var node = batch.NextChild();
            node.Is(NodeKind.Statement)
                .NextChild().Is(NodeKind.Assignment)
                .NextChildIs(NodeKind.DereferenceOperation)
                .NextChildIs(NodeKind.TernaryOperator);
        }
        
        [Fact]
        public void Check_Binary_And_Unary_Expressions()
        {
            var code = @"А = -2 + 2";
            
            var batch = ParseBatchAndGetValidator(code);
            batch.Is(NodeKind.CodeBatch);

            var assignment = batch
                .NextChild().Is(NodeKind.Statement)
                .NextChild().Is(NodeKind.Assignment);
            assignment.NextChild();
            var node = assignment.NextChild();
            node.Is(NodeKind.BinaryOperation)
                .Equal("+");
            node.NextChild().Is(NodeKind.UnaryOperation)
                .NextChildIs(NodeKind.Constant);
            node.NextChild().Is(NodeKind.Constant);
        }
        
        [Fact]
        public void Check_Logical_Expressions()
        {
            var code = @"Переменная >= 2 ИЛИ Не Переменная < 1";
            
            var expr = ParseExpressionAndGetValidator(code);
            expr.Is(NodeKind.BinaryOperation)
                .Equal("ИЛИ");

            expr.NextChild().Is(NodeKind.BinaryOperation)
                .Equal(">=");
            
            expr.NextChild().Is(NodeKind.UnaryOperation)
                .NextChild().Is(NodeKind.BinaryOperation)
                .Equal("<");
        }
        
        [Fact]
        public void Check_Logical_Priority_Direct()
        {
            var code = @"Переменная1 ИЛИ Переменная2 И Переменная3";

            var expr = ParseExpressionAndGetValidator(code);
            expr.Is(NodeKind.BinaryOperation)
                .Equal("ИЛИ");
        }
        
        [Fact]
        public void Check_Logical_Priority_Parenthesis()
        {
            var code = @"(Переменная1 ИЛИ Переменная2) И Переменная3";
            
            var expr = ParseExpressionAndGetValidator(code);
            expr.Is(NodeKind.BinaryOperation)
                .Equal("И");
        }
        
        private static SyntaxTreeValidator ParseModuleAndGetValidator(string code)
        {
            return MakeValidator(code, p => p.ParseStatefulModule());
        }
        
        private static SyntaxTreeValidator ParseBatchAndGetValidator(string code)
        {
            return MakeValidator(code, p => p.ParseCodeBatch());
        }
        
        private static SyntaxTreeValidator ParseExpressionAndGetValidator(string code)
        {
            return MakeValidator(code, p => p.ParseExpression());
        }

        private static SyntaxTreeValidator MakeValidator(string code, Action<DefaultBslParser> action)
        {
            var lexer = new Lexer();
            lexer.Code = code;

            var client = new DefaultAstBuilder();
            var parser = new DefaultBslParser(client, lexer);
            action(parser);

            parser.Errors.Should().BeEmpty("the valid code is passed");
            var treeValidator = new SyntaxTreeValidator(new TestAstNode(client.RootNode.Children.First()));
            return treeValidator;
        }

    }
}