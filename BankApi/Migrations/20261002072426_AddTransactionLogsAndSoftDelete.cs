using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BankApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionLogsAndSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Clients_ClientId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_BankAccounts_BankAccountId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Phones_Clients_ClientId",
                table: "Phones");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Phones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromAccountId = table.Column<int>(type: "integer", nullable: true),
                    FromAccountNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ToAccountId = table.Column<int>(type: "integer", nullable: true),
                    ToAccountNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Clients_ClientId",
                table: "BankAccounts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_BankAccounts_BankAccountId",
                table: "Cards",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phones_Clients_ClientId",
                table: "Phones",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Clients_ClientId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_BankAccounts_BankAccountId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Phones_Clients_ClientId",
                table: "Phones");

            migrationBuilder.DropTable(
                name: "TransactionLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Phones");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Clients");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Clients_ClientId",
                table: "BankAccounts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_BankAccounts_BankAccountId",
                table: "Cards",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Phones_Clients_ClientId",
                table: "Phones",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
