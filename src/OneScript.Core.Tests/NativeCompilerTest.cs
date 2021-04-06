﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using FluentAssertions;
using OneScript.StandardLibrary.Collections;
using OneScript.StandardLibrary.Json;
using OneScript.StandardLibrary.Native;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Values;
using ScriptEngine.Types;
using Xunit;

namespace OneScript.Core.Tests
{
    public class NativeCompilerTest
    {
        [Fact]
        public void CanInjectContext_As_Symbols()
        {
            var context = new GlobalJsonFunctions();
            var scope = SymbolScope.FromContext(context);

            scope.Methods.Should().HaveCount(3);
            scope.Methods.IndexOf("ЗаписатьJSON").Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public void Can_Compile_Assignment()
        {
            var blockOfCode = new CompiledBlock(new DefaultTypeManager());
            
            blockOfCode.Parameters.Insert("MyVar", new TypeTypeValue(BasicTypes.Number));
            blockOfCode.CodeBlock = "MyVar = 2";

            var expr = blockOfCode.MakeExpression();
            expr.Body.As<BlockExpression>().Expressions.Should().HaveCount(2); // в конце всегда неявный return
            expr.Body.As<BlockExpression>().Expressions[0].Should().BeAssignableTo<BinaryExpression>();

            expr.Parameters.Should().HaveCount(1);
        }

        [Fact]
        public void Can_Compile_Binary_Expressions()
        {
            var blockOfCode = new CompiledBlock(new DefaultTypeManager());
            
            blockOfCode.Parameters.Insert("MyVar", new TypeTypeValue(BasicTypes.Number));
            blockOfCode.CodeBlock = "MyVar = MyVar + 1";

            var expr = blockOfCode.MakeExpression();

            var body = expr.Body.As<BlockExpression>().Expressions;
            
            body[0].As<BinaryExpression>().NodeType.Should().Be(ExpressionType.Assign);
            body[0].As<BinaryExpression>().Right.Should().BeAssignableTo<BinaryExpression>();
        }
        
        [Fact]
        public void Can_Compile_Unary_Expressions()
        {
            var blockOfCode = new CompiledBlock(new DefaultTypeManager());
            
            blockOfCode.Parameters.Insert("MyVar", new TypeTypeValue(BasicTypes.Number));
            blockOfCode.CodeBlock = "MyVar = -MyVar";

            var expr = blockOfCode.MakeExpression();

            var body = expr.Body.As<BlockExpression>().Expressions;
            
            body[0].As<BinaryExpression>().NodeType.Should().Be(ExpressionType.Assign);
            body[0].As<BinaryExpression>().Right.Should().BeAssignableTo<UnaryExpression>();
        }

        [Fact]
        public void Can_Compile_Empty_Body()
        {
            var blockOfCode = new CompiledBlock(new DefaultTypeManager());
            var func = blockOfCode.CreateDelegate();
            var result = func(default);
            Assert.Equal(ValueFactory.Create(), result);
        }
        
        [Fact]
        public void Can_Compile_AcceptParameters_In_Array()
        {
            var blockOfCode = new CompiledBlock(new DefaultTypeManager());
            blockOfCode.Parameters.Insert("A", new TypeTypeValue(BasicTypes.Number));
            blockOfCode.Parameters.Insert("Б", new TypeTypeValue(BasicTypes.String));
            
            var func = blockOfCode.CreateDelegate();
            var result = func(new []{ ValueFactory.Create(1), ValueFactory.Create("hello") });
            
            Assert.Equal(ValueFactory.Create(), result);
        }
        
        [Fact]
        public void Can_Compile_AcceptParameters()
        {
            var blockOfCode = new CompiledBlock(new DefaultTypeManager());
            blockOfCode.Parameters.Insert("A", new TypeTypeValue(BasicTypes.Number));
            blockOfCode.Parameters.Insert("Б", new TypeTypeValue(BasicTypes.String));
            
            var func = blockOfCode.CreateDelegate<Func<decimal, string, IValue>>();
            var result = func(1, "привет");
            
            Assert.Equal(ValueFactory.Create(), result);
        }

        [Fact]
        public void Number_To_Number_Operations_Are_Available()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.CodeBlock = "А = 1+1*2/3%4";
            var assignment = block.MakeExpression()
                .Body
                .As<BlockExpression>()
                .Expressions
                .First();

            assignment.NodeType.Should().Be(ExpressionType.Assign);
        }
        
        [Fact]
        public void Number_To_IValue_Operations_Are_Available()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.CodeBlock = "А = 1+(А*Б/3)%В";
            
            block.Parameters.Insert("А", new TypeTypeValue(BasicTypes.Number));
            block.Parameters.Insert("Б", new TypeTypeValue(BasicTypes.Number));
            block.Parameters.Insert("В", new TypeTypeValue(BasicTypes.Number));
            
            var assignment = block.MakeExpression()
                .Body
                .As<BlockExpression>()
                .Expressions
                .First();
            
            assignment.NodeType.Should().Be(ExpressionType.Assign);

        }
        
        [Fact]
        public void DateAddition_Is_Availiable()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.CodeBlock = "А = '19840331'+(86400 * 37)";
            
            var assignment = block.MakeExpression()
                .Body
                .As<BlockExpression>()
                .Expressions
                .First();
            
            assignment.NodeType.Should().Be(ExpressionType.Assign);
            var expr = assignment.As<BinaryExpression>();

            expr.Left.Type.Should().Be<DateTime>();
            expr.Right.Type.Should().Be<DateTime>();
        }
        
        [Fact]
        public void DateDiff_Available()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.CodeBlock = "F = (Сегодня - '19840331') / 86400 / 366";
            block.Parameters.Insert("Сегодня", new TypeTypeValue(BasicTypes.Date));
            var assignment = block.MakeExpression()
                .Body
                .As<BlockExpression>()
                .Expressions
                .First();
            
            assignment.NodeType.Should().Be(ExpressionType.Assign);
            var expr = assignment.As<BinaryExpression>();

            expr.Left.Type.Should().Be<decimal>();
            expr.Right.Type.Should().Be<decimal>();
        }
        
        [Fact]
        public void Parameter_Passing_And_Return_Is_Available()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.CodeBlock = "Возврат (Сегодня - '19840331') / 86400 / 366";
            block.Parameters.Insert("Сегодня", new TypeTypeValue(BasicTypes.Date));

            var beaverAge = block.CreateDelegate<Func<DateTime, IValue>>();
            var age = beaverAge(DateTime.Now);

            age.AsNumber().Should().BeGreaterThan(0);
        }
        
        [Fact]
        public void Can_Assign_To_Indexer()
        {
            var tm = new DefaultTypeManager();
            var arrayType = tm.RegisterClass(typeof(ArrayImpl));
            
            var block = new CompiledBlock(tm);
            block.CodeBlock = "Arr[10] = 15";
            block.Parameters.Insert("Arr", new TypeTypeValue(arrayType));

            var expr = block.MakeExpression();
            var statement = expr.Body.As<BlockExpression>().Expressions.First();

            statement.NodeType.Should().Be(ExpressionType.Assign);
            var assign = statement.As<BinaryExpression>();
            assign.Left.NodeType.Should().Be(ExpressionType.Index);

        }
        
        [Fact]
        public void Can_Read_Indexer()
        {
            var tm = new DefaultTypeManager();
            var arrayType = tm.RegisterClass(typeof(ArrayImpl));
            
            var block = new CompiledBlock(tm);
            block.CodeBlock = "А = Arr[10]";
            block.Parameters.Insert("Arr", new TypeTypeValue(arrayType));

            var expr = block.MakeExpression();
            var statement = expr.Body.As<BlockExpression>().Expressions.First();

            statement.NodeType.Should().Be(ExpressionType.Assign);
            var assign = statement.As<BinaryExpression>();
            assign.Right.NodeType.Should().Be(ExpressionType.Index);
        }

        [Fact]
        public void Can_Do_While()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.Parameters.Insert("Результат", new TypeTypeValue(BasicTypes.Number));
            block.CodeBlock = "Ф = 1;" +
                              "Пока Ф < 10 Цикл" +
                              "\tРезультат = Результат + Ф;" +
                              "\tФ = Ф + 1;" +
                              "\tЕсли Ф > 2 Тогда Прервать; КонецЕсли;" +
                              "КонецЦикла;";
            var func = block.MakeExpression().Compile();
            
            var args = new object[] { decimal.One };
            func.DynamicInvoke(args);
            args[0].Should().Be(4);
        }
        
        [Fact]
        public void Can_Do_IfThen()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.CodeBlock = "Если Истина Тогда Ф=1; КонецЕсли";
            var loop = block.MakeExpression()
                .Body
                .As<BlockExpression>()
                .Expressions
                .First();
            loop.NodeType.Should().Be(ExpressionType.Conditional);
        }
        
        [Fact]
        public void Can_Do_IfThenElse()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.CodeBlock = "Если Истина Тогда Ф=1; Иначе Ф=2; КонецЕсли";
            var loop = block.MakeExpression()
                .Body
                .As<BlockExpression>()
                .Expressions
                .First();
            loop.NodeType.Should().Be(ExpressionType.Conditional);
        }
        
        [Fact]
        public void Can_Do_ElseIf()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.Parameters.Insert("П", new TypeTypeValue(BasicTypes.Number));
            block.Parameters.Insert("Ф", new TypeTypeValue(BasicTypes.Number));
            block.CodeBlock = 
                "Если П=1 Тогда Ф=1;" +
                "ИначеЕсли П=2 Тогда Ф=2;" +
                "ИначеЕсли П=3 Тогда Ф=3;" +
                "Иначе Ф=0; КонецЕсли";
            var expression = block.MakeExpression(); 
            var condition = expression 
                .Body
                .As<BlockExpression>()
                .Expressions
                .First();
            condition.NodeType.Should().Be(ExpressionType.Conditional);
            var func = expression.Compile();

            for (decimal i = 0; i < 4; i++)
            {
                var args = new object[] {i, (decimal)0};
                func.DynamicInvoke(args);
                args[1].Should().Be(i);
            }
        }
        
        [Fact]
        public void Can_ForLoop()
        {
            var block = new CompiledBlock(new DefaultTypeManager());
            block.Parameters.Insert("Результат", new TypeTypeValue(BasicTypes.Number));
            block.CodeBlock = 
                "Для Ф = 1 По 2+2*2 Цикл " +
                "Результат = Результат + Ф;" +
                "Если Ф > 2 Тогда Прервать; КонецЕсли; " +
                "Продолжить;" +
                "КонецЦикла;";
            var expression = block.MakeExpression();
            var func = expression.Compile();
            var args = new object[] { decimal.Zero };
            func.DynamicInvoke(args);
            args[0].Should().Be(6);
        }
    }
}