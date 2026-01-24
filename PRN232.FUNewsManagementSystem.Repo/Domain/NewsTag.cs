
public class NewsTag
{
    public int NewsArticleID { get; set; }
    public int TagID { get; set; }

    // Navigation properties
    public virtual NewsArticle NewsArticle { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
