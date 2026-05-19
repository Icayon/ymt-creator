namespace YMTCreator.Models;

public class PedDrawable
{
    public int  Index           { get; set; }
    public bool IsUniversal     { get; set; } = true;
    public int  NumAlternatives { get; set; }
    public bool HasCloth        { get; set; }

    public int    PropMask  => IsUniversal ? 1 : 17;
    public string IndexStr  => Index.ToString("D3");
    public string Suffix    => IsUniversal ? "_u" : "_r";

    public List<PedTexture> Textures { get; set; } = [];

    public string DisplayName =>
        $"Drawable {IndexStr} ({Suffix})  |  {Textures.Count} tex" +
        (NumAlternatives > 0 ? $"  +{NumAlternatives} alt" : "") +
        (HasCloth ? "  [cloth]" : "");
}
