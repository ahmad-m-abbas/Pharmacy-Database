﻿using System.Linq.Expressions;
using AutoMapper;

namespace Infrastructure;

public static class MapperExtensions
{
    public static Expression<Func<TEntity, bool>> ConvertPredicate<TDto, TEntity>(this IMapper mapper,
        Expression<Func<TDto, bool>> predicate)
    {
        return (Expression<Func<TEntity, bool>>)new PredicateVisitor<TDto, TEntity>(mapper).Visit(predicate);
    }

    public static Expression<Func<TDbModel, object>> Convert<TModel, TDbModel>(
        Expression<Func<TModel, object>> sourceMember)
    {
        var memberName = ((MemberExpression)sourceMember.Body).Member.Name;
        var parameter = Expression.Parameter(typeof(TDbModel), "src");
        var property = Expression.Property(parameter, memberName);
        return Expression.Lambda<Func<TDbModel, object>>(property, parameter);
    }


    private class PredicateVisitor<TDto, TEntity> : ExpressionVisitor
    {
        private readonly MemberAssignment[] _bindings;
        private readonly ParameterExpression _entityParameter;

        public PredicateVisitor(IMapper mapper)
        {
            var mockQuery = mapper.ProjectTo<TDto>(new TEntity[0].AsQueryable(), null);
            var lambdaExpression =
                (LambdaExpression)((UnaryExpression)((MethodCallExpression)mockQuery.Expression).Arguments[1]).Operand;

            _bindings = ((MemberInitExpression)lambdaExpression.Body).Bindings.Cast<MemberAssignment>().ToArray();
            _entityParameter = Expression.Parameter(typeof(TEntity));
        }

        // This is required to modify type parameters
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Expression.Lambda(
                base.Visit(node.Body),
                node.Parameters.Select(p => (ParameterExpression)base.Visit(p)).ToArray()
            );
        }

        // Do member mapping
        protected override Expression VisitMember(MemberExpression node)
        {
            var member = node.Member;
            var binding = _bindings.FirstOrDefault(b => b.Member == member);

            if (binding != null) return base.Visit(binding.Expression);

            return base.VisitMember(node);
        }

        // Replace parameters reference
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(TDto)) return _entityParameter;
            if (node.Type == typeof(TEntity)) return _entityParameter;

            return base.VisitParameter(node);
        }
    }
}