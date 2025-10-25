using APICodeMetrics.Interfaces;
using APICodeMetrics.Models;
using APICodeMetrics.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace APICodeMetrics.Services;

public class DataCollectionOrchestrator(
    IDataCollector<ProjectDto[]> projectCollector,
    IDataCollector<RepositoryDto[]> repositoryCollector,
    IDataCollector<BranchDto[]> branchCollector,
    IDataCollector<CommitDto[]> commitCollector,
    IDataCollector<CommitDetailsDto> commitDetailsCollector,
    IDataCollector<CommitDiffDto> commitDiffCollector,
    APICodeMetrics.Data.ApiCodeMetricsContext context, // <-- Добавлен контекст
    ILogger<DataCollectionOrchestrator> logger)
{
    public async Task<bool> CollectAllDataAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting full data collection process.");
        
        try
        {
            var projects = await projectCollector.CollectAsync(null, cancellationToken);
            logger.LogInformation("Collected {ProjectCount} projects.", projects.Length);

            foreach (var projectDto in projects)
            { 
                logger.LogInformation("Processing project: {ProjectName}", projectDto.Name);

                var repositories = await repositoryCollector.CollectAsync(projectDto, cancellationToken);
                logger.LogInformation("  Collected {RepoCount} repositories for project {ProjectName}.", repositories.Length, projectDto.Name);

                foreach (var repoDto in repositories)
                {
                    logger.LogInformation("    Processing repository: {RepoName}", repoDto.Name);

                    var repoContext = (Project: projectDto, Repository: repoDto);
                    var branches = await branchCollector.CollectAsync(repoContext, cancellationToken);
                    logger.LogInformation("      Collected {BranchCount} branches for repository {RepoName}.", branches.Length, repoDto.Name);

                    foreach (var branchDto in branches)
                    {
                        logger.LogInformation("        Processing branch: {BranchName}", branchDto.Name);

                        var branchContext = (Project: projectDto, Repository: repoDto, Branch: branchDto);
                        var commits = await commitCollector.CollectAsync(branchContext, cancellationToken);
                        logger.LogInformation("          Collected {CommitCount} commits for branch {BranchName}.", commits.Length, branchDto.Name);

                        foreach (var commitDto in commits)
                        {
                            logger.LogInformation("            Processing commit: {CommitSha}", commitDto.Sha1);

                            if (string.IsNullOrEmpty(commitDto.Sha1))
                            {
                                logger.LogWarning("Found commit with empty or null SHA1 in branch {BranchName}. Will process with empty details/diff.", branchDto.Name);
                            }
                            
                            var commitDetailsContext = (Project: projectDto, Repository: repoDto, Branch: branchDto, Commit: commitDto);
                            var commitDetailsDto = await commitDetailsCollector.CollectAsync(commitDetailsContext, cancellationToken);
                            logger.LogDebug("              Retrieved details for commit {CommitSha}.", commitDto.Sha1);
                            
                            var commitDiffContext = (Project: projectDto, Repository: repoDto, Branch: branchDto, Commit: commitDto);
                            var commitDiffDto = await commitDiffCollector.CollectAsync(commitDiffContext, cancellationToken);
                            logger.LogDebug("              Retrieved diff for commit {CommitSha}.", commitDto.Sha1);

                            await SaveToDatabaseAsync(projectDto, repoDto, branchDto, commitDto, commitDetailsDto, commitDiffDto, cancellationToken);
                        }
                    }
                }
            }

            logger.LogInformation("Full data collection completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during full data collection.");
            return false;
        }
    }
    
    private async Task SaveToDatabaseAsync(
            ProjectDto projectDto,
            RepositoryDto repoDto,
            BranchDto branchDto,
            CommitDto commitDto,
            CommitDetailsDto commitDetailsDto,
            CommitDiffDto commitDiffDto,
            CancellationToken cancellationToken)
    {
        logger.LogDebug("Saving data for commit {CommitSha} to database.", commitDto.Sha1);
        
        var authorEmail = commitDto.Author?.Email ?? "unknown@example.com";
        var authorName = commitDto.Author?.Name ?? "Unknown Author";

        var authorUser = await context.GitUsers.FirstOrDefaultAsync(u => u.Email == authorEmail, cancellationToken);
        
        if (authorUser == null)
        {
            authorUser = new GitUser { Email = authorEmail, Name = authorName };
            context.GitUsers.Add(authorUser);
            await context.SaveChangesAsync(cancellationToken);
        }

        var committerEmail = commitDto.Committer?.Email ?? "unknown@example.com";
        var committerName = commitDto.Committer?.Name ?? "Unknown Committer";

        var committerUser = await context.GitUsers.FirstOrDefaultAsync(u => u.Email == committerEmail, cancellationToken);
        
        if (committerUser == null)
        {
            committerUser = new GitUser { Email = committerEmail, Name = committerName };
            context.GitUsers.Add(committerUser);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        var project = await context.Projects.FirstOrDefaultAsync(p => p.Name == projectDto.Name, cancellationToken);
        
        if (project == null)
        {
            var createdAt = ParseDateTimeString(projectDto.CreatedAt);
            var updatedAt = ParseDateTimeString(projectDto.UpdatedAt);

            project = new Project
            {
                Name = projectDto.Name,
                FullName = projectDto.FullName,
                Description = projectDto.Description,
                IsPublic = projectDto.IsPublic,
                LfsAllow = projectDto.LfsAllow,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
            
            context.Projects.Add(project);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        var repository = await context.Repositories.FirstOrDefaultAsync(r => r.Name == repoDto.Name && r.ProjectId == project.Id, cancellationToken);
        if (repository == null)
        {
            var createdAt = ParseDateTimeString(repoDto.CreatedAt);
            var updatedAt = ParseDateTimeString(repoDto.UpdatedAt);

            repository = new Repository
            {
                Name = repoDto.Name,
                Slug = repoDto.Name,
                Description = repoDto.Description,
                IsFork = repoDto.IsFork,
                OwnerName = repoDto.OwnerName,
                CloneLinkHttps = repoDto.CloneLinks?.Https ?? "",
                CloneLinkSsh = repoDto.CloneLinks?.Ssh ?? "",
                CreatedAt = createdAt, // <-- Присваиваем DateTime? полю DateTime? (если модель изменена)
                UpdatedAt = updatedAt, // <-- Присваиваем DateTime? полю DateTime? (если модель изменена)
                ProjectId = project.Id // Связываем с проектом
            };
            context.Repositories.Add(repository);
            await context.SaveChangesAsync(cancellationToken); // Сохраняем репозиторий, чтобы получить Id
        }

        // 4. Найти или создать Branch
        var branch = await context.RepoBranches.FirstOrDefaultAsync(b => b.Name == branchDto.Name && b.RepositoryId == repository.Id, cancellationToken);
        if (branch == null)
        {
            // --- ИСПРАВЛЕНО: Используем вспомогательный метод (теперь возвращает DateTime?) ---
            var createdAt = ParseDateTimeString(branchDto.CreatedAt);
            var updatedAt = ParseDateTimeString(branchDto.UpdatedAt);

            branch = new RepoBranch
            {
                Name = branchDto.Name,
                IsProtected = branchDto.Protected,
                TargetBranch = branchDto.TargetBranch,
                CreatedAt = createdAt, // <-- Присваиваем DateTime? полю DateTime? (если модель изменена)
                UpdatedAt = updatedAt, // <-- Присваиваем DateTime? полю DateTime? (если модель изменена)
                RepositoryId = repository.Id // Связываем с репозиторием
            };
            context.RepoBranches.Add(branch);
            await context.SaveChangesAsync(cancellationToken); // Сохраняем ветку, чтобы получить Id
        }

        // 5. Создать RepoCommit (даже если Sha1 пустой)
        // --- ИСПРАВЛЕНО: Используем вспомогательный метод (теперь возвращает DateTime?) ---
        var commitCreatedAt = ParseDateTimeString(commitDto.CreatedAt);
        var commitUpdatedAt = ParseDateTimeString(commitDto.UpdatedAt);

        var repoCommit = new RepoCommit
        {
            Sha1 = commitDto.Sha1, // Может быть пустым
            Message = commitDto.Message,
            CreatedAt = commitCreatedAt, // <-- Присваиваем DateTime? полю DateTime? (если модель изменена)
            UpdatedAt = commitUpdatedAt, // <-- Присваиваем DateTime? полю DateTime? (если модель изменена)
            AuthorId = authorUser.Id,      // Связываем с пользователем
            CommitterId = committerUser.Id, // Связываем с пользователем
            BranchId = branch.Id          // Связываем с веткой
        };

        context.RepoCommits.Add(repoCommit);
        await context.SaveChangesAsync(cancellationToken); // Сохраняем коммит, чтобы получить Id

        // 6. Создать RepoFile для диффа (если дифф есть)
        if (!string.IsNullOrEmpty(commitDiffDto.Diff))
        {
            var repoFile = new RepoFile
            {
                FileName = $"diff_for_{commitDto.Sha1 ?? "empty_sha"}.patch", // Имя файла, например
                Content = commitDiffDto.Diff,
                CommitId = repoCommit.Id // Связываем с коммитом
            };
            context.RepoFiles.Add(repoFile);
        }

        // Сохраняем изменения (дифф, если был) в базу данных
        await context.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Successfully saved data for commit {CommitSha} to database.", commitDto.Sha1);
        return;

        DateTime? ParseDateTimeString(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                // Возвращаем null, чтобы обозначить отсутствующую дату.
                logger.LogWarning("Date string is null or empty, using null as default.");
                return null; // <-- Возвращаем null
            }
            try
            {
                var dto = DateTimeOffset.Parse(dateString);
                return dto.UtcDateTime; // Возвращаем DateTime с Kind=Utc, но теперь это DateTime?
            }
            catch (FormatException ex)
            {
                logger.LogWarning(ex, "Failed to parse date string '{DateString}', using null as default.", dateString);
                return null; // <-- Возвращаем null в случае ошибки тоже
            }
        }
    }
}