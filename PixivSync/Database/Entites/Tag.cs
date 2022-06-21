using JetBrains.Annotations;

namespace PixivSync.Database.Entites;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Tag
{
    public string Name { get; init; } = null!;
    public string? Translation { get; set; }
    public string? RomajiName { get; set; }
    public virtual ICollection<Illust> Illusts { get; protected set; } = null!;
}