namespace PixivSync.Pixiv;

public class Tag
{
    public string Name { get; init; }
    public string? Translation { get; set; }
    public string? RomajiName { get; set; }
    public virtual ICollection<Illust> Illusts { get; protected set; }
}