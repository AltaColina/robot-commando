using System.Diagnostics.CodeAnalysis;

namespace RobotCommando.Models.Expressions;

public sealed class Condition<TContext> : GameExpression<TContext, Func<TContext, bool>>
{
    public bool IsTrue(TContext context) => Expression.Invoke(context);

    [return: NotNullIfNotNull("expressionText")]
    public static implicit operator Condition<TContext>?(string? expressionText) => expressionText is null ? null : new() { ExpressionText = expressionText };
}