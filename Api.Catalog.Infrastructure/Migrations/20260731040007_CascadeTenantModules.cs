using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeTenantModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenant_module_tenant_tenant_id",
                schema: "catalog",
                table: "tenant_module");

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_module_tenant_tenant_id",
                schema: "catalog",
                table: "tenant_module",
                column: "tenant_id",
                principalSchema: "catalog",
                principalTable: "tenant",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenant_module_tenant_tenant_id",
                schema: "catalog",
                table: "tenant_module");

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_module_tenant_tenant_id",
                schema: "catalog",
                table: "tenant_module",
                column: "tenant_id",
                principalSchema: "catalog",
                principalTable: "tenant",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
