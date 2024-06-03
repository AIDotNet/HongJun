using HongJun.Service.Domina;
using HongJun.Service.Domina.Core;
using HongJun.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HongJun.Service.DataAccess;

public sealed class MasterDbContext(DbContextOptions<MasterDbContext> options,UserContext userContext) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.HasIndex(e => e.GiteeId);
            entity.HasIndex(e => e.GithubId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Phone);
        });
        
        var user = new User(Guid.NewGuid().ToString("N"),"admin","239573049@qq.com","Aa010426");
        
        user.SetAdmin();
        
        modelBuilder.Entity<User>().HasData(user);
        
        
    }
    
    public override int SaveChanges()
    {
        OnBeforeSaveChanges();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaveChanges()
    {
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            if (userContext.IsAuthenticated)
            {
                switch (entry)
                {
                    case { State: EntityState.Added, Entity: ICreatable creatable }:
                        creatable.Creator ??= userContext.CurrentUserId;
                        if (creatable.CreatedAt == default)
                            creatable.CreatedAt = DateTime.Now;
                        break;
                    case { State: EntityState.Modified, Entity: IUpdatable entity }:
                        entity.UpdatedAt ??= DateTime.Now;
                        entity.Modifier ??= userContext.CurrentUserId;
                        break;
                }
            }
            else
            {
                switch (entry.Entity)
                {
                    case ICreatable creatable:
                        creatable.CreatedAt = DateTime.Now;
                        break;
                    case IUpdatable entity:
                        entity.UpdatedAt = DateTime.Now;
                        break;
                }
            }
        }
    }
}