using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "person",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    email = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "platform_role",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    slug = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "account",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "catalog",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "platform_membership",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_membership", x => x.id);
                    table.ForeignKey(
                        name: "FK_platform_membership_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "catalog",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    family_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    remember_me = table.Column<bool>(type: "boolean", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    revoked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_token_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "catalog",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "person_platform_roles",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    platform_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_platform_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_person_platform_roles_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "catalog",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_person_platform_roles_platform_role_platform_role_id",
                        column: x => x.platform_role_id,
                        principalSchema: "catalog",
                        principalTable: "platform_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "platform_role_permission",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    resource = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    platform_role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_role_permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_platform_role_permission_platform_role_platform_role_id",
                        column: x => x.platform_role_id,
                        principalSchema: "catalog",
                        principalTable: "platform_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_log",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "text", nullable: true),
                    success = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    ocurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_log_person_user_id",
                        column: x => x.user_id,
                        principalSchema: "catalog",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_audit_log_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "budget",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    valid_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_email = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget", x => x.id);
                    table.ForeignKey(
                        name: "FK_budget_person_user_id",
                        column: x => x.user_id,
                        principalSchema: "catalog",
                        principalTable: "person",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_budget_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "category",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.id);
                    table.ForeignKey(
                        name: "FK_category_category_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "catalog",
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_category_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "media",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    extension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media", x => x.id);
                    table.ForeignKey(
                        name: "FK_media_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pricing_list",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    valid_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricing_list", x => x.id);
                    table.ForeignKey(
                        name: "FK_pricing_list_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    reference = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_membership",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_membership", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_membership_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "catalog",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_membership_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_module",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_code = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_module", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_module_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_role",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_role", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_role_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "budget_item",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    budget_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_description = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    product_reference = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    price_rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_rule_type = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_budget_item_budget_budget_id",
                        column: x => x.budget_id,
                        principalSchema: "catalog",
                        principalTable: "budget",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_budget_item_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "assets",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assets", x => x.id);
                    table.ForeignKey(
                        name: "FK_assets_media_media_id",
                        column: x => x.media_id,
                        principalSchema: "catalog",
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_assets_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "price_rule_direct",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_list_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_rule_direct", x => x.id);
                    table.ForeignKey(
                        name: "FK_price_rule_direct_pricing_list_price_list_id",
                        column: x => x.price_list_id,
                        principalSchema: "catalog",
                        principalTable: "pricing_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_price_rule_direct_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "price_rule_quantity",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    min = table.Column<decimal>(type: "numeric", nullable: false),
                    max = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_list_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_rule_quantity", x => x.id);
                    table.ForeignKey(
                        name: "FK_price_rule_quantity_pricing_list_price_list_id",
                        column: x => x.price_list_id,
                        principalSchema: "catalog",
                        principalTable: "pricing_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_price_rule_quantity_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_tenant_roles",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_tenant_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_person_tenant_roles_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "catalog",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_person_tenant_roles_tenant_role_tenant_role_id",
                        column: x => x.tenant_role_id,
                        principalSchema: "catalog",
                        principalTable: "tenant_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_person_tenant_roles_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "catalog",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_role_permission",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    resource = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    platform_role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_role_permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenant_role_permission_tenant_role_platform_role_id",
                        column: x => x.platform_role_id,
                        principalSchema: "catalog",
                        principalTable: "tenant_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_person_id",
                schema: "catalog",
                table: "account",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_assets_media_id",
                schema: "catalog",
                table: "assets",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "IX_assets_tenant_id",
                schema: "catalog",
                table: "assets",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_tenant_id_entity_name_entity_id",
                schema: "catalog",
                table: "audit_log",
                columns: new[] { "tenant_id", "entity_name", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_tenant_id_ocurred_at",
                schema: "catalog",
                table: "audit_log",
                columns: new[] { "tenant_id", "ocurred_at" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_user_id",
                schema: "catalog",
                table: "audit_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_tenant_id",
                schema: "catalog",
                table: "budget",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_user_id",
                schema: "catalog",
                table: "budget",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_item_budget_id",
                schema: "catalog",
                table: "budget_item",
                column: "budget_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_item_tenant_id",
                schema: "catalog",
                table: "budget_item",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_category_ParentId",
                schema: "catalog",
                table: "category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_category_tenant_id",
                schema: "catalog",
                table: "category",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_media_tenant_id",
                schema: "catalog",
                table: "media",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_person_email",
                schema: "catalog",
                table: "person",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_platform_roles_person_id",
                schema: "catalog",
                table: "person_platform_roles",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_person_platform_roles_platform_role_id",
                schema: "catalog",
                table: "person_platform_roles",
                column: "platform_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_person_tenant_roles_person_id",
                schema: "catalog",
                table: "person_tenant_roles",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_person_tenant_roles_tenant_id",
                schema: "catalog",
                table: "person_tenant_roles",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_person_tenant_roles_tenant_role_id",
                schema: "catalog",
                table: "person_tenant_roles",
                column: "tenant_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_platform_membership_person_id",
                schema: "catalog",
                table: "platform_membership",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_platform_role_permission_platform_role_id",
                schema: "catalog",
                table: "platform_role_permission",
                column: "platform_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_rule_direct_price_list_id",
                schema: "catalog",
                table: "price_rule_direct",
                column: "price_list_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_rule_direct_tenant_id",
                schema: "catalog",
                table: "price_rule_direct",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_rule_quantity_price_list_id",
                schema: "catalog",
                table: "price_rule_quantity",
                column: "price_list_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_rule_quantity_tenant_id",
                schema: "catalog",
                table: "price_rule_quantity",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_list_tenant_id",
                schema: "catalog",
                table: "pricing_list",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_tenant_id",
                schema: "catalog",
                table: "product",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_person_id",
                schema: "catalog",
                table: "refresh_token",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_token_hash",
                schema: "catalog",
                table: "refresh_token",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_membership_person_id_tenant_id",
                schema: "catalog",
                table: "tenant_membership",
                columns: new[] { "person_id", "tenant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_membership_tenant_id",
                schema: "catalog",
                table: "tenant_membership",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_module_module_code_tenant_id",
                schema: "catalog",
                table: "tenant_module",
                columns: new[] { "module_code", "tenant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_module_tenant_id",
                schema: "catalog",
                table: "tenant_module",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_role_tenant_id",
                schema: "catalog",
                table: "tenant_role",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_role_permission_platform_role_id",
                schema: "catalog",
                table: "tenant_role_permission",
                column: "platform_role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "assets",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "audit_log",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "budget_item",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "category",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "person_platform_roles",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "person_tenant_roles",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "platform_membership",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "platform_role_permission",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "price_rule_direct",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "price_rule_quantity",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "product",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "refresh_token",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "tenant_membership",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "tenant_module",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "tenant_role_permission",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "media",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "budget",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "platform_role",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "pricing_list",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "tenant_role",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "person",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "tenant",
                schema: "catalog");
        }
    }
}
