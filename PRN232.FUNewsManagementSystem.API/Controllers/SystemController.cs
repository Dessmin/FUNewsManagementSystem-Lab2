using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.FUNewsManagementSystem.API.Controllers;

/// <summary>
/// System management endpoints for seeding data
/// </summary>
[Route("api/system")]
[ApiController]
public class SystemController : ControllerBase
{
    private readonly NewsManagementDbContext _context;
    private readonly ILogger<SystemController> _logger;

    public SystemController(NewsManagementDbContext context, ILogger<SystemController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seed all initial data including categories, tags, system accounts, and news articles
    /// </summary>
    /// <returns>Success message with seeded data count</returns>
    [HttpPost("seed")]
    [SwaggerOperation(
        Summary = "Seed all initial data",
        Description = "Seeds the database with initial categories, tags, system accounts, and sample news articles. No authorization required."
    )]
    [SwaggerResponse(200, "Data seeded successfully")]
    [SwaggerResponse(409, "Data already exists")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> SeedDataAsync()
    {
        try
        {
            _logger.LogInformation("Starting data seeding process...");

            // Check if data already exists
            var hasData = await _context.SystemAccounts.AnyAsync() || 
                         await _context.Categories.AnyAsync() ||
                         await _context.Tags.AnyAsync();

            if (hasData)
            {
                _logger.LogWarning("Database already contains data");
                return Conflict(new { Message = "Database already contains data. Seeding skipped." });
            }

            var seededCounts = new
            {
                SystemAccounts = 0,
                Categories = 0,
                Tags = 0,
                NewsArticles = 0
            };

            // Use execution strategy to handle retries with transactions
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Seed System Accounts
                    var systemAccounts = await SeedSystemAccountsAsync();
                    seededCounts = seededCounts with { SystemAccounts = systemAccounts.Count };

                    // Seed Categories
                    var categories = await SeedCategoriesAsync();
                    seededCounts = seededCounts with { Categories = categories.Count };

                    // Seed Tags
                    var tags = await SeedTagsAsync();
                    seededCounts = seededCounts with { Tags = tags.Count };

                    // Seed News Articles
                    var newsArticles = await SeedNewsArticlesAsync(systemAccounts, categories, tags);
                    seededCounts = seededCounts with { NewsArticles = newsArticles.Count };

                    // Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            _logger.LogInformation("Data seeding completed successfully");

            return Ok(new
            {
                Message = "Database seeded successfully",
                SeededData = seededCounts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during data seeding");
            return StatusCode(500, new { Message = "An error occurred while seeding data", Error = ex.Message });
        }
    }

    private async Task<List<SystemAccount>> SeedSystemAccountsAsync()
    {
        var passwordHasher = new PasswordHasher<SystemAccount>();
        var accounts = new List<SystemAccount>();

        var accountsToCreate = new[]
        {
            new { Name = "System Admin", Email = "admin@funews.edu.vn", Role = 1, Password = "Admin123!" },
            new { Name = "News Staff", Email = "staff@funews.edu.vn", Role = 2, Password = "Staff123!" },
            new { Name = "John Lecturer", Email = "john.lecturer@funews.edu.vn", Role = 3, Password = "Lecturer123!" },
            new { Name = "Jane Staff", Email = "jane.staff@funews.edu.vn", Role = 2, Password = "Staff123!" },
            new { Name = "Dr. Smith", Email = "dr.smith@funews.edu.vn", Role = 3, Password = "Lecturer123!" }
        };

        foreach (var accountData in accountsToCreate)
        {
            var account = new SystemAccount
            {
                AccountName = accountData.Name,
                AccountEmail = accountData.Email,
                AccountRole = accountData.Role,
                AccountPassword = passwordHasher.HashPassword(null, accountData.Password)
            };

            accounts.Add(account);
            await _context.SystemAccounts.AddAsync(account);
        }

        _logger.LogInformation($"Seeded {accounts.Count} system accounts");
        return accounts;
    }

    private async Task<List<Category>> SeedCategoriesAsync()
    {
        var categories = new List<Category>();

        // Parent categories
        var parentCategories = new[]
        {
            new { Name = "Academic", Description = "Academic related news and announcements" },
            new { Name = "Student Life", Description = "Student activities and campus life" },
            new { Name = "Research", Description = "Research activities and publications" },
            new { Name = "Sports", Description = "Sports events and achievements" },
            new { Name = "Technology", Description = "Technology and innovation news" }
        };

        foreach (var categoryData in parentCategories)
        {
            var category = new Category
            {
                CategoryName = categoryData.Name,
                CategoryDescription = categoryData.Description,
                IsActive = true,
                ParentCategoryID = null
            };

            categories.Add(category);
            await _context.Categories.AddAsync(category);
        }

        await _context.SaveChangesAsync(); // Save parent categories first to get IDs

        // Sub-categories
        var academicCategory = categories.First(c => c.CategoryName == "Academic");
        var studentLifeCategory = categories.First(c => c.CategoryName == "Student Life");

        var subCategories = new[]
        {
            new { Name = "Curriculum Updates", Description = "Updates to academic curriculum", ParentId = academicCategory.CategoryID },
            new { Name = "Faculty News", Description = "Faculty appointments and achievements", ParentId = academicCategory.CategoryID },
            new { Name = "Student Events", Description = "Upcoming student events", ParentId = studentLifeCategory.CategoryID },
            new { Name = "Club Activities", Description = "Student club activities and news", ParentId = studentLifeCategory.CategoryID }
        };

        foreach (var subCategoryData in subCategories)
        {
            var subCategory = new Category
            {
                CategoryName = subCategoryData.Name,
                CategoryDescription = subCategoryData.Description,
                IsActive = true,
                ParentCategoryID = subCategoryData.ParentId
            };

            categories.Add(subCategory);
            await _context.Categories.AddAsync(subCategory);
        }

        _logger.LogInformation($"Seeded {categories.Count} categories");
        return categories;
    }

    private async Task<List<Tag>> SeedTagsAsync()
    {
        var tags = new List<Tag>();

        var tagData = new[]
        {
            new { Name = "announcement", Note = "General announcements" },
            new { Name = "deadline", Note = "Important deadlines" },
            new { Name = "event", Note = "Upcoming events" },
            new { Name = "scholarship", Note = "Scholarship opportunities" },
            new { Name = "graduation", Note = "Graduation related news" },
            new { Name = "exam", Note = "Examination related" },
            new { Name = "research", Note = "Research related content" },
            new { Name = "innovation", Note = "Innovation and technology" },
            new { Name = "competition", Note = "Competitions and contests" },
            new { Name = "international", Note = "International programs and exchanges" }
        };

        foreach (var tagInfo in tagData)
        {
            var tag = new Tag
            {
                TagName = tagInfo.Name,
                Note = tagInfo.Note
            };

            tags.Add(tag);
            await _context.Tags.AddAsync(tag);
        }

        _logger.LogInformation($"Seeded {tags.Count} tags");
        return tags;
    }

    private async Task<List<NewsArticle>> SeedNewsArticlesAsync(List<SystemAccount> accounts, List<Category> categories, List<Tag> tags)
    {
        var newsArticles = new List<NewsArticle>();
        var random = new Random();

        var newsData = new[]
        {
            new
            {
                Title = "New Academic Year Registration Opens",
                Headline = "Students can now register for the upcoming academic year",
                Content = "The registration portal for the new academic year is now open. Students are encouraged to complete their course registration by the specified deadline to ensure their preferred class schedules.",
                Source = "Academic Office",
                CategoryName = "Academic",
                TagNames = new[] { "announcement", "deadline" }
            },
            new
            {
                Title = "Research Excellence Awards 2024",
                Headline = "Faculty members recognized for outstanding research contributions",
                Content = "The university proudly announces the recipients of this year's Research Excellence Awards. These faculty members have demonstrated exceptional dedication to advancing knowledge in their respective fields.",
                Source = "Research Office",
                CategoryName = "Research",
                TagNames = new[] { "research", "announcement" }
            },
            new
            {
                Title = "Student Tech Competition Winners",
                Headline = "Computer Science students win national coding competition",
                Content = "Our Computer Science students have achieved remarkable success in the national coding competition, showcasing their programming skills and innovative thinking.",
                Source = "CS Department",
                CategoryName = "Technology",
                TagNames = new[] { "competition", "innovation" }
            },
            new
            {
                Title = "International Exchange Program",
                Headline = "New partnership with European universities announced",
                Content = "The university is excited to announce new partnership agreements with several prestigious European universities, opening new opportunities for student and faculty exchanges.",
                Source = "International Office",
                CategoryName = "Academic",
                TagNames = new[] { "international", "announcement" }
            },
            new
            {
                Title = "Campus Sports Day 2024",
                Headline = "Annual sports day promises exciting competitions",
                Content = "The annual campus sports day is scheduled for next month, featuring various competitive sports and recreational activities for students and staff.",
                Source = "Sports Committee",
                CategoryName = "Sports",
                TagNames = new[] { "event", "competition" }
            }
        };

        foreach (var newsInfo in newsData)
        {
            var category = categories.FirstOrDefault(c => c.CategoryName == newsInfo.CategoryName);
            var creator = accounts[random.Next(accounts.Count)];

            var newsArticle = new NewsArticle
            {
                NewsTitle = newsInfo.Title,
                Headline = newsInfo.Headline,
                NewsContent = newsInfo.Content,
                NewsSource = newsInfo.Source,
                CategoryID = category?.CategoryID ?? categories.First().CategoryID,
                CreatedByID = creator.AccountID,
                CreatedDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                NewsStatus = true,
                ModifiedDate = null,
                UpdatedByID = null
            };

            newsArticles.Add(newsArticle);
            await _context.NewsArticles.AddAsync(newsArticle);
        }

        // Save news articles first to get IDs for NewsTag relationships
        await _context.SaveChangesAsync();

        // Add tags to news articles
        for (var i = 0; i < newsArticles.Count; i++)
        {
            var newsArticle = newsArticles[i];
            var newsInfo = newsData[i];

            foreach (var tagName in newsInfo.TagNames)
            {
                var tag = tags.FirstOrDefault(t => t.TagName == tagName);
                if (tag != null)
                {
                    var newsTag = new NewsTag
                    {
                        NewsArticleID = newsArticle.NewsArticleID,
                        TagID = tag.TagID
                    };

                    await _context.NewsTags.AddAsync(newsTag);
                }
            }
        }

        _logger.LogInformation($"Seeded {newsArticles.Count} news articles with tags");
        return newsArticles;
    }
}