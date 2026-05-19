namespace YMTCreator.Models;

public class PedComponent
{
    public ComponentSlot Slot      { get; set; }
    public bool          IsEnabled { get; set; } = true;

    public List<PedDrawable> Drawables { get; set; } = [];

    public string SlotName    => Slot.ToString().ToUpper();
    public string DisplayName => $"[{(int)Slot}]  {SlotName}  —  {Drawables.Count} drawable(s)";
}
