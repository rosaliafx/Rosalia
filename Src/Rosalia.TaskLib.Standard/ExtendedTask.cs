namespace Rosalia.TaskLib.Standard
{
    using System;
    using System.Linq.Expressions;
    using Rosalia.Core;
    using Rosalia.Core.Fluent;

    public abstract class ExtendedTask<TContext, TInput, TResult> : AbstractLeafTask<TContext> where TInput : class
    {
        private Func<ExecutionContext<TContext>, TInput> _contextToInput;
        private Action<TResult, TContext> _applyResultToContext;

        protected ExtendedTask() : this(null, null)
        {
        }

        protected ExtendedTask(Func<ExecutionContext<TContext>, TInput> contextToInput) : this(contextToInput, null)
        {
        }

        protected ExtendedTask(Action<TResult, TContext> applyResultToContext) : this(null, applyResultToContext)
        {
        }

        protected ExtendedTask(Func<ExecutionContext<TContext>, TInput> contextToInput, Action<TResult, TContext> applyResultToContext)
        {
            _contextToInput = contextToInput;
            _applyResultToContext = applyResultToContext;
        }

        public ExtendedTask<TContext, TInput, TResult> FillInput(Func<ExecutionContext<TContext>, TInput> fillInputFunc)
        {
            _contextToInput = fillInputFunc;
            return this;
        }

        public ExtendedTask<TContext, TInput, TResult> FillInput(TInput input)
        {
            _contextToInput = context => input;
            return this;
        }

        public virtual ExtendedTask<TContext, TInput, TResult> ApplyResult(Action<TResult, TContext> applyResultFunc)
        {
            _applyResultToContext = applyResultFunc;
            return this;
        }

        protected override void Execute(ResultBuilder resultBuilder, ExecutionContext<TContext> context)
        {
            TInput input = GetInput(context, resultBuilder);
            TResult output = Execute(input, context, resultBuilder);
            if (resultBuilder.IsSuccess)
            {
                ApplyOutputToContext(output, context, resultBuilder);
            }
        }

        protected virtual void ApplyOutputToContext(TResult output, ExecutionContext<TContext> context, ResultBuilder resultBuilder)
        {
            if (_applyResultToContext != null)
            {
                _applyResultToContext(output, context.Data);
            }
        }

        protected abstract TResult Execute(TInput input, ExecutionContext<TContext> context, ResultBuilder resultBuilder);

        protected virtual TInput GetInput(ExecutionContext<TContext> context, ResultBuilder resultBuilder)
        {
            if (_contextToInput == null)
            {
                return null;
            }

            return _contextToInput(context);
        }

        protected TDependency GetRequired<TDependency>(TInput input, Expression<Func<TInput, TDependency>> dependency)
        {
            var dependencyValue = dependency.Compile()(input);
            if (dependencyValue != null)
            {
                return dependencyValue;
            }

            var expression = dependency.Body;

            throw new Exception(string.Format(
                "Required property {0} is not set for input {1}", 
                expression is MemberExpression ? ((MemberExpression)expression).Member.Name : string.Empty,
                input.GetType().Name));
        }
    }
}