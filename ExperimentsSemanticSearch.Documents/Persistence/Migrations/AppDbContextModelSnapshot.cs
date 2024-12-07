﻿// <auto-generated />
using ExperimentsSemanticSearch.Documents.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;

#nullable disable

namespace ExperimentsSemanticSearch.Documents.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "vector");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExperimentsSemanticSearch.Documents.Persistence.Entities.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("text");

                    b.Property<string>("CultureInfo")
                        .HasColumnType("text");

                    b.Property<Vector>("EmbeddingVector")
                        .HasColumnType("vector(384)");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EmbeddingVector");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("EmbeddingVector"), "hnsw");
                    NpgsqlIndexBuilderExtensions.HasOperators(b.HasIndex("EmbeddingVector"), new[] { "vector_cosine_ops" });

                    b.ToTable("Document", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
