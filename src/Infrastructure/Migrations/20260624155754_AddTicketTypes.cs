using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EventHub.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTicketTypes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ticket_types",
            schema: "app",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                event_id = table.Column<int>(type: "integer", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                price_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                capacity = table.Column<int>(type: "integer", nullable: false),
                sold = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                reserved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ticket_types", x => x.id);
                table.ForeignKey(
                    name: "fk_ticket_types_events_event_id",
                    column: x => x.event_id,
                    principalSchema: "app",
                    principalTable: "events",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_ticket_types_event_id",
            schema: "app",
            table: "ticket_types",
            column: "event_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ticket_types",
            schema: "app");
    }
}
