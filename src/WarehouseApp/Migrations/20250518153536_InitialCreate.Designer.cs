﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WarehouseApp.Infrastructure;

#nullable disable

namespace WarehouseApp.Migrations
{
    [DbContext(typeof(WarehouseContext))]
    [Migration("20250518153536_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("WarehouseApp.Domain.Box", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Depth")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ExpirationDateInput")
                        .HasColumnType("DATE");

                    b.Property<decimal>("Height")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ManufactureDate")
                        .HasColumnType("DATE");

                    b.Property<Guid?>("PalletId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Weight")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Width")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PalletId");

                    b.ToTable("Boxes");
                });

            modelBuilder.Entity("WarehouseApp.Domain.Pallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Depth")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Height")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Weight")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Width")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Pallets");
                });

            modelBuilder.Entity("WarehouseApp.Domain.Box", b =>
                {
                    b.HasOne("WarehouseApp.Domain.Pallet", null)
                        .WithMany("Boxes")
                        .HasForeignKey("PalletId");
                });

            modelBuilder.Entity("WarehouseApp.Domain.Pallet", b =>
                {
                    b.Navigation("Boxes");
                });
#pragma warning restore 612, 618
        }
    }
}
