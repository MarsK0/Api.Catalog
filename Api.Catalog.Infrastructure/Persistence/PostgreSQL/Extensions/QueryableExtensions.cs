using Api.Catalog.Application.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL.Extensions;

internal static class QueryableExtensions
{
    public static IQueryable<T> OrderBySort<T>(this IQueryable<T> source, IEnumerable<SortParam> sort)
    {
        if (sort is null || !sort.Any())
            return source;

        var isFirst = true;
        foreach (var s in sort)
        {
            source = source.OrderBySort(s, isFirst);
            isFirst = false;
        }

        return source;
    }
    public static IQueryable<T> OrderBySort<T>(this IQueryable<T> source, SortParam sort, bool isFirst = true)
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

        string methodName;
        if (isFirst)
            methodName = sort.Desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        else
            methodName = sort.Desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);

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
