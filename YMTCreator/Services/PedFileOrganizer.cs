using System.IO;
using YMTCreator.Models;

namespace YMTCreator.Services;

public class PedFileOrganizer
{
    private static readonly string[] CompNames =
        ["head", "berd", "hair", "uppr", "lowr", "hand", "feet", "teef", "accs", "task", "decl", "jbib"];

    // ─── Build the list of file renames without touching disk ─────────────────
    public List<FileMapping> BuildMappings(ScanResult scan, string newPedName)
    {
        var mappings = new List<FileMapping>();

        foreach (var comp in scan.Components)
        {
            var compName = comp.Slot.ToString().ToLower();

            for (int i = 0; i < comp.Drawables.Count; i++)
            {
                var d      = comp.Drawables[i];
                var suffix = d.IsUniversal ? "u" : "r";

                // Main .ydd
                if (d.SourcePath != null)
                    mappings.Add(new($"{d.SourcePath}",
                        $"{newPedName}^{compName}_{i:D3}_{suffix}.ydd"));

                // Alt meshes
                foreach (var (altNum, altPath) in d.AltSourcePaths)
                    mappings.Add(new(altPath,
                        $"{newPedName}^{compName}_{i:D3}_{suffix}_{altNum}.ydd"));

                // Textures
                foreach (var tex in d.Textures)
                    if (tex.SourcePath != null)
                        mappings.Add(new(tex.SourcePath,
                            $"{newPedName}^{compName}_diff_{i:D3}_{tex.Letter}_{tex.SkinTone}.ytd"));

                // Cloth physics
                if (d.HasCloth && d.ClothSourcePath != null)
                    mappings.Add(new(d.ClothSourcePath,
                        $"{newPedName}^{compName}_{i:D3}_{suffix}.yld"));
            }
        }

        // Props (grouped by anchor, renumbered per anchor)
        foreach (var group in scan.Props.GroupBy(p => p.Anchor).OrderBy(g => (int)g.Key))
        {
            var anchorName = group.Key.ToString().ToLower();
            int i = 0;
            foreach (var prop in group.OrderBy(p => p.Index))
            {
                if (prop.SourcePath != null)
                    mappings.Add(new(prop.SourcePath,
                        $"{newPedName}^p_{anchorName}_{i:D3}.ydd"));

                foreach (var (altNum, altPath) in prop.AltSourcePaths)
                    mappings.Add(new(altPath,
                        $"{newPedName}^p_{anchorName}_{i:D3}_{altNum}.ydd"));

                foreach (var tex in prop.Textures)
                    if (tex.SourcePath != null)
                        mappings.Add(new(tex.SourcePath,
                            $"{newPedName}^p_{anchorName}_diff_{i:D3}_{tex.Letter}_{tex.SkinTone}.ytd"));
                i++;
            }
        }

        return mappings;
    }

    // ─── Copy files to output folder using the mappings ───────────────────────
    public int CopyFiles(List<FileMapping> mappings, string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);
        int copied = 0;
        foreach (var m in mappings)
        {
            var dest = Path.Combine(outputFolder, m.NewName);
            File.Copy(m.SourcePath, dest, overwrite: true);
            copied++;
        }
        return copied;
    }

    // ─── Build a new ScanResult with sequential indices for YMT generation ────
    public ScanResult BuildRenamedScan(ScanResult original, string newPedName, string outputFolder)
    {
        var result = new ScanResult
        {
            FolderPath = outputFolder,
            DlcName    = original.DlcName
        };

        foreach (var comp in original.Components)
        {
            var newComp = new PedComponent { Slot = comp.Slot, IsEnabled = comp.IsEnabled };
            var compName = comp.Slot.ToString().ToLower();

            for (int i = 0; i < comp.Drawables.Count; i++)
            {
                var d      = comp.Drawables[i];
                var suffix = d.IsUniversal ? "u" : "r";

                var newDrawable = new PedDrawable
                {
                    Index           = i,
                    IsUniversal     = d.IsUniversal,
                    NumAlternatives = d.NumAlternatives,
                    HasCloth        = d.HasCloth,
                    SourcePath      = Path.Combine(outputFolder, $"{newPedName}^{compName}_{i:D3}_{suffix}.ydd"),
                    ClothSourcePath = d.HasCloth
                        ? Path.Combine(outputFolder, $"{newPedName}^{compName}_{i:D3}_{suffix}.yld")
                        : null
                };

                foreach (var tex in d.Textures)
                    newDrawable.Textures.Add(new PedTexture
                    {
                        Letter     = tex.Letter,
                        SkinTone   = tex.SkinTone,
                        TexId      = tex.TexId,
                        SourcePath = Path.Combine(outputFolder,
                            $"{newPedName}^{compName}_diff_{i:D3}_{tex.Letter}_{tex.SkinTone}.ytd")
                    });

                newComp.Drawables.Add(newDrawable);
            }

            result.Components.Add(newComp);
        }

        foreach (var group in original.Props.GroupBy(p => p.Anchor).OrderBy(g => (int)g.Key))
        {
            var anchorName = group.Key.ToString().ToLower();
            int i = 0;
            foreach (var prop in group.OrderBy(p => p.Index))
            {
                var newProp = new PedPropDrawable
                {
                    Index           = i,
                    Anchor          = prop.Anchor,
                    HasAlpha        = prop.HasAlpha,
                    NumAlternatives = prop.NumAlternatives,
                    SourcePath      = Path.Combine(outputFolder, $"{newPedName}^p_{anchorName}_{i:D3}.ydd")
                };

                foreach (var tex in prop.Textures)
                    newProp.Textures.Add(new PedPropTexture
                    {
                        Letter     = tex.Letter,
                        SkinTone   = tex.SkinTone,
                        TexId      = tex.TexId,
                        SourcePath = Path.Combine(outputFolder,
                            $"{newPedName}^p_{anchorName}_diff_{i:D3}_{tex.Letter}_{tex.SkinTone}.ytd")
                    });

                result.Props.Add(newProp);
                i++;
            }
        }

        return result;
    }
}
