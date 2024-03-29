﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tenon.Repository.EfCore.MySqlTests;

#nullable disable

namespace Tenon.Repository.EfCore.MySqlTests.Migrations
{
    [DbContext(typeof(MySqlTestDbContext))]
    [Migration("20240107073955_create_table_uninTest")]
    partial class create_table_uninTest
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4 ");

            modelBuilder.Entity("Tenon.Repository.EfCore.MySqlTests.Entities.Blog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("CreateBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<long>("ModifyBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ModifyTime")
                        .HasColumnType("datetime");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("blogs", (string)null);
                });

            modelBuilder.Entity("Tenon.Repository.EfCore.MySqlTests.Entities.Post", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("BlogId")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<long>("CreateBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<long>("ModifyBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ModifyTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("BlogId");

                    b.ToTable("posts", (string)null);
                });

            modelBuilder.Entity("Tenon.Repository.EfCore.MySqlTests.Entities.Post", b =>
                {
                    b.HasOne("Tenon.Repository.EfCore.MySqlTests.Entities.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Blog");
                });

            modelBuilder.Entity("Tenon.Repository.EfCore.MySqlTests.Entities.Blog", b =>
                {
                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
