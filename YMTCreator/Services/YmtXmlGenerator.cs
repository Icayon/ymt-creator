using System.Text;
using YMTCreator.Models;

namespace YMTCreator.Services;

public class YmtXmlGenerator
{
    private static readonly string[] CompNames =
        ["HEAD", "BERD", "HAIR", "UPPR", "LOWR", "HAND", "FEET", "TEEF", "ACCS", "TASK", "DECL", "JBIB"];

    private static readonly Dictionary<PropAnchor, string> AnchorNames = new()
    {
        [PropAnchor.Head]   = "ANCHOR_HEAD",
        [PropAnchor.Eyes]   = "ANCHOR_EYES",
        [PropAnchor.Ears]   = "ANCHOR_EARS",
        [PropAnchor.Mouth]  = "ANCHOR_MOUTH",
        [PropAnchor.LWrist] = "ANCHOR_LEFT_WRIST",
        [PropAnchor.RWrist] = "ANCHOR_RIGHT_WRIST",
    };

    public string Generate(ScanResult scan)
    {
        var enabledComps = scan.Components.Where(c => c.IsEnabled && c.Drawables.Count > 0).ToList();

        // ── availComp (12 values, 255 = unused) ───────────────────────────────
        var availComp = Enumerable.Repeat(255, 12).ToArray();
        for (int i = 0; i < enabledComps.Count; i++)
            availComp[(int)enabledComps[i].Slot] = i;

        // ── compInfos: one entry per drawable across all components ───────────
        var compInfoEntries = new List<(int slot, int drawIdx)>();
        foreach (var comp in enabledComps)
            for (int i = 0; i < comp.Drawables.Count; i++)
                compInfoEntries.Add(((int)comp.Slot, i));

        // ── Props grouped by anchor ───────────────────────────────────────────
        var propsByAnchor = scan.Props
            .GroupBy(p => p.Anchor)
            .OrderBy(g => (int)g.Key)
            .ToList();

        int totalProps = scan.Props.Count;

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<CPedVariationInfo>");
        sb.AppendLine(" <bHasTexVariations value=\"true\" />");
        sb.AppendLine(" <bHasDrawblVariations value=\"true\" />");
        sb.AppendLine(" <bHasLowLODs value=\"false\" />");
        sb.AppendLine(" <bIsSuperLOD value=\"false\" />");
        sb.AppendLine($" <availComp>{string.Join(" ", availComp)}</availComp>");

        // ── aComponentData3 ────────────────────────────────────────────────────
        if (enabledComps.Count > 0)
        {
            sb.AppendLine(" <aComponentData3 itemType=\"CPVComponentData\">");
            foreach (var comp in enabledComps)
            {
                int totalTex = comp.Drawables.Sum(d => d.Textures.Count);
                sb.AppendLine("  <Item>");
                sb.AppendLine($"   <numAvailTex value=\"{totalTex}\" />");
                sb.AppendLine("   <aDrawblData3 itemType=\"CPVDrawblData\">");
                foreach (var d in comp.Drawables)
                {
                    sb.AppendLine("    <Item>");
                    sb.AppendLine($"     <propMask value=\"{d.PropMask}\" />");
                    sb.AppendLine($"     <numAlternatives value=\"{d.NumAlternatives}\" />");
                    AppendTexData(sb, d.Textures.Select(t => t.TexId).ToList(), "CPVTextureData", 5);
                    sb.AppendLine("     <clothData>");
                    sb.AppendLine($"      <ownsCloth value=\"{(d.HasCloth ? "true" : "false")}\" />");
                    sb.AppendLine("     </clothData>");
                    sb.AppendLine("    </Item>");
                }
                sb.AppendLine("   </aDrawblData3>");
                sb.AppendLine("  </Item>");
            }
            sb.AppendLine(" </aComponentData3>");
        }
        else
        {
            sb.AppendLine(" <aComponentData3 itemType=\"CPVComponentData\" />");
        }

        sb.AppendLine(" <aSelectionSets itemType=\"CPedSelectionSet\" />");

        // ── compInfos ─────────────────────────────────────────────────────────
        if (compInfoEntries.Count > 0)
        {
            sb.AppendLine(" <compInfos itemType=\"CComponentInfo\">");
            foreach (var (slot, drawIdx) in compInfoEntries)
            {
                sb.AppendLine("  <Item>");
                sb.AppendLine("   <hash_2FD08CEF>none</hash_2FD08CEF>");
                sb.AppendLine("   <hash_FC507D28>none</hash_FC507D28>");
                sb.AppendLine("   <hash_07AE529D>0 0 0 0 0</hash_07AE529D>");
                sb.AppendLine("   <flags value=\"0\" />");
                sb.AppendLine("   <inclusions>0</inclusions>");
                sb.AppendLine("   <exclusions>0</exclusions>");
                sb.AppendLine($"   <hash_6032815C>PV_COMP_{CompNames[slot]}</hash_6032815C>");
                sb.AppendLine("   <hash_7E103C8B value=\"0\" />");
                sb.AppendLine($"   <hash_D12F579D value=\"{slot}\" />");
                sb.AppendLine($"   <hash_FA1F27BF value=\"{drawIdx}\" />");
                sb.AppendLine("  </Item>");
            }
            sb.AppendLine(" </compInfos>");
        }
        else
        {
            sb.AppendLine(" <compInfos itemType=\"CComponentInfo\" />");
        }

        // ── propInfo ──────────────────────────────────────────────────────────
        sb.AppendLine(" <propInfo>");
        sb.AppendLine($"  <numAvailProps value=\"{totalProps}\" />");

        if (totalProps > 0)
        {
            sb.AppendLine("  <aPropMetaData itemType=\"CPedPropMetaData\">");
            foreach (var prop in scan.Props)
            {
                sb.AppendLine("   <Item>");
                sb.AppendLine("    <audioId>none</audioId>");
                sb.AppendLine("    <expressionMods>0 0 0 0 0</expressionMods>");
                AppendPropTexData(sb, prop.Textures.Select(t => t.TexId).ToList(), 4);
                sb.AppendLine($"    <renderFlags>{(prop.HasAlpha ? "PRF_ALPHA" : "")}</renderFlags>");
                sb.AppendLine("    <propFlags value=\"0\" />");
                sb.AppendLine("    <flags value=\"0\" />");
                sb.AppendLine($"    <anchorId value=\"{(int)prop.Anchor}\" />");
                sb.AppendLine($"    <propId value=\"{prop.Index}\" />");
                sb.AppendLine("    <hash_AC887A91 value=\"0\" />");
                sb.AppendLine("   </Item>");
            }
            sb.AppendLine("  </aPropMetaData>");

            sb.AppendLine("  <aAnchors itemType=\"CAnchorProps\">");
            foreach (var group in propsByAnchor)
            {
                sb.AppendLine("   <Item>");
                sb.AppendLine($"    <props>{string.Join(" ", group.Select(p => p.Textures.Count))}</props>");
                sb.AppendLine($"    <anchor>{AnchorNames[group.Key]}</anchor>");
                sb.AppendLine("   </Item>");
            }
            sb.AppendLine("  </aAnchors>");
        }
        else
        {
            sb.AppendLine("  <aPropMetaData itemType=\"CPedPropMetaData\" />");
            sb.AppendLine("  <aAnchors itemType=\"CAnchorProps\" />");
        }

        sb.AppendLine(" </propInfo>");
        sb.AppendLine($" <dlcName>{scan.DlcName}</dlcName>");
        sb.AppendLine("</CPedVariationInfo>");

        return sb.ToString();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static void AppendTexData(StringBuilder sb, List<int> texIds, string itemType, int indent)
    {
        var pad = new string(' ', indent);
        if (texIds.Count == 0)
        {
            sb.AppendLine($"{pad}<aTexData itemType=\"{itemType}\" />");
            return;
        }
        sb.AppendLine($"{pad}<aTexData itemType=\"{itemType}\">");
        foreach (var id in texIds)
        {
            sb.AppendLine($"{pad} <Item>");
            sb.AppendLine($"{pad}  <texId value=\"{id}\" />");
            sb.AppendLine($"{pad}  <distribution value=\"255\" />");
            sb.AppendLine($"{pad} </Item>");
        }
        sb.AppendLine($"{pad}</aTexData>");
    }

    private static void AppendPropTexData(StringBuilder sb, List<int> texIds, int indent)
    {
        var pad = new string(' ', indent);
        if (texIds.Count == 0)
        {
            sb.AppendLine($"{pad}<texData itemType=\"CPedPropTexData\" />");
            return;
        }
        sb.AppendLine($"{pad}<texData itemType=\"CPedPropTexData\">");
        foreach (var id in texIds)
        {
            sb.AppendLine($"{pad} <Item>");
            sb.AppendLine($"{pad}  <inclusions>0</inclusions>");
            sb.AppendLine($"{pad}  <exclusions>0</exclusions>");
            sb.AppendLine($"{pad}  <texId value=\"{id}\" />");
            sb.AppendLine($"{pad}  <inclusionId value=\"0\" />");
            sb.AppendLine($"{pad}  <exclusionId value=\"0\" />");
            sb.AppendLine($"{pad}  <distribution value=\"255\" />");
            sb.AppendLine($"{pad} </Item>");
        }
        sb.AppendLine($"{pad}</texData>");
    }
}
