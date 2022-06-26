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
            string s = Regex.Replace(Name, @"[\/\\:\*""\?<>\|]", "_");
            s = Regex.Replace(s, @"[@＠][^@＠]*$", string.Empty);
            return s;
        }
    }
}