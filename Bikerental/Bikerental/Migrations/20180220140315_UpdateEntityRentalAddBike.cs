using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bikerental.Migrations
{
    public partial class UpdateEntityRentalAddBike : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BikeID",
                table: "Rentals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_BikeID",
                table: "Rentals",
                column: "BikeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Bikes_BikeID",
                table: "Rentals",
                column: "BikeID",
                principalTable: "Bikes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Bikes_BikeID",
                table: "Rentals");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_BikeID",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "BikeID",
                table: "Rentals");
        }
    }
}
