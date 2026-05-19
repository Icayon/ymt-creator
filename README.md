# YMT Creator — GTA V Ped Clothing

A desktop tool for generating `.ymt` files for GTA V addon peds automatically, by scanning the ped's clothing folder.

Built for FiveM / RAGE modding workflows. No manual XML editing required.

---

## What it does

When creating an addon ped for GTA V or FiveM, every ped needs a `.ymt` file that tells the game which clothing components exist, how many drawables each slot has, and how many texture variants each drawable contains. Writing this file by hand is tedious and error-prone.

This tool scans your ped's stream folder, detects all `.ydd` (drawables) and `.ytd` (textures) files automatically, and generates the correct `.ymt` binary ready to drop into your resource.

---

## Features

- Scans folders **recursively** — works whether your files are flat or organized in subfolders
- Supports the **`pedname^component`** naming convention used by addon peds
- Detects all **12 clothing component slots** (head, berd, hair, uppr, lowr, hand, feet, teef, accs, task, decl, jbib)
- Detects all **prop anchor slots** (p_head, p_eyes, p_ears, p_lwrist, p_rwrist)
- Reads every **texture variant** (a, b, c... up to z) per drawable
- Handles **universal** (`_u`) and **race-specific** (`_r`) model suffixes
- Detects **alternate meshes** (`_1`, `_2`...) for anti-clipping
- Detects **cloth physics** files (`.yld`)
- Outputs a proper **binary `.ymt`** file (RSC7 format, same as vanilla game files)
- Preview the generated XML before saving
- Enable/disable individual components via checkboxes

---

## Requirements

None. Download the `.exe` and run it — no installation needed.

> The executable bundles the .NET 8 runtime and CodeWalker.Core internally.

---

## Usage

1. Open the app
2. Click **Abrir carpeta** and select the folder containing your ped's `.ydd` and `.ytd` files
3. Click **Escanear** — the tool detects all components, drawables and textures
4. Review the tree on the left. Uncheck any components you want to exclude
5. Optionally fill in the **DLC name** field
6. Click **Generar YMT.XML** to preview the output
7. Click **Guardar** → save as `.ymt` (binary, ready to use)

---

## File naming convention

The tool recognises standard GTA V clothing file names:

```
# Drawables
[pedname^]component_index_u.ydd          universal model
[pedname^]component_index_r.ydd          race-specific model
[pedname^]component_index_u_1.ydd        alternate mesh (anti-clipping)

# Textures
[pedname^]component_diff_index_a_uni.ytd   variant a, universal skin
[pedname^]component_diff_index_b_whi.ytd   variant b, caucasian skin
[pedname^]component_diff_index_c_bla.ytd   variant c, black skin

# Props
[pedname^]p_type_index.ydd
[pedname^]p_type_diff_index_a_uni.ytd

# Cloth physics
[pedname^]component_index_u.yld
```

**Supported skin tone suffixes:** `uni` `whi` `bla` `chi` `lat` `ara` `bal` `jam` `kor` `ita` `pak`

**Component slots:**

| ID | Name | Description |
|----|------|-------------|
| 0  | head | Masks / head overlays |
| 1  | berd | Beards / facial hair |
| 2  | hair | Hair |
| 3  | uppr | Torso / upper body |
| 4  | lowr | Legs / pants |
| 5  | hand | Hands / gloves |
| 6  | feet | Shoes |
| 7  | teef | Teeth |
| 8  | accs | Accessories |
| 9  | task | Task accessories (parachute, scuba) |
| 10 | decl | Decals |
| 11 | jbib | Jacket / second torso layer |

---

## Build from source

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
git clone https://github.com/ivancayonh/ymt-creator.git
cd ymt-creator
dotnet publish YMTCreator/YMTCreator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none -o dist_share
```

The output will be a single `YMTCreator.exe` in `dist_share/`.

> `CodeWalker.Core.dll` (from [YMTEditor](https://github.com/grzybeek/YMTEditor)) is required in `YMTCreator/lib/` to compile binary `.ymt` output.

---

## Credits

**Created by Ivan Cayon** ([@ivancayonh](https://github.com/ivancayonh))

Binary `.ymt` compilation powered by [CodeWalker.Core](https://github.com/dexyfex/CodeWalker) by dexyfex.

---

## License

MIT
