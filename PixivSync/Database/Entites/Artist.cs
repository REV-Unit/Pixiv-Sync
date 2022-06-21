using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace PixivSync.Database.Entites;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Artist : User
{
    public string Name { get; set; } = null!;
    public virtual ICollection<Illust> Illusts { get; protected set; } = null!;

    public string NormalizedName
    {
        get
        {
            Name = Regex.Replace(Name, @"[\/\\:\*""\?<>\|]", "_");
            Name = Regex.Replace(Name, @"[@＠][^@＠]*$", string.Empty);
            return Name;
        }
    }
}