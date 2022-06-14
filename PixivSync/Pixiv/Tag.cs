namespace PixivSync.Pixiv;

public class Tag
{
    public virtual string Name { get; init; }
    public virtual string? Translation { get; set; }
    public virtual string? RomajiName { get; set; }
    public virtual ICollection<Illust> Illusts { get; protected set; }
}