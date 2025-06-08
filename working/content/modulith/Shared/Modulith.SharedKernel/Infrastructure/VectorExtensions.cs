using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions;

namespace Modulith.SharedKernel.Infrastructure;

public static class VectorExtensions
{
    public static double CosineDistance(this DbFunctions _, float[] vector1, float[] vector2)
    {
        throw new InvalidOperationException("This method is for use with Entity Framework Core only.");
    }

    public static double EuclideanDistance(this DbFunctions _, float[] vector1, float[] vector2)
    {
        throw new InvalidOperationException("This method is for use with Entity Framework Core only.");
    }

    public static double DotProduct(this DbFunctions _, float[] vector1, float[] vector2)
    {
        throw new InvalidOperationException("This method is for use with Entity Framework Core only.");
    }

    public static IQueryable<T> OrderByCosineDistance<T>(
        this IQueryable<T> query,
        Expression<Func<T, float[]>> vectorSelector,
        float[] queryVector)
    {
        return query.OrderBy(x => EF.Functions.CosineDistance(vectorSelector.Compile()(x), queryVector));
    }

    public static IQueryable<T> OrderByEuclideanDistance<T>(
        this IQueryable<T> query,
        Expression<Func<T, float[]>> vectorSelector,
        float[] queryVector)
    {
        return query.OrderBy(x => EF.Functions.EuclideanDistance(vectorSelector.Compile()(x), queryVector));
    }
} 