using System.Linq.Expressions;

namespace ProductApi.Core.Shared.Specification;

public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity)
    {
        Func<T, bool> predicate = ToExpression().Compile();
        return predicate(entity);
    }

    public Specification<T> And(Specification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    public Specification<T> Or(Specification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

internal sealed class AndSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = left.ToExpression();
        var rightExpression = right.ToExpression();

        var paramExpr = Expression.Parameter(typeof(T));
        var combined = Expression.AndAlso(
            leftExpression.Body,
            Expression.Invoke(rightExpression, paramExpr));

        return Expression.Lambda<Func<T, bool>>(combined, paramExpr);
    }
}

internal sealed class OrSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = left.ToExpression();
        var rightExpression = right.ToExpression();

        var paramExpr = Expression.Parameter(typeof(T));
        var combined = Expression.OrElse(
            leftExpression.Body,
            Expression.Invoke(rightExpression, paramExpr));

        return Expression.Lambda<Func<T, bool>>(combined, paramExpr);
    }
}

internal sealed class NotSpecification<T>(Specification<T> specification) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var expression = specification.ToExpression();
        var notExpression = Expression.Not(expression.Body);

        return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters);
    }
}
