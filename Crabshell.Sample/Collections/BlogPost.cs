using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;
using Crabshell.Sample.Actions;
using Crabshell.Sample.Hooks;

namespace Crabshell.Sample.Collections;

public enum PostStatus { Draft, Published, Archived }

[Collection("blog_posts",
    Label = "Blog Posts",
    SaveOptions = SaveOption.Save | SaveOption.SaveAndClone | SaveOption.SaveAndGoToNext,
    CustomSaveOptions = new[] { typeof(PublishPostAction) },
    CustomBulkOptions = new[] { typeof(DeleteSelectedPostsAction) },
    Hooks = [typeof(BlogPostHooks)])]
public class BlogPost : CrabshellDocument
{
    [GridOptions(Visible = true, Order = 0, Filterable = true)]
    [TextField(Required = true, MaxLength = 200, Label = "Title")]
    public string Title { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(Required = true, MaxLength = 200, Label = "Slug")]
    public string Slug { get; set; } = default!;

    [GridOptions(Visible = false)]
    [TextField(MaxLength = 500, Label = "Excerpt")]
    public string? Excerpt { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Content")]
    [RichTextField(Label = "Content")]
    public string? Content { get; set; }

    // ── PUBLISHING ──────────────────────────────────────────
    [GridOptions(Visible = true, Order = 1, Width = 120, Sortable = true, Filterable = true)]
    [FieldGroup("Publishing", Sidebar = true)]
    [SelectField(Required = true, Label = "Status")]
    public PostStatus Status { get; set; }

    [GridOptions(Visible = true, Order = 2, Width = 170, Sortable = true)]
    [FieldGroup("Publishing", Sidebar = true)]
    [DateTimeField(Label = "Published At", HasTime = true, ShowNowButton = true, ShowButton = true)]
    public DateTime? PublishedAt { get; set; }

    [GridOptions(Visible = true, Order = 3, Width = 100, Sortable = true)]
    [FieldGroup("Publishing", Sidebar = true)]
    [BoolField(Label = "Featured", IsSwitch = true)]
    public bool IsFeatured { get; set; }

    // ── TAXONOMY ────────────────────────────────────────────
    [GridOptions(Visible = true, Order = 4, Width = 160)]
    [FieldGroup("Taxonomy", Sidebar = true)]
    [RelationshipField(typeof(Author), Label = "Author")]
    public Guid AuthorId { get; set; }

    [GridOptions(Visible = true, Order = 5, Width = 160)]
    [FieldGroup("Taxonomy", Sidebar = true)]
    [RelationshipField(typeof(Category), Label = "Category")]
    public Guid CategoryId { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("Taxonomy", Sidebar = true)]
    [TextField(MaxLength = 300, Label = "Tags (comma-separated)")]
    public string? Tags { get; set; }

    // ── MEDIA ───────────────────────────────────────────────
    [GridOptions(Visible = false)]
    [FieldGroup("Media")]
    [MediaField(Label = "Cover Image", Accept = "image/*", MaxSizeMb = 5)]
    public string? CoverImage { get; set; }

    // ── STATS ───────────────────────────────────────────────
    [GridOptions(Visible = false)]
    [FieldGroup("Stats", Sidebar = true)]
    [NumberField(Label = "Read Time", Min = 1, Max = 120, Step = "1", Suffix = " min")]
    public int? ReadTimeMinutes { get; set; }

    // ── SEO ─────────────────────────────────────────────────
    [GridOptions(Visible = false)]
    [FieldGroup("SEO")]
    [TextField(MaxLength = 200, Label = "Meta Title")]
    public string? MetaTitle { get; set; }

    [GridOptions(Visible = false)]
    [FieldGroup("SEO")]
    [TextField(MaxLength = 300, Label = "Meta Description")]
    public string? MetaDescription { get; set; }
}
