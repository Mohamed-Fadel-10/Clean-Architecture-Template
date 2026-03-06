using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Persistence.Contexts
{
    public static class EntityFrameworkExtensions
    {
        /// <summary>
        /// Applies the specified query filter to all entities that are assignable from the provided type.
        /// </summary>
        /// <typeparam name="T">The type to which the query filter should be applied.</typeparam>
        /// <param name="modelBuilder">The ModelBuilder instance.</param>
        /// <param name="expression">The filter expression to apply.</param>
        public static void ApplyQueryFilterToAssignableEntities<T>(this ModelBuilder modelBuilder,
            Expression<Func<T, bool>> expression)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(T).IsAssignableFrom(entityType.ClrType))
                    continue;
                //if (IsDerivedFromBaseType(entityType.ClrType, typeof(Course)))
                //    continue;
                var parameterType = Expression.Parameter(entityType.ClrType);
                var expressionFilter = ReplacingExpressionVisitor.Replace(
                    expression.Parameters.Single(), parameterType, expression.Body);

                var currentQueryFilter = entityType.GetQueryFilter();
                if (currentQueryFilter != null)
                {
                    var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
                        currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
                    expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
                }

                var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);
                entityType.SetQueryFilter(lambdaExpression);
            }
        }
        private static bool IsDerivedFromBaseType(Type entityType, Type baseType)
        {
            // Traverse through the inheritance hierarchy to check if baseType is a direct base type
            while (entityType != null && entityType != typeof(object))
            {
                if (entityType.BaseType == baseType)
                    return true;

                entityType = entityType.BaseType;
            }

            return false;
        }
    }
}
