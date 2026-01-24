
public class NewsArticle
{
    public int NewsArticleID { get; set; }
    public string NewsTitle { get; set; } = null!;
    public string? Headline { get; set; }
    public DateTime CreatedDate { get; set; }
    public string NewsContent { get; set; } = null!;
    public string? NewsSource { get; set; }
    public int CategoryID { get; set; }
    public bool NewsStatus { get; set; }
    public int CreatedByID { get; set; }
    public int? UpdatedByID { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual SystemAccount CreatedBy { get; set; } = null!;
    public virtual SystemAccount? UpdatedBy { get; set; }
    public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
