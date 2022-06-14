namespace PixivSync.Pixiv;

public class Page
{
    public virtual PageId Id { get; init; }
    //public virtual Illust Illust { get; set; }

    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    public virtual string ThumbMini { get; set; }
    public virtual string Small { get; set; }
    public virtual string Regular { get; set; }
    public virtual string Original { get; set; }
}

public class PageId
{
    public virtual int Number { get; init; }
    public virtual long IllustId { get; init; }

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