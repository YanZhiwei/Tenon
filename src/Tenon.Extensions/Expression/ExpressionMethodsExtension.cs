using System;
using System.Linq.Expressions;
using System.Reflection;
using SystemExpressions = System.Linq.Expressions.Expression;
namespace Tenon.Extensions.Expression;

public static class ExpressionMethodsExtension
{
    public static string GetMemberName<TEntity, TMember>(this Expression<Func<TEntity, TMember>> memberExpression)
    {
        return GetMemberInfo(memberExpression)?.Name;
    }

    public static MemberInfo GetMemberInfo<TEntity, TMember>(this Expression<Func<TEntity, TMember>> expression)
    {
        if (expression.NodeType != ExpressionType.Lambda)
            throw new ArgumentException(nameof(expression));

        var lambda = (LambdaExpression) expression;

        var memberExpression = ExtractMemberExpression(lambda.Body);
        if (memberExpression == null)
            throw new ArgumentException(nameof(expression));

        return memberExpression.Member;
    }

    private static MemberExpression ExtractMemberExpression(SystemExpressions expression)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.MemberAccess:
                return (MemberExpression) expression;

            case ExpressionType.Convert:
                var operand = ((UnaryExpression) expression).Operand;
                return ExtractMemberExpression(operand);

            default:
                return null;
        }
    }
}