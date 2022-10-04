using System.Diagnostics.CodeAnalysis;

namespace RobotCommando.Models.Expressions;

public sealed class Effect<TContext> : GameExpression<TContext, Action<TContext>>
{
    public void Execute(TContext context) => Expression.Invoke(context);

    [return: NotNullIfNotNull("expressionText")]
    public static implicit operator Effect<TContext>?(string? expressionText) => expressionText is null ? null : new() { ExpressionText = expressionText };
}