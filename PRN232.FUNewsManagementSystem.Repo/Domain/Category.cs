
public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CategoryDescription { get; set; }
    public int? ParentCategoryID { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
