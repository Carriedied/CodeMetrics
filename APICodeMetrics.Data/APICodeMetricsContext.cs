using APICodeMetrics.Models;
using Microsoft.EntityFrameworkCore;

namespace APICodeMetrics.Data;

public class ApiCodeMetricsContext : DbContext
{
    public ApiCodeMetricsContext(DbContextOptions<ApiCodeMetricsContext> options) : base(options) { }

    public DbSet<GitUser> GitUsers { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Repository> Repositories { get; set; } = null!;
    public DbSet<RepoBranch> RepoBranches { get; set; } = null!;
    public DbSet<RepoCommit> RepoCommits { get; set; } = null!;
    public DbSet<RepoFile> RepoFiles { get; set; } = null!;
    public DbSet<AnalysisResult> AnalysisReports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связей и индексов, если нужно
        modelBuilder.Entity<Repository>()
            .HasOne(r => r.Project)
            .WithMany(p => p.Repositories)
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RepoBranch>()
            .HasOne(rb => rb.Repository)
            .WithMany(r => r.Branches)
            .HasForeignKey(rb => rb.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RepoCommit>()
            .HasOne(rc => rc.Branch)
            .WithMany(b => b.Commits)
            .HasForeignKey(rc => rc.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RepoCommit>()
            .HasOne(rc => rc.Author)
            .WithMany()
            .HasForeignKey(rc => rc.AuthorId)
            .OnDelete(DeleteBehavior.Restrict); // Или Cascade, в зависимости от бизнес-логики

        modelBuilder.Entity<RepoCommit>()
            .HasOne(rc => rc.Committer)
            .WithMany()
            .HasForeignKey(rc => rc.CommitterId)
            .OnDelete(DeleteBehavior.Restrict); // Или Cascade, в зависимости от бизнес-логики

        modelBuilder.Entity<RepoFile>()
            .HasOne(rf => rf.Commit)
            .WithMany(c => c.Files)
            .HasForeignKey(rf => rf.CommitId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnalysisResult>(entity =>
        {
            entity.Property(e => e.Content).HasColumnType("text"); // Убедитесь, что Content будет TEXT
            entity.Property(e => e.AnalysisVersion).HasMaxLength(255); // Установите разумный лимит для версии
        });
    }
}