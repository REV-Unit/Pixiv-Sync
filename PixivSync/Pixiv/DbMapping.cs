using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace PixivSync.Pixiv;

[UsedImplicitly]
public sealed class ArtistMap : ClassMap<Artist>
{
    public ArtistMap()
    {
        Id(x => x.Id).GeneratedBy.Assigned();
        Map(x => x.Name);
        HasMany(x => x.Illusts).Inverse().Cascade.All().LazyLoad();
    }
}

[UsedImplicitly]
public sealed class IllustMap : ClassMap<Illust>
{
    public IllustMap()
    {
        Id(x => x.Id).GeneratedBy.Assigned();
        References(x => x.Artist).Cascade.All().LazyLoad();
        Map(x => x.Title);
        Map(x => x.Description);
        HasManyToMany(x => x.Tags).Cascade.All().Table("IllustTag").LazyLoad();
        Map(x => x.RestrictType);
        Map(x => x.Type);
        Map(x => x.CreateDate);
        Map(x => x.UploadDate);
        HasMany(x => x.Pages).Cascade.All().LazyLoad();
        Map(x => x.Deleted);
    }
}

[UsedImplicitly]
public sealed class TagMap : ClassMap<Tag>
{
    public TagMap()
    {
        Id(x => x.Name);
        Map(x => x.Translation);
        Map(x => x.RomajiName);
        HasManyToMany(x => x.Illusts).Inverse().Cascade.All().Table("IllustTag").LazyLoad();
    }
}

[UsedImplicitly]
public sealed class PageMap : ClassMap<Page>
{
    public PageMap()
    {
        CompositeId(x => x.Id).Mapped().KeyProperty(x => x.IllustId).KeyProperty(x => x.Number);
        //References(x => x.Illust).Column("IllustId");
        Map(x => x.Width);
        Map(x => x.Height);
        Map(x => x.ThumbMini);
        Map(x => x.Small);
        Map(x => x.Regular);
        Map(x => x.Original);
    }
}