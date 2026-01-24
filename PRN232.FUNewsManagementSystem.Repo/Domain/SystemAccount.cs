
public class SystemAccount
{
    public int AccountID { get; set; }
    public string AccountName { get; set; } = null!;
    public string? AccountEmail { get; set; }
    public int AccountRole { get; set; }
    public string AccountPassword { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<NewsArticle> CreatedNewsArticles { get; set; } = new List<NewsArticle>();
    public virtual ICollection<NewsArticle> UpdatedNewsArticles { get; set; } = new List<NewsArticle>();
}
