using System.Text.RegularExpressions;

namespace PixivSync.Pixiv;

public class Artist
{
    public virtual long Id { get; init; }
    public virtual string Name { get; set; }
    public virtual ICollection<Illust> Illusts { get; protected set; }

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