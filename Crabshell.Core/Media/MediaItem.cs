using Crabshell.Core.Attributes;
using Crabshell.Core.Attributes.Fields;
using Crabshell.Core.Documents;

namespace Crabshell.Core.Media;

[Collection("media_items", Label = "Media")]
public class MediaItem : CrabshellDocument
{
    [TextField(MaxLength = 255, Required = true)]
    public string FileName { get; set; } = default!;

    [TextField(MaxLength = 1024, Required = true)]
    public string FileKey { get; set; } = default!;

    [TextField(MaxLength = 2048, Required = true)]
    public string Url { get; set; } = default!;

    [TextField(MaxLength = 255, Required = true)]
    public string ContentType { get; set; } = default!;

    [NumberField]
    public long SizeBytes { get; set; }
}