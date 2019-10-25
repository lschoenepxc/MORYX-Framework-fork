﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    [DefaultSchema(ProductsConstants.Schema)]
    public class ProductsContext : MarvinDbContext
    {
        /// <inheritdoc />
        public ProductsContext()
        { 
        }

        /// <inheritdoc />
        public ProductsContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {           
        }

        /// <inheritdoc />
        public ProductsContext(DbConnection connection, ContextMode mode) : base(connection, mode)
        {
        }

        public virtual DbSet<ProductTypeEntity> ProductEntities { get; set; }
    
        public virtual DbSet<PartLink> PartLinks { get; set; }
    
        public virtual DbSet<ProductRecipeEntity> ProductRecipeEntities { get; set; }
    
        public virtual DbSet<ProductProperties> ProductProperties { get; set; }

        public virtual DbSet<ProductFileEntity> ProductFiles { get; set; }

        public virtual DbSet<ProductInstanceEntity> ArticleEntities { get; set; }
    
        public virtual DbSet<WorkplanEntity> WorkplanEntities { get; set; }

        public virtual DbSet<WorkplanReference> WorkplanReferences { get; set; }
    
        public virtual DbSet<StepEntity> StepEntities { get; set; }

        public virtual DbSet<ConnectorEntity> ConnectorEntities { get; set; }

        public virtual DbSet<ConnectorReference> ConnectorReferences { get; set; }
    
        public virtual DbSet<OutputDescriptionEntity> OutputDescriptionEntities { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Workplane reference
            modelBuilder.Entity<WorkplanReference>()
                .HasRequired(w => w.Target)
                .WithMany(w => w.TargetReferences)
                .HasForeignKey(s => s.TargetId);

            modelBuilder.Entity<WorkplanReference>()
                .HasRequired(w => w.Source)
                .WithMany(w => w.SourceReferences)
                .HasForeignKey(s => s.SourceId);

            // Article
            modelBuilder.Entity<ProductInstanceEntity>()
                .HasRequired(p => p.ProductType)
                .WithMany()
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductInstanceEntity>()
                .HasOptional(a => a.Parent)
                .WithMany(a => a.Parts)
                .HasForeignKey(a => a.ParentId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ProductInstanceEntity>()
                .HasOptional(a => a.PartLink)
                .WithMany()
                .HasForeignKey(a => a.PartLinkId);

            // Connector
            modelBuilder.Entity<ConnectorEntity>()
                .HasRequired(c => c.Workplan)
                .WithMany(w => w.Connectors)
                .HasForeignKey(c => c.WorkplanId);

            modelBuilder.Entity<ConnectorEntity>()
                .HasMany(c => c.Usages)
                .WithOptional(c => c.Connector)
                .HasForeignKey(c => c.ConnectorId);

            // Step entity
            modelBuilder.Entity<StepEntity>()
                .HasMany(s => s.Connectors);

            modelBuilder.Entity<StepEntity>()
                .HasMany(s => s.OutputDescriptions)
                .WithRequired(o => o.Step)
                .HasForeignKey(o => o.StepEntityId);

            modelBuilder.Entity<StepEntity>()
                .HasRequired(s => s.Workplan)
                .WithMany(w => w.Steps)
                .HasForeignKey(s => s.WorkplanId);

            modelBuilder.Entity<StepEntity>()
                .HasOptional(s => s.SubWorkplan)
                .WithMany(w => w.Parents)
                .HasForeignKey(s => s.SubWorkplanId);

            // RecipeEntity
            modelBuilder.Entity<ProductRecipeEntity>()
                .HasOptional(r => r.Workplan)
                .WithMany(w => w.Recipes)
                .HasForeignKey(s => s.WorkplanId);

            // ProductEntity
            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Parts)
                .WithRequired(p => p.Parent)
                .HasForeignKey(p => p.ParentId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Parents)
                .WithRequired(p => p.Child)
                .HasForeignKey(p => p.ChildId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.Recipes)
                .WithRequired(p => p.ProductType)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasMany(p => p.OldVersions)
                .WithOptional(p => p.ProductType)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductTypeEntity>()
                .HasRequired(p => p.CurrentVersion)
                .WithMany()
                .HasForeignKey(t => t.CurrentVersionId);

            // Indexes
            modelBuilder.Entity<ProductTypeEntity>()
                .Property(e => e.Identifier)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new[]
                    {
                        new IndexAttribute("Identifier_Revision_Index", 1),
                        new IndexAttribute("Identifier")
                    }));

            modelBuilder.Entity<ProductTypeEntity>()
                .Property(e => e.Revision)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("Identifier_Revision_Index", 2)));
        }
    }
}
