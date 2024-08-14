﻿// <auto-generated />
using System;
using EntityFrameworkCore.Relational.WebApi.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EntityFrameworkCore.Relational.WebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240106093718_AdditionalProductTablosundaProductIdAlaniKeyYapildi")]
    partial class AdditionalProductTablosundaProductIdAlaniKeyYapildi
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EntityFrameworkCore.Relational.WebApi.Models.AdditionalProduct", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("money");

                    b.HasKey("ProductId");

                    b.ToTable("AdditionalProducts");
                });

            modelBuilder.Entity("EntityFrameworkCore.Relational.WebApi.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Products");
                });

            modelBuilder.Entity("EntityFrameworkCore.Relational.WebApi.Models.AdditionalProduct", b =>
                {
                    b.HasOne("EntityFrameworkCore.Relational.WebApi.Models.Product", "Product")
                        .WithOne("AdditionalProduct")
                        .HasForeignKey("EntityFrameworkCore.Relational.WebApi.Models.AdditionalProduct", "ProductId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("EntityFrameworkCore.Relational.WebApi.Models.Product", b =>
                {
                    b.Navigation("AdditionalProduct");
                });
#pragma warning restore 612, 618
        }
    }
}
