/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Native.Runtime;
using OneScript.Values;

namespace OneScript.Native.Compiler
{
    internal class BinaryOperationCompiler
    {
        private ExpressionType _opCode;

        public Expression Compile(BinaryOperationNode node, Expression left, Expression right)
        {
            _opCode = ExpressionHelpers.TokenToOperationCode(node.Operation);
            
            if (IsValue(left.Type))
            {
                return CompileDynamicOperation(left, right);
            }
            else
            {
                return CompileStaticOperation(left, right);
            }
        }

        private Expression CompileStaticOperation(Expression left, Expression right)
        {
            if (_opCode == ExpressionType.Equal || _opCode == ExpressionType.NotEqual)
            {
                return MakeStaticEqualityOperation(left, right);
            }
            
            if (IsNumeric(left.Type))
            {
                return MakeNumericOperation(left, right);
            }
            
            if (left.Type == typeof(DateTime))
            {
                return DateOperation(left, right);
            }
            
            if (left.Type == typeof(string) || left.Type == typeof(BslStringValue))
            {
                if (_opCode == ExpressionType.Add)
                {
                    return StringAddition(left, right);
                }

                if (IsComparisonOperation(_opCode))
                {
                    return MakeDynamicComparison(
                        ExpressionHelpers.ConvertToBslValue(left),
                        ExpressionHelpers.ConvertToBslValue(right));
                }
                
                throw new NativeCompilerException($"Operator {_opCode} is not defined for strings");
            }
            
            if (left.Type == typeof(bool))
            {
                return MakeLogicalOperation(left, right);
            }
            
            throw new NativeCompilerException($"Operation {_opCode} is not defined for {left.Type} and {right.Type}");
        }

        private Expression MakeStaticEqualityOperation(Expression left, Expression right)
        {
            if (right.Type.IsValue())
                return MakeDynamicEquality(ExpressionHelpers.ConvertToBslValue(left), right);
            
            return Expression.MakeBinary(_opCode, left, right);
        }

        private Expression MakeNumericOperation(Expression left, Expression right)
        {
            if (IsNumeric(right.Type))
            {
                return AdjustArithmeticOperandTypesAndMakeBinary(left, right);
            }
            if(IsValue(right.Type))
            {
                right = ExpressionHelpers.ToNumber(right);
                return AdjustArithmeticOperandTypesAndMakeBinary(left, right);
            }

            throw new NativeCompilerException($"Operation {_opCode} is not defined for Number and {right.Type}");
        }

        private Expression AdjustArithmeticOperandTypesAndMakeBinary(Expression left, Expression right)
        {
            if (left.Type == right.Type)
                return Expression.MakeBinary(_opCode, left, right);
            
            // try find direct operator
            var method = GetUserDefinedBinaryOperator("op_" + _opCode.ToString(), left.Type, right.Type);
            if (method == null)
            {
                // try convert
                if (left.Type.IsInteger() && !right.Type.IsInteger())
                {
                    // нужна нецелочисленная операция
                    left = Expression.Convert(left, typeof(decimal));
                    return AdjustArithmeticOperandTypesAndMakeBinary(left, right);
                }

                right = Expression.Convert(right, left.Type);
            }

            return Expression.MakeBinary(_opCode, left, right, false, method);
        }
        
        private static MethodInfo GetUserDefinedBinaryOperator(string name, Type leftType, Type rightType) {
 
            Type[] types = { leftType, rightType };
            
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo method = leftType.GetMethod(name, flags, null, types, null);
            if (method == null)
            {
                method = rightType.GetMethod(name, flags, null, types, null);
            }
            
            return method;
        }
        
        private Expression DateOperation(Expression left, Expression right)
        {
            if (IsNumeric(right.Type))
            {
                return DateOffsetOperation(left, right);
            }

            if (_opCode == ExpressionType.Subtract && right.Type == typeof(DateTime))
            {
                return DateDiffExpression(left, right);
            }
            else if (IsComparisonOperation(_opCode))
            {
                var bslDateLeft = ExpressionHelpers.ToDate(left);
                return Expression.MakeBinary(_opCode, bslDateLeft, right);
            }
            else if (IsValue(right.Type))
            {
                var isDate = Expression.TypeIs(right, typeof(BslDateValue));
                
                if(_opCode == ExpressionType.Subtract)
                    return Expression.Condition(isDate, DateDiffExpression(left, Expression.Convert(right, typeof(DateTime))),
                        DateOffsetOperation(left, ExpressionHelpers.ToNumber(right)));
                else
                    return DateOffsetOperation(left, ExpressionHelpers.ToNumber(right));

            }
            
            throw new NativeCompilerException($"Operation {_opCode} is not defined for dates");
        }

        private static bool IsComparisonOperation(ExpressionType opCode)
        {
            return opCode == ExpressionType.LessThan ||
                   opCode == ExpressionType.LessThanOrEqual ||
                   opCode == ExpressionType.GreaterThan ||
                   opCode == ExpressionType.GreaterThanOrEqual;
        }

        private static Expression DateDiffExpression(Expression left, Expression right)
        {
            var spanExpr = Expression.Subtract(left, ExpressionHelpers.ToDate(right));
            var totalSeconds = Expression.Property(spanExpr, nameof(TimeSpan.TotalSeconds));
            var decimalSeconds = Expression.Convert(totalSeconds, typeof(decimal));

            return decimalSeconds;
        }
        
        private Expression DateOffsetOperation(Expression left, Expression right)
        {
            var adder = typeof(DateTime).GetMethod(nameof(DateTime.AddSeconds));
            Debug.Assert(adder != null);
            
            var toDouble = Expression.Convert(right, typeof(double));
            Expression arg;
            if (_opCode == ExpressionType.Add)
                arg = toDouble;
            else if (_opCode == ExpressionType.Subtract)
                arg = Expression.Negate(toDouble);
            else
            {
                throw new NativeCompilerException($"Operation {_opCode} is not defined for dates");
            }

            return Expression.Call(left, adder, arg);
        }

        private Expression MakeLogicalOperation(Expression left, Expression right)
        {
            if (IsValue(right.Type))
            {
                return Expression.MakeBinary(_opCode, left, ExpressionHelpers.ToBoolean(right));
            }
            else
            {
                return Expression.MakeBinary(_opCode, left, Expression.Convert(right, typeof(bool)));
            }
        }
        
        private Expression StringAddition(Expression left, Expression right)
        {
            if (left.Type == typeof(BslStringValue))
            {
                return Expression.Add(left, ExpressionHelpers.ToString(right));
            }
            
            var concatMethod = typeof(string).GetMethod(
                nameof(string.Concat),
                new[] { typeof(string), typeof(string) });

            Debug.Assert(concatMethod != null);
            
            if (right.Type == typeof(string))
            {
                return Expression.Call(null, concatMethod, left, right);
            }

            return Expression.Call(null, concatMethod, left, ExpressionHelpers.ToString(right));
        }
        
        private Expression CompileDynamicOperation(Expression left, Expression right)
        {
            Debug.Assert(left.Type.IsValue());
            
            switch (_opCode)
            {
                case ExpressionType.Add:
                    return MakeDynamicAddition(left, right);
                case ExpressionType.Subtract:
                    return MakeDynamicSubtraction(left, right);
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return MakeDynamicComparison(left, right); 
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return MakeDynamicEquality(left, right);
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                    return MakeNumericOperation(
                        ExpressionHelpers.ToNumber(left),
                        ExpressionHelpers.ToNumber(right));
                default:
                    throw new NativeCompilerException($"Operation {_opCode} is not defined for IValues");
            }
        }

        private Expression MakeDynamicEquality(Expression left, Expression right)
        {
            Debug.Assert(left.Type.IsValue());

            var result = ExpressionHelpers.CallEquals(left, right);
            if (_opCode == ExpressionType.NotEqual)
                result = Expression.Not(result);

            return result;
        }

        private Expression MakeDynamicComparison(Expression left, Expression right)
        {
            Debug.Assert(left.Type.IsValue());

            var compareCall = ExpressionHelpers.CallCompareTo(left, right);
            return Expression.MakeBinary(_opCode, compareCall, Expression.Constant(0));
        }

        private Expression MakeDynamicSubtraction(Expression left, Expression right)
        {
            return ExpressionHelpers.Subtract(left, right);
        }

        private UnaryExpression CreateConversionException(Expression typeOfLeft)
        {
            var exceptionConstructor = typeof(NativeCompilerException).GetConstructor(new[] {typeof(string)});
            Debug.Assert(exceptionConstructor != null);

            var message = Expression.Add(
                Expression.Constant($"Operation {_opCode} is not defined for type "),
                typeOfLeft);

            var createException = Expression.New(exceptionConstructor, message);
            var throwException = Expression.Throw(createException);
            return throwException;
        }

        private Expression MakeDynamicAddition(Expression left, Expression right)
        {
            return ExpressionHelpers.Add(left, right);
        }

        private static bool IsNumeric(Type type) => type.IsNumeric();
        
        private static bool IsValue(Type type) => type.IsValue();
    }
}