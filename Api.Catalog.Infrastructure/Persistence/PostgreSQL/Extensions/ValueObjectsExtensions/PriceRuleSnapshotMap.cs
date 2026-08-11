using Api.Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL.Extensions.ValueObjectsExtensions;

internal static class PriceRuleSnapshotMap
{
    public static void OwnsPriceRuleSnapshot<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, PriceRuleSnapshot?>> navigation,
        string columnPrefix = "pricerule_snapshot"
    ) where TEntity : class
    {
        builder.OwnsOne(navigation, snapshot =>
        {
            snapshot.Property(p => p.PriceRuleId).HasColumnName($"{columnPrefix}_id");
            snapshot.Property(p => p.RuleType).HasColumnName($"{columnPrefix}_type");
            snapshot.Property(p => p.Price).HasColumnName($"{columnPrefix}_price");
        });
    }
}
