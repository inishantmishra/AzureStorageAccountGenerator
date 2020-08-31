﻿// <auto-generated />
using System;
using AzureStorageAccountGenerator.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AzureStorageAccountGenerator.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200831072040_StorageAccountModel Modified")]
    partial class StorageAccountModelModified
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("AzureStorageAccountGenerator.Models.DMSServiceInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AzStorageConnectionString")
                        .HasColumnType("text");

                    b.Property<string>("AzStorageContainer")
                        .HasColumnType("text");

                    b.Property<string>("DMSTenantURL")
                        .HasColumnType("text");

                    b.Property<string>("DatabaseName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DatabasePassword")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DatabasePort")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DatabaseServer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DatabaseUser")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDbProvisioned")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsReplication")
                        .HasColumnType("boolean");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("DMSServiceInfo");
                });

            modelBuilder.Entity("AzureStorageAccountGenerator.Models.StorageAccountModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AccessTier")
                        .HasColumnType("text");

                    b.Property<string>("AccountId")
                        .HasColumnType("text");

                    b.Property<string>("AccountName")
                        .HasColumnType("text");

                    b.Property<string>("AccountType")
                        .HasColumnType("text");

                    b.Property<bool?>("AllowBlobPublicAccess")
                        .HasColumnType("boolean");

                    b.Property<string>("BlobEndPoints")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DMSServiceInfoId")
                        .HasColumnType("integer");

                    b.Property<bool?>("EnableHttpsTrafficOnly")
                        .HasColumnType("boolean");

                    b.Property<string>("FileEndPoints")
                        .HasColumnType("text");

                    b.Property<bool?>("IsHnsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("KeyAccessName")
                        .HasColumnType("text");

                    b.Property<string>("KeyAccessValue")
                        .HasColumnType("text");

                    b.Property<string>("KeySource")
                        .HasColumnType("text");

                    b.Property<string>("Kind")
                        .HasColumnType("text");

                    b.Property<string>("LargeFileSharesState")
                        .HasColumnType("text");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<string>("MinimumTlsVersion")
                        .HasColumnType("text");

                    b.Property<string>("PrimaryLocation")
                        .HasColumnType("text");

                    b.Property<string>("QueueEndPoints")
                        .HasColumnType("text");

                    b.Property<string>("SkuName")
                        .HasColumnType("text");

                    b.Property<string>("TableEndPoints")
                        .HasColumnType("text");

                    b.Property<string>("TagKey")
                        .HasColumnType("text");

                    b.Property<string>("TagValue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DMSServiceInfoId");

                    b.ToTable("StorageAccounts");
                });

            modelBuilder.Entity("AzureStorageAccountGenerator.Models.StorageAccountModel", b =>
                {
                    b.HasOne("AzureStorageAccountGenerator.Models.DMSServiceInfo", "DMSServiceInfo")
                        .WithMany()
                        .HasForeignKey("DMSServiceInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
