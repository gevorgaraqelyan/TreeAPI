using Microsoft.EntityFrameworkCore;
using TreeAPI.Controllers;
using TreeAPI.Models;

namespace TreeAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Tree> Trees { get; set; } = null!;
    public DbSet<Node> Nodes { get; set; } = null!;
    public DbSet<ExceptionLog> ExceptionLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tree>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();

            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(e => e.Tree)
                .WithMany(e => e.Nodes)
                .HasForeignKey(e => e.TreeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<ExceptionLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventId).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.ExceptionType).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}