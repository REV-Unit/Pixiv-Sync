namespace PixivSync.Pixiv;

public class Page
{
    public PageId Id { get; init; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string ThumbMini { get; set; }
    public string Small { get; set; }
    public string Regular { get; set; }
    public string Original { get; set; }
}

public class PageId
{
    public int Number { get; init; }
    public long IllustId { get; init; }

    protected bool Equals(PageId other)
    {
        return Number == other.Number && IllustId == other.IllustId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((PageId)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Number, IllustId);
    }
}