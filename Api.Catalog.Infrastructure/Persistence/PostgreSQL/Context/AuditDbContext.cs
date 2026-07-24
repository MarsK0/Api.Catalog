using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;
/// <summary>
/// Contexto dedicado à escrita de logs de falha de auditoria via conexão isolada,
/// fora da transação da operação de negócio.
/// ATENÇÃO: NÃO gerar nem aplicar migrations a partir deste contexto.
/// AppDbContext é o dono exclusivo do schema/migrations de AuditLog.
/// </summary>
internal sealed class AuditDbContext(
    DbContextOptions<AuditDbContext> options
) : DbContext(options)
{
    internal DbSet<AuditLog> AuditLogs { get; init; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuditLogMap());
    }
}
