using Api.Catalog.Application.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL.Extensions;

internal static class QueryableExtensions
{
    public static IQueryable<T> OrderBySort<T>(this IQueryable<T> source, IEnumerable<SortParam> sort)
    {
        foreach (var s in sort)
            source.OrderBySort(s);

        return source;
    }
    public static IQueryable<T> OrderBySort<T>(this IQueryable<T> source, SortParam sort)
    {
        if (string.IsNullOrWhiteSpace(sort.Field))
            return source;

        var entityType = typeof(T);

        var propertyInfo = entityType.GetProperty(
            sort.Field,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
        );

        if (propertyInfo is null)
            return source;

        var parameter = Expression.Parameter(entityType, "x");
        var propertyAccess = Expression.Property(parameter, propertyInfo);
        var keySelector = Expression.Lambda(propertyAccess, parameter);

        var methodName = sort.Desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { entityType, propertyInfo.PropertyType },
            source.Expression,
            Expression.Quote(keySelector)
        );

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}
