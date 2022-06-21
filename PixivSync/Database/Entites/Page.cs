using JetBrains.Annotations;

namespace PixivSync.Database.Entites;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Page
{
    public PageId Id { get; init; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
    public string ThumbMini { get; set; } = null!;
    public string Small { get; set; } = null!;
    public string Regular { get; set; } = null!;
    public string Original { get; set; } = null!;
}

public sealed class PageId
{
    public int Number { get; init; }
    public long IllustId { get; init; }

    private bool Equals(PageId other)
    {
        return Number == other.Number && IllustId == other.IllustId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((PageId)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Number, IllustId);
    }
}