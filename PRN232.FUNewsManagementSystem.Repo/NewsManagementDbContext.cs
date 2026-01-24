using Microsoft.EntityFrameworkCore;

public class NewsManagementDbContext : DbContext
{
    public NewsManagementDbContext()
    {
    }

    public NewsManagementDbContext(DbContextOptions<NewsManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<NewsArticle> NewsArticles { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<NewsTag> NewsTags { get; set; }
    public virtual DbSet<SystemAccount> SystemAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // NewsArticle Configuration
        modelBuilder.Entity<NewsArticle>(entity =>
        {
            entity.HasKey(e => e.NewsArticleID);

            entity.Property(e => e.NewsTitle)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Headline)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedDate)
                .IsRequired();

            entity.Property(e => e.NewsContent)
                .IsRequired();

            entity.Property(e => e.NewsSource)
                .HasMaxLength(200);

            entity.Property(e => e.NewsStatus)
                .IsRequired();

            entity.Property(e => e.ModifiedDate);

            // Relationship: NewsArticle -> Category
            entity.HasOne(e => e.Category)
                .WithMany(c => c.NewsArticles)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: NewsArticle -> SystemAccount (CreatedBy)
            entity.HasOne(e => e.CreatedBy)
                .WithMany(a => a.CreatedNewsArticles)
                .HasForeignKey(e => e.CreatedByID)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: NewsArticle -> SystemAccount (UpdatedBy)
            entity.HasOne(e => e.UpdatedBy)
                .WithMany(a => a.UpdatedNewsArticles)
                .HasForeignKey(e => e.UpdatedByID)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.CategoryID);
            entity.HasIndex(e => e.CreatedByID);
            entity.HasIndex(e => e.UpdatedByID);
        });

        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID);

            entity.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CategoryDescription)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .IsRequired();

            // Self-referencing relationship: Category -> ParentCategory
            entity.HasOne(e => e.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(e => e.ParentCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ParentCategoryID);
        });

        // Tag Configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagID);

            entity.Property(e => e.TagName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Note)
                .HasMaxLength(500);

            entity.HasIndex(e => e.TagName)
                .IsUnique();
        });

        // NewsTag Configuration (Many-to-Many Junction Table)
        modelBuilder.Entity<NewsTag>(entity =>
        {
            entity.HasKey(e => new { e.NewsArticleID, e.TagID });

            // Relationship: NewsTag -> NewsArticle
            entity.HasOne(e => e.NewsArticle)
                .WithMany(n => n.NewsTags)
                .HasForeignKey(e => e.NewsArticleID)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: NewsTag -> Tag
            entity.HasOne(e => e.Tag)
                .WithMany(t => t.NewsTags)
                .HasForeignKey(e => e.TagID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SystemAccount Configuration
        modelBuilder.Entity<SystemAccount>(entity =>
        {
            entity.HasKey(e => e.AccountID);

            entity.Property(e => e.AccountName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.AccountEmail)
                .HasMaxLength(200);

            entity.Property(e => e.AccountRole)
                .IsRequired();

            entity.Property(e => e.AccountPassword)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasIndex(e => e.AccountEmail)
                .IsUnique();
        });
    }
}
