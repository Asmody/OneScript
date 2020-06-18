/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

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
            
            var treeValidator = ParseAndGetValidator(code);

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
            var node = ParseAndGetValidator(code);

            node.Is(NodeKind.MethodsSection);
            node.CurrentNode.Children.Should().HaveCount(2, "two methods in code");

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

            var variable = ParseAndGetValidator(code).NextChild();

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
        
        private static SyntaxTreeValidator ParseAndGetValidator(string code)
        {
            var lexer = new Lexer();
            lexer.Code = code;

            var client = new TestParserClient();
            var parser = new DefaultBslParser(client, lexer);
            parser.ParseStatefulModule();

            parser.Errors.Should().BeEmpty("the valid code is passed");
            var treeValidator = new SyntaxTreeValidator(client.RootNode.Children[0]);
            return treeValidator;
        }

    }
}