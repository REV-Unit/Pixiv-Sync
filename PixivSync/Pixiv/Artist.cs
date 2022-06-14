namespace PixivSync.Pixiv;

public class Artist
{
    public virtual long Id { get; init; }
    public virtual string Name { get; set; }
    public virtual ICollection<Illust> Illusts { get; protected set; }
}