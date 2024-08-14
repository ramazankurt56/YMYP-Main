﻿// <auto-generated />
using System;
using EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("CategoryName");

                    b.HasKey("Id");

                    b.ToTable("Categories", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("2225bf74-9d4a-4147-b7ca-f6f98e991cdf"),
                            Description = "Description 1",
                            Name = "Kategori 1"
                        },
                        new
                        {
                            Id = new Guid("768ecb47-30aa-405d-88c6-f44700d8c2e6"),
                            Description = "Description 2",
                            Name = "Kategori 2"
                        },
                        new
                        {
                            Id = new Guid("913d3414-da95-4267-9a26-d98400cdfbcf"),
                            Description = "Description 2",
                            Name = "Kategori 2"
                        });
                });

            modelBuilder.Entity("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.CategoryProduct", b =>
                {
                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CategoryId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("CategoryProducts");
                });

            modelBuilder.Entity("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Lesson", b =>
                {
                    b.Property<int>("LessonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LessonId"));

                    b.Property<string>("LessonField")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LessonName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LessonType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LessonId");

                    b.ToTable("Lessons");
                });

            modelBuilder.Entity("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.ToTable("Products", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("13e2ae91-1d21-4046-9363-32b4c9209a92"),
                            Name = "Product1",
                            Price = 1m
                        },
                        new
                        {
                            Id = new Guid("05214435-e400-4720-abeb-6691ec84e12a"),
                            Name = "Product2",
                            Price = 50m
                        });
                });

            modelBuilder.Entity("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Topic", b =>
                {
                    b.Property<int>("TopicId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TopicId"));

                    b.Property<int>("LessonId")
                        .HasColumnType("int");

                    b.Property<string>("TopicName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TopicId");

                    b.HasIndex("LessonId");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.CategoryProduct", b =>
                {
                    b.HasOne("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Topic", b =>
                {
                    b.HasOne("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Lesson", "Lesson")
                        .WithMany("Topics")
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lesson");
                });

            modelBuilder.Entity("EntityFrameworkCoreGrup2.CodeFirst.WebAPI.Models.Lesson", b =>
                {
                    b.Navigation("Topics");
                });
#pragma warning restore 612, 618
        }
    }
}
