namespace YMTCreator;

public static class Lang
{
    public static bool IsEnglish { get; set; } = false;

    // Header
    public static string HeaderTitle => IsEnglish
        ? "■  YMT CREATOR — GTA V PED CLOTHING"
        : "■  YMT CREADOR — GTA V PED CLOTHING";
    public static string HeaderSub => IsEnglish
        ? "Generates the .ymt for GTA V addon peds"
        : "Genera el .ymt para peds addon de GTA V";

    // Toolbar
    public static string FolderLabel   => IsEnglish ? "Folder:"          : "Carpeta:";
    public static string FolderHint    => IsEnglish
        ? "(select a folder containing .ydd and .ytd files)"
        : "(selecciona una carpeta con .ydd y .ytd)";
    public static string BtnBrowseText => IsEnglish ? "📁  Open folder"  : "📁  Abrir carpeta";
    public static string BtnScanText   => IsEnglish ? "⟳  Scan"          : "⟳  Escanear";
    public static string DlcLabel      => IsEnglish ? "DLC name:"        : "DLC name:";
    public static string DlcTooltip    => IsEnglish
        ? "Value for <dlcName> field (can be left empty)"
        : "Valor del campo <dlcName> (puede dejarse vacío)";

    // Left panel
    public static string TreeHeader  => IsEnglish ? "DETECTED CLOTHING" : "PRENDAS DETECTADAS";
    public static string ExpandAll   => IsEnglish ? "Expand all"        : "Expandir todo";
    public static string CollapseAll => IsEnglish ? "Collapse all"      : "Colapsar todo";

    // Right panel
    public static string XmlHeader       => IsEnglish
        ? "XML PREVIEW  (ready for CodeWalker)"
        : "PREVISUALIZACIÓN XML  (listo para CodeWalker)";
    public static string BtnGenerateText => IsEnglish ? "⚙  Generate YMT.XML" : "⚙  Generar YMT.XML";
    public static string BtnSaveText     => IsEnglish ? "💾  Save"            : "💾  Guardar";
    public static string BtnCopyText     => IsEnglish ? "📋  Copy"            : "📋  Copiar";

    // Status messages
    public static string StatusReady          => IsEnglish
        ? "Ready. Select a folder to start."
        : "Listo. Selecciona una carpeta para comenzar.";
    public static string StatusFolderSelected => IsEnglish
        ? "Folder selected. Click 'Scan' to detect clothing."
        : "Carpeta seleccionada. Pulsa 'Escanear' para detectar prendas.";
    public static string StatusGenerated      => IsEnglish
        ? "✔ XML generated. Use 'Save' to export the binary .ymt."
        : "✔ XML generado. Usa 'Guardar' para exportar el .ymt binario.";
    public static string StatusCopied         => IsEnglish
        ? "✔ XML copied to clipboard."
        : "✔ XML copiado al portapapeles.";
    public static string ErrFolderNotFound    => IsEnglish
        ? "❌ The folder does not exist."
        : "❌ La carpeta no existe.";

    public static string StatusScanned(int comps, int drawables, int textures, int props) => IsEnglish
        ? $"✔ Scanned: {comps} components, {drawables} drawables, {textures} textures, {props} props."
        : $"✔ Escaneado: {comps} componentes, {drawables} drawables, {textures} texturas, {props} props.";
    public static string StatusSavedBinary(string path) => IsEnglish
        ? $"✔ Saved as binary .ymt: {path}"
        : $"✔ Guardado como binario .ymt: {path}";
    public static string StatusSavedXml(string path) => IsEnglish
        ? $"✔ Saved as XML: {path}"
        : $"✔ Guardado como XML: {path}";
    public static string ErrScan(string msg)     => IsEnglish ? $"❌ Scan error: {msg}"     : $"❌ Error al escanear: {msg}";
    public static string ErrGenerate(string msg) => IsEnglish ? $"❌ Generate error: {msg}" : $"❌ Error al generar XML: {msg}";
    public static string ErrSave(string msg)     => IsEnglish ? $"❌ Save error: {msg}"     : $"❌ Error al guardar: {msg}";

    // Save dialog
    public static string SaveDialogTitle => IsEnglish ? "Save YMT" : "Guardar YMT";

    // TreeView
    public static string TreeComponents(int n) => IsEnglish ? $"COMPONENTS  ({n})" : $"COMPONENTES  ({n})";
    public static string TreeProps(int n)       => $"PROPS  ({n})";
    public static string TreeAnchor(string name, int n) => $"ANCHOR_{name}  ({n} prop(s))";
    public static string TreeEmpty => IsEnglish
        ? "No .ydd / .ytd files found in the folder."
        : "No se encontraron archivos .ydd / .ytd en la carpeta.";

    // Stats
    public static string Stats(int comps, int drawables, int textures, int props) => IsEnglish
        ? $"Components: {comps}  |  Drawables: {drawables}  |  Textures: {textures}  |  Props: {props}"
        : $"Componentes: {comps}  |  Drawables: {drawables}  |  Texturas: {textures}  |  Props: {props}";

    // Window title
    public static string WindowTitle => IsEnglish
        ? "YMT Creator — GTA V  |  by @ivancayonh"
        : "YMT Creador — GTA V  |  by @ivancayonh";

    // ── Organizer tab ────────────────────────────────────────────────────────
    public static string TabGenerator  => IsEnglish ? "YMT Generator"  : "Generador YMT";
    public static string TabOrganizer  => IsEnglish ? "File Organizer" : "Organizador";

    public static string OrgPedName       => IsEnglish ? "New ped name:"    : "Nombre de la ped:";
    public static string OrgPedNameHint   => IsEnglish ? "e.g. mynewped"    : "ej. mynewped";
    public static string OrgOutputFolder  => IsEnglish ? "Output folder:"   : "Carpeta destino:";
    public static string OrgBrowseOutput  => IsEnglish ? "📁  Select"       : "📁  Seleccionar";
    public static string OrgPreview       => IsEnglish ? "🔍  Preview"      : "🔍  Vista previa";
    public static string OrgCopy          => IsEnglish ? "📋  Copy & Rename files" : "📋  Copiar y renombrar";
    public static string OrgGenerateYmt   => IsEnglish ? "⚙  Generate YMT" : "⚙  Generar YMT";
    public static string OrgColOldName    => IsEnglish ? "Original file"    : "Archivo original";
    public static string OrgColNewName    => IsEnglish ? "New name"         : "Nuevo nombre";
    public static string OrgInfo          => IsEnglish
        ? "Scan a folder with clothing from mixed packs. The tool matches each drawable with its textures and renumbers them sequentially. Enter a ped name, select an output folder and click Copy & Rename."
        : "Escanea una carpeta con ropa de distintos packs. La herramienta empareja cada drawable con sus texturas y los renumera secuencialmente. Introduce el nombre de la ped, selecciona la carpeta destino y pulsa Copiar y renombrar.";

    public static string OrgNeedScan     => IsEnglish ? "Scan a folder first (use the toolbar above)." : "Primero escanea una carpeta (usa la barra de herramientas).";
    public static string OrgNeedPedName  => IsEnglish ? "Enter a ped name." : "Introduce el nombre de la ped.";
    public static string OrgNeedOutput   => IsEnglish ? "Select an output folder." : "Selecciona la carpeta destino.";
    public static string OrgSameFolder   => IsEnglish ? "Output folder cannot be the same as the source folder." : "La carpeta destino no puede ser la misma que la de origen.";
    public static string OrgPreviewDone(int n) => IsEnglish ? $"Preview: {n} files will be renamed." : $"Vista previa: se renombrarán {n} archivos.";
    public static string OrgCopyDone(int n)    => IsEnglish ? $"✔ Done! {n} files copied and renamed." : $"✔ Completado. {n} archivos copiados y renombrados.";
    public static string OrgErrCopy(string m)  => IsEnglish ? $"❌ Copy error: {m}" : $"❌ Error al copiar: {m}";
    public static string OrgYmtSaved(string p) => IsEnglish ? $"✔ YMT saved: {p}" : $"✔ YMT guardado: {p}";
}
