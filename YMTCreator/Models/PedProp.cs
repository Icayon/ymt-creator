namespace YMTCreator.Models;

public class PedPropDrawable
{
    public int        Index           { get; set; }
    public PropAnchor Anchor          { get; set; }
    public bool       HasAlpha        { get; set; }
    public int        NumAlternatives { get; set; }

    public List<PedPropTexture> Textures { get; set; } = [];

    public string IndexStr    => Index.ToString("D3");
    public string AnchorName  => Anchor.ToString().ToUpper();
    public string DisplayName =>
        $"Prop {IndexStr}  |  {Textures.Count} tex" +
        (HasAlpha ? "  [alpha]" : "") +
        (NumAlternatives > 0 ? $"  +{NumAlternatives} alt" : "");
}
