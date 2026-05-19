namespace YMTCreator.Models;

public class ScanResult
{
    public string FolderPath { get; set; } = "";
    public string DlcName   { get; set; } = "";

    public List<PedComponent>    Components { get; set; } = [];
    public List<PedPropDrawable> Props      { get; set; } = [];

    public int TotalDrawables => Components.Sum(c => c.Drawables.Count);
    public int TotalTextures  => Components.Sum(c => c.Drawables.Sum(d => d.Textures.Count));
    public int TotalProps     => Props.Count;
}
