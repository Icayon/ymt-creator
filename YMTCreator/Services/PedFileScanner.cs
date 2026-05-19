using System.IO;
using System.Text.RegularExpressions;
using YMTCreator.Models;

namespace YMTCreator.Services;

public class PedFileScanner
{
    // ─── Component name → slot index ──────────────────────────────────────────
    private static readonly string[] CompNames =
        ["head", "berd", "hair", "uppr", "lowr", "hand", "feet", "teef", "accs", "task", "decl", "jbib"];

    // ─── Prop type name → (anchorId, PropAnchor enum) ────────────────────────
    private static readonly Dictionary<string, (int id, PropAnchor anchor)> PropDefs = new()
    {
        ["head"]   = (0, PropAnchor.Head),
        ["eyes"]   = (1, PropAnchor.Eyes),
        ["ears"]   = (2, PropAnchor.Ears),
        ["mouth"]  = (3, PropAnchor.Mouth),
        ["lwrist"] = (6, PropAnchor.LWrist),
        ["rwrist"] = (7, PropAnchor.RWrist),
    };

    // ─── Skin-tone suffix → texId ─────────────────────────────────────────────
    private static readonly Dictionary<string, int> ToneToId = new(StringComparer.OrdinalIgnoreCase)
    {
        ["uni"] = 0, ["whi"] = 1, ["bla"] = 2, ["chi"] = 3, ["lat"] = 4,
        ["ara"] = 5, ["bal"] = 6, ["jam"] = 7, ["kor"] = 8, ["ita"] = 9, ["pak"] = 10
    };

    // ─── Optional ped-name prefix pattern: "pedname^" ────────────────────────
    // Matches any characters that are NOT a caret, followed by a caret.
    private const string Pfx = @"(?:[^^]+\^)?";

    // ─── Regexes ───────────────────────────────────────────────────────────────
    // [pedname^]component_index_u|r[_altNumber].ydd
    private static readonly Regex RxCompDrawable = new(
        $@"^{Pfx}(head|berd|hair|uppr|lowr|hand|feet|teef|accs|task|decl|jbib)_(\d{{3}})_(u|r)(?:_(\d+))?\.ydd$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // [pedname^]component_diff_index_letter_tone.ytd
    private static readonly Regex RxCompTexture = new(
        $@"^{Pfx}(head|berd|hair|uppr|lowr|hand|feet|teef|accs|task|decl|jbib)_diff_(\d{{3}})_([a-z])_(uni|whi|bla|chi|lat|ara|bal|jam|kor|ita|pak)\.ytd$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // [pedname^]p_type_index[_altNumber].ydd
    private static readonly Regex RxPropDrawable = new(
        $@"^{Pfx}p_(head|eyes|ears|mouth|lwrist|rwrist)_(\d{{3}})(?:_(\d+))?\.ydd$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // [pedname^]p_type_diff_index_letter_tone.ytd
    private static readonly Regex RxPropTexture = new(
        $@"^{Pfx}p_(head|eyes|ears|mouth|lwrist|rwrist)_diff_(\d{{3}})_([a-z])_(uni|whi|bla|chi|lat|ara|bal|jam|kor|ita|pak)\.ytd$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // [pedname^]component_index_u|r.yld
    private static readonly Regex RxCloth = new(
        $@"^{Pfx}(head|berd|hair|uppr|lowr|hand|feet|teef|accs|task|decl|jbib)_(\d{{3}})_(u|r)\.yld$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // ─── Public API ────────────────────────────────────────────────────────────
    public ScanResult Scan(string folderPath)
    {
        var result = new ScanResult { FolderPath = folderPath };

        var files = Directory
            .GetFiles(folderPath, "*", SearchOption.AllDirectories)
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .Select(f => f!)
            .ToList();

        ScanComponents(files, result);
        ScanProps(files, result);
        CheckClothFiles(files, result);

        return result;
    }

    // ─── Components ────────────────────────────────────────────────────────────
    private static void ScanComponents(List<string> files, ScanResult result)
    {
        var map = new Dictionary<ComponentSlot, Dictionary<int, PedDrawable>>();

        // Pass 1 – drawables (.ydd)
        foreach (var file in files)
        {
            var m = RxCompDrawable.Match(file);
            if (!m.Success) continue;

            var compName = m.Groups[1].Value.ToLower();
            var index    = int.Parse(m.Groups[2].Value);
            var isUniv   = m.Groups[3].Value.Equals("u", StringComparison.OrdinalIgnoreCase);
            var altStr   = m.Groups[4].Value;

            var slot = (ComponentSlot)Array.IndexOf(CompNames, compName);

            if (!map.TryGetValue(slot, out var slotMap))
                map[slot] = slotMap = [];

            if (!slotMap.TryGetValue(index, out var drawable))
                slotMap[index] = drawable = new PedDrawable { Index = index, IsUniversal = isUniv };

            if (!string.IsNullOrEmpty(altStr))
                drawable.NumAlternatives = Math.Max(drawable.NumAlternatives, int.Parse(altStr));
        }

        // Pass 2 – textures (.ytd)
        foreach (var file in files)
        {
            var m = RxCompTexture.Match(file);
            if (!m.Success) continue;

            var compName = m.Groups[1].Value.ToLower();
            var index    = int.Parse(m.Groups[2].Value);
            var letter   = m.Groups[3].Value[0];
            var tone     = m.Groups[4].Value.ToLower();

            var slot = (ComponentSlot)Array.IndexOf(CompNames, compName);

            if (!map.TryGetValue(slot, out var slotMap)) continue;
            if (!slotMap.TryGetValue(index, out var drawable)) continue;
            if (drawable.Textures.Any(t => t.Letter == letter)) continue;

            drawable.Textures.Add(new PedTexture
            {
                Letter   = letter,
                SkinTone = tone,
                TexId    = ToneToId.GetValueOrDefault(tone, 0)
            });
        }

        // Assemble sorted components
        foreach (var kvp in map.OrderBy(k => (int)k.Key))
        {
            var comp = new PedComponent { Slot = kvp.Key };
            comp.Drawables.AddRange(kvp.Value.Values.OrderBy(d => d.Index));
            foreach (var d in comp.Drawables)
                d.Textures = [.. d.Textures.OrderBy(t => t.Letter)];
            result.Components.Add(comp);
        }
    }

    // ─── Props ─────────────────────────────────────────────────────────────────
    private static void ScanProps(List<string> files, ScanResult result)
    {
        var map = new Dictionary<(string, int), PedPropDrawable>();

        // Pass 1 – drawables (.ydd)
        foreach (var file in files)
        {
            var m = RxPropDrawable.Match(file);
            if (!m.Success) continue;

            var propType = m.Groups[1].Value.ToLower();
            var index    = int.Parse(m.Groups[2].Value);
            var altStr   = m.Groups[3].Value;
            var key      = (propType, index);

            if (!PropDefs.TryGetValue(propType, out var def)) continue;

            if (!map.TryGetValue(key, out var prop))
                map[key] = prop = new PedPropDrawable { Index = index, Anchor = def.anchor };

            if (!string.IsNullOrEmpty(altStr))
                prop.NumAlternatives = Math.Max(prop.NumAlternatives, int.Parse(altStr));
        }

        // Pass 2 – textures (.ytd)
        foreach (var file in files)
        {
            var m = RxPropTexture.Match(file);
            if (!m.Success) continue;

            var propType = m.Groups[1].Value.ToLower();
            var index    = int.Parse(m.Groups[2].Value);
            var letter   = m.Groups[3].Value[0];
            var tone     = m.Groups[4].Value.ToLower();
            var key      = (propType, index);

            if (!map.TryGetValue(key, out var prop)) continue;
            if (prop.Textures.Any(t => t.Letter == letter)) continue;

            if (propType == "eyes") prop.HasAlpha = true;

            prop.Textures.Add(new PedPropTexture
            {
                Letter   = letter,
                SkinTone = tone,
                TexId    = ToneToId.GetValueOrDefault(tone, 0)
            });
        }

        result.Props.AddRange(map.Values
            .OrderBy(p => (int)p.Anchor)
            .ThenBy(p => p.Index));

        foreach (var p in result.Props)
            p.Textures = [.. p.Textures.OrderBy(t => t.Letter)];
    }

    // ─── Cloth (.yld) ─────────────────────────────────────────────────────────
    private static void CheckClothFiles(List<string> files, ScanResult result)
    {
        // Build set of (compName, index) pairs that have a .yld
        var yldSet = new HashSet<(string comp, int index)>();
        foreach (var file in files)
        {
            var m = RxCloth.Match(file);
            if (!m.Success) continue;
            var comp  = m.Groups[1].Value.ToLower();
            var index = int.Parse(m.Groups[2].Value);
            yldSet.Add((comp, index));
        }

        foreach (var comp in result.Components)
        {
            var compName = comp.Slot.ToString().ToLower();
            foreach (var d in comp.Drawables)
            {
                if (yldSet.Contains((compName, d.Index)))
                    d.HasCloth = true;
            }
        }
    }
}
