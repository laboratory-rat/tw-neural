using Domain.Collection;
using Domain.Dictionary;
using Domain.General;
using Domain.Neural;
using Domain.User;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public class ApiDbContext : IdentityDbContext<EntityUser>
    {
        public ApiDbContext(DbContextOptions options) : base(options) { }

        // sets
        public DbSet<EntityUserSocial> UserSocials { get; set; }
        public DbSet<TwitterSourceCollection> TwitterSouceCollections { get; set; }
        public DbSet<TwitterSource> TwitterSources { get; set; }
        public DbSet<NeuralNet> NeuralNets { get; set; }
        public DbSet<TrainSet> TrainSets { get; set; }
        public DbSet<TwitterDictionary> TwitterDictionaries { get; set; }
        public DbSet<DictionaryLanguage> DictionaryLanguages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<TwitterSourceCollection>()
                .HasMany(x => x.Sorces)
                .WithOne(x => x.Collection)
                .HasForeignKey(x => x.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TwitterDictionary>()
                .HasMany(x => x.Languages)
                .WithOne(x => x.Dictionary)
                .HasForeignKey(x => x.DictionaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EntityUser>()
                .HasMany(x => x.TwitterCollections)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EntityUser>()
                .HasMany(x => x.TwitterDictionaries)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // neural

            builder
                .Entity<NeuralNet>()
                .HasOne(x => x.User)
                .WithMany(x => x.NeuralNets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<TrainSet>()
                .HasOne(x => x.User)
                .WithMany(x => x.TrainSets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<NeuralNet>()
                .HasOne(x => x.TrainSet)
                .WithMany(x => x.NeuralNets)
                .HasForeignKey(x => x.TrainSetId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override Task<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            (entity as BaseEntity).CreatedTime = DateTime.UtcNow;
            return base.AddAsync(entity, cancellationToken);
        }

        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            var e = entity as BaseEntity;
            if (e != null)
            {
                e.UpdatedTime = DateTime.UtcNow;
            }

            return base.Update(entity);
        }

        public static ApiDbContext CreateFromConnectionString(string connectionString)
        {
            Trace.WriteLine(connectionString);

            var options = new DbContextOptionsBuilder<ApiDbContext>();
            options.UseMySql(connectionString);

            return new ApiDbContext(options.Options);
        }
    }
}
