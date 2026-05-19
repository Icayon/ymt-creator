namespace YMTCreator.Models;

public class PedTexture
{
    public char   Letter     { get; set; }
    public string SkinTone   { get; set; } = "uni";
    public int    TexId      { get; set; }
    public string? SourcePath { get; set; }

    public string DisplayName => $"  tex_{Letter}  ({SkinTone}, texId={TexId})";
}

public class PedPropTexture
{
    public char   Letter     { get; set; }
    public string SkinTone   { get; set; } = "uni";
    public int    TexId      { get; set; }
    public string? SourcePath { get; set; }

    public string DisplayName => $"  tex_{Letter}  ({SkinTone}, texId={TexId})";
}
