using Api.Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL.Extensions.ValueObjectsExtensions;

internal static class ProductSnapshotMap
{
    public static void OwnsProductSnapshot<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, ProductSnapshot?>> navigation,
        string columnPrefix = "product_snapshot"
    ) where TEntity : class
    {
        builder.OwnsOne(navigation, snapshot =>
        {
            snapshot.Property(p => p.Id).HasColumnName($"{columnPrefix}_id");
            snapshot.Property(p => p.Description).HasColumnName($"{columnPrefix}_description").HasMaxLength(60);
            snapshot.Property(p => p.Reference).HasColumnName($"{columnPrefix}_reference").HasMaxLength(30);
        });
    }
}
