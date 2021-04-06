using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OneScript.Native.Compiler
{
    public interface IBlockExpressionGenerator
    {
        void Add(Expression item);
        Expression Block();
    }

    public interface ILoopBlockExpressionGenerator : IBlockExpressionGenerator
    {
        LabelTarget BreakLabel { get; }
        LabelTarget ContinueLabel { get; }
    }

    public static class GeneratorHelper
    {
        public static Expression OneOrBlock(this IList<Expression> list)
        {
            if (list.Count == 1) return list[0];
            return Expression.Block(list);
        }
    }

    public class SimpleBlockExpressionGenerator : IBlockExpressionGenerator
    {
        readonly List<Expression> _statements = new List<Expression>();

        public void Add(Expression item)
        {
            _statements.Add(item);
        }

        public Expression Block()
        {
            return _statements.OneOrBlock();
        }
    }

    public class IfThenBlockGenerator : IBlockExpressionGenerator
    {
        class IfThenElement
        {
            public readonly List<Expression> Condition = new List<Expression>();
            public readonly List<Expression> Body = new List<Expression>();
        }

        private readonly Stack<IfThenElement> _conditionalBlocks = new Stack<IfThenElement>();
        private readonly List<Expression> _elseBlock = new List<Expression>();

        private IfThenElement _currentElement;
        private List<Expression> _statements;
        
        public void Add(Expression item)
        {
            _statements.Add(item);
        }

        public Expression Block()
        {
            var top = _conditionalBlocks.Pop();
            var block = _elseBlock.Count == 0
                ? Expression.IfThen(
                    top.Condition.OneOrBlock(),
                    top.Body.OneOrBlock())
                : Expression.IfThenElse(
                    top.Condition.OneOrBlock(),
                    top.Body.OneOrBlock(),
                    _elseBlock.OneOrBlock());

            while (_conditionalBlocks.Count > 0)
            {
                var next = _conditionalBlocks.Pop();
                block = Expression.IfThenElse(
                    top.Condition.OneOrBlock(), 
                    top.Body.OneOrBlock(), 
                    block);
            }

            return block;
        }

        public void StartCondition()
        {
            _currentElement = new IfThenElement();
            _statements = _currentElement.Condition;
            _conditionalBlocks.Push(_currentElement);
        }

        public void StartBody()
        {
            _statements = _currentElement.Body;
        }

        public void StartElseBody()
        {
            _statements = _elseBlock;
        }
    }
    
    public class WhileBlockExpressionGenerator : ILoopBlockExpressionGenerator
    {
        private readonly List<Expression> _conditionStatements = new List<Expression>();
        private readonly List<Expression> _bodyStatements = new List<Expression>();

        private List<Expression> _statements = null;

        public void Add(Expression item)
        {
            _statements.Add(item);
        }

        public Expression Block()
        {
            var result = new List<Expression>();
            
            result.Add(Expression.IfThen(
                Expression.Not(_conditionStatements.OneOrBlock()), 
                Expression.Break(BreakLabel)));
            result.AddRange(_bodyStatements);

            return Expression.Loop(Expression.Block(result), BreakLabel, ContinueLabel);
        }

        public void StartCondition()
        {
            _statements = _conditionStatements;
        }

        public void StartBody()
        {
            _statements = _bodyStatements;
        }

        public LabelTarget BreakLabel { get; } = Expression.Label(typeof(void));

        public LabelTarget ContinueLabel { get; } = Expression.Label(typeof(void));
    }

    public class ForBlockExpressionGenerator : ILoopBlockExpressionGenerator
    {
        private readonly List<Expression> _bodyStatements = new List<Expression>();

        public Expression IteratorExpression { get; set; }
        public Expression InitialValue { get; set; }
        public Expression UpperLimit { get; set; }

        private readonly LabelTarget _loopLabel = Expression.Label(typeof(void));
       
        public void Add(Expression item)
        {
            _bodyStatements.Add(item);
        }

        public Expression Block()
        {
            var result = new List<Expression>();
            result.Add(Expression.Assign(IteratorExpression, InitialValue)); // TODO: MakeAssign ?
            var finalVar = Expression.Variable(typeof(decimal)); // TODO: BslNumericValue ?
            result.Add(Expression.Assign(finalVar, InitialValue));
            
            var loop = new List<Expression>();
            loop.Add(Expression.IfThen(
                Expression.GreaterThan(IteratorExpression, InitialValue), 
                Expression.Break(BreakLabel)));
            
            loop.AddRange(_bodyStatements);
            
            loop.Add(Expression.Label(ContinueLabel));
            loop.Add(Expression.Increment(IteratorExpression));
            
            result.Add(Expression.Loop(Expression.Block(loop), BreakLabel, ContinueLabel));
            
            result.Add(Expression.Label(BreakLabel));

            return Expression.Block(result);
        }

        public void AddBreakExpression()
        {
            Add(Expression.Break(BreakLabel));
        }

        public void AddContinueExpression()
        {
            Add(Expression.Continue(ContinueLabel));
        }

        public LabelTarget BreakLabel { get; } = Expression.Label(typeof(void));

        public LabelTarget ContinueLabel { get; } = Expression.Label(typeof(void));
    }

    public class ForEachBlockExpressionGenerator : ILoopBlockExpressionGenerator
    {
        private readonly List<Expression> _bodyStatements = new List<Expression>();

        public Expression EnumeratorExpression { get; set; }
        public Expression Iterator { get; set; }

        public void Add(Expression item)
        {
            _bodyStatements.Add(item);
        }

        public Expression Block()
        {
            throw new NotImplementedException();
        }

        public void AddBreakExpression()
        {
            Add(Expression.Break(BreakLabel));
        }

        public void AddContinueExpression()
        {
            Add(Expression.Continue(ContinueLabel));
        }

        public LabelTarget BreakLabel { get; } = Expression.Label(typeof(void));

        public LabelTarget ContinueLabel { get; } = Expression.Label(typeof(void));
    }
}