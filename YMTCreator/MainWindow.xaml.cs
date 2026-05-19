using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using YMTCreator.Models;
using YMTCreator.Services;
using YmtBinaryWriter = YMTCreator.Services.YmtBinaryWriter;

namespace YMTCreator;

public partial class MainWindow : Window
{
    private readonly PedFileScanner  _scanner   = new();
    private readonly YmtXmlGenerator _generator = new();

    private ScanResult? _scan;
    private string?     _lastXml;
    private bool        _folderSelected;

    public MainWindow()
    {
        InitializeComponent();
        UpdateLangButtons();
    }

    // ── Language toggle ───────────────────────────────────────────────────────
    private void BtnLangEs_Click(object sender, RoutedEventArgs e)
    {
        Lang.IsEnglish = false;
        ApplyLanguage();
    }

    private void BtnLangEn_Click(object sender, RoutedEventArgs e)
    {
        Lang.IsEnglish = true;
        ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        Title                    = Lang.WindowTitle;
        TxtHeaderTitle.Text      = Lang.HeaderTitle;
        TxtHeaderSub.Text        = Lang.HeaderSub;
        TxtFolderLabel.Text      = Lang.FolderLabel;
        TxtDlcLabel.Text         = Lang.DlcLabel;
        TxtDlcName.ToolTip       = Lang.DlcTooltip;
        TxtTreeHeader.Text       = Lang.TreeHeader;
        BtnExpandAll.ToolTip     = Lang.ExpandAll;
        BtnCollapseAll.ToolTip   = Lang.CollapseAll;
        TxtXmlHeader.Text        = Lang.XmlHeader;
        BtnBrowse.Content        = Lang.BtnBrowseText;
        BtnScan.Content          = Lang.BtnScanText;
        BtnGenerate.Content      = Lang.BtnGenerateText;
        BtnSave.Content          = Lang.BtnSaveText;
        BtnCopy.Content          = Lang.BtnCopyText;

        if (!_folderSelected)
            TxtFolder.Text = Lang.FolderHint;

        SetStatus(Lang.StatusReady);
        UpdateLangButtons();

        if (_scan is not null)
        {
            BuildTree(_scan);
            UpdateStats();
        }
    }

    private void UpdateLangButtons()
    {
        var activeBg   = new SolidColorBrush(Color.FromRgb(0x1a, 0x1a, 0x1a));
        var inactiveBg = new SolidColorBrush(Colors.Transparent);
        var activeFg   = Brushes.White;
        var inactiveFg = new SolidColorBrush(Color.FromRgb(0x44, 0x29, 0x00));

        BtnLangEs.Background = !Lang.IsEnglish ? activeBg   : inactiveBg;
        BtnLangEs.Foreground = !Lang.IsEnglish ? activeFg   : inactiveFg;
        BtnLangEn.Background =  Lang.IsEnglish ? activeBg   : inactiveBg;
        BtnLangEn.Foreground =  Lang.IsEnglish ? activeFg   : inactiveFg;
    }

    // ── Folder browser ────────────────────────────────────────────────────────
    private void BtnBrowse_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFolderDialog
        {
            Title = Lang.IsEnglish
                ? "Select the folder containing your ped's .ydd and .ytd files"
                : "Selecciona la carpeta con los .ydd y .ytd de la ped"
        };
        if (dlg.ShowDialog() != true) return;

        TxtFolder.Text   = dlg.FolderName;
        _folderSelected  = true;
        BtnScan.IsEnabled = true;
        SetStatus(Lang.StatusFolderSelected);
    }

    // ── Scan ─────────────────────────────────────────────────────────────────
    private void BtnScan_Click(object sender, RoutedEventArgs e)
    {
        var folder = TxtFolder.Text.Trim();
        if (!Directory.Exists(folder))
        {
            SetStatus(Lang.ErrFolderNotFound, error: true);
            return;
        }

        try
        {
            _scan = _scanner.Scan(folder);
            BuildTree(_scan);
            BtnGenerate.IsEnabled = true;
            BtnSave.IsEnabled     = false;
            _lastXml              = null;
            TxtXml.Text           = "";

            SetStatus(Lang.StatusScanned(
                _scan.Components.Count,
                _scan.TotalDrawables,
                _scan.TotalTextures,
                _scan.TotalProps));
            UpdateStats();
        }
        catch (Exception ex)
        {
            SetStatus(Lang.ErrScan(ex.Message), error: true);
        }
    }

    // ── Generate XML ─────────────────────────────────────────────────────────
    private void BtnGenerate_Click(object sender, RoutedEventArgs e)
    {
        if (_scan is null) return;

        _scan.DlcName = TxtDlcName.Text.Trim();

        try
        {
            _lastXml          = _generator.Generate(_scan);
            TxtXml.Text       = _lastXml;
            BtnSave.IsEnabled = true;
            SetStatus(Lang.StatusGenerated);
        }
        catch (Exception ex)
        {
            SetStatus(Lang.ErrGenerate(ex.Message), error: true);
        }
    }

    // ── Save ─────────────────────────────────────────────────────────────────
    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_lastXml)) return;

        var suggestedName = Path.GetFileName(TxtFolder.Text.TrimEnd('\\', '/'));
        var dlg = new SaveFileDialog
        {
            Title            = Lang.SaveDialogTitle,
            FileName         = $"{suggestedName}.ymt",
            Filter           = "YMT Binary (*.ymt)|*.ymt|YMT XML (*.ymt.xml)|*.ymt.xml|All Files (*.*)|*.*",
            InitialDirectory = TxtFolder.Text
        };

        if (dlg.ShowDialog() != true) return;

        try
        {
            if (dlg.FilterIndex == 1)
            {
                YmtBinaryWriter.Save(_lastXml, dlg.FileName);
                SetStatus(Lang.StatusSavedBinary(dlg.FileName));
            }
            else
            {
                File.WriteAllText(dlg.FileName, _lastXml, System.Text.Encoding.UTF8);
                SetStatus(Lang.StatusSavedXml(dlg.FileName));
            }
        }
        catch (Exception ex)
        {
            SetStatus(Lang.ErrSave(ex.Message), error: true);
        }
    }

    // ── Copy to clipboard ────────────────────────────────────────────────────
    private void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(TxtXml.Text))
        {
            Clipboard.SetText(TxtXml.Text);
            SetStatus(Lang.StatusCopied);
        }
    }

    // ── TreeView expand/collapse ─────────────────────────────────────────────
    private void BtnExpandAll_Click(object sender, RoutedEventArgs e)   => SetExpanded(PedTree, true);
    private void BtnCollapseAll_Click(object sender, RoutedEventArgs e) => SetExpanded(PedTree, false);

    private static void SetExpanded(ItemsControl parent, bool expand)
    {
        foreach (var item in parent.Items)
        {
            if (parent.ItemContainerGenerator.ContainerFromItem(item) is not TreeViewItem tvi) continue;
            tvi.IsExpanded = expand;
            SetExpanded(tvi, expand);
        }
    }

    // ── Build TreeView ────────────────────────────────────────────────────────
    private void BuildTree(ScanResult scan)
    {
        PedTree.Items.Clear();

        // ── Components ────────────────────────────────────────────────────────
        if (scan.Components.Count > 0)
        {
            var compHeader = MakeHeaderItem(Lang.TreeComponents(scan.Components.Count), "#F7B731");
            compHeader.IsExpanded = true;

            foreach (var comp in scan.Components)
            {
                var compItem = MakeCheckItem(comp.DisplayName, comp, comp_CheckChanged);
                compItem.IsExpanded = false;

                foreach (var drawable in comp.Drawables)
                {
                    var drawItem = MakeLabelItem(drawable.DisplayName, "#aaddff");
                    drawItem.IsExpanded = false;

                    foreach (var tex in drawable.Textures)
                        drawItem.Items.Add(MakeLabelItem(tex.DisplayName, "#99bbdd"));

                    compItem.Items.Add(drawItem);
                }

                compHeader.Items.Add(compItem);
            }

            PedTree.Items.Add(compHeader);
        }

        // ── Props ─────────────────────────────────────────────────────────────
        if (scan.Props.Count > 0)
        {
            var propsByAnchor = scan.Props
                .GroupBy(p => p.Anchor)
                .OrderBy(g => (int)g.Key);

            var propHeader = MakeHeaderItem(Lang.TreeProps(scan.Props.Count), "#F7B731");
            propHeader.IsExpanded = true;

            foreach (var group in propsByAnchor)
            {
                var anchorItem = MakeLabelItem(
                    Lang.TreeAnchor(group.Key.ToString().ToUpper(), group.Count()),
                    "#ffdd99");
                anchorItem.IsExpanded = false;

                foreach (var prop in group)
                {
                    var propItem = MakeLabelItem(prop.DisplayName, "#aaddff");
                    propItem.IsExpanded = false;

                    foreach (var tex in prop.Textures)
                        propItem.Items.Add(MakeLabelItem(tex.DisplayName, "#99bbdd"));

                    anchorItem.Items.Add(propItem);
                }

                propHeader.Items.Add(anchorItem);
            }

            PedTree.Items.Add(propHeader);
        }

        if (scan.Components.Count == 0 && scan.Props.Count == 0)
        {
            var empty = new TreeViewItem();
            empty.Header = MakeText(Lang.TreeEmpty, "#888");
            PedTree.Items.Add(empty);
        }
    }

    // ── TreeViewItem factories ────────────────────────────────────────────────
    private static TreeViewItem MakeHeaderItem(string text, string color)
    {
        var item = new TreeViewItem();
        item.Header = MakeText(text, color, bold: true, size: 12);
        return item;
    }

    private static TreeViewItem MakeLabelItem(string text, string color)
    {
        var item = new TreeViewItem();
        item.Header = MakeText(text, color);
        return item;
    }

    private static TreeViewItem MakeCheckItem(string text, PedComponent comp,
        RoutedEventHandler handler)
    {
        var chk = new CheckBox
        {
            IsChecked  = comp.IsEnabled,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e8e8e8")),
            Tag        = comp
        };

        var tb = new TextBlock
        {
            Text       = text,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e8e8e8")),
            Margin     = new Thickness(4, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(chk);
        sp.Children.Add(tb);

        chk.Checked   += handler;
        chk.Unchecked += handler;

        var item = new TreeViewItem { Header = sp };
        return item;
    }

    private static TextBlock MakeText(string text, string hex, bool bold = false, double size = 13) =>
        new()
        {
            Text       = text,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex)),
            FontWeight = bold ? FontWeights.SemiBold : FontWeights.Normal,
            FontSize   = size
        };

    // ── Checkbox toggle handler ───────────────────────────────────────────────
    private void comp_CheckChanged(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox chk && chk.Tag is PedComponent comp)
            comp.IsEnabled = chk.IsChecked == true;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private void SetStatus(string msg, bool error = false)
    {
        TxtStatus.Text       = msg;
        TxtStatus.Foreground = error
            ? new SolidColorBrush(Colors.Tomato)
            : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
    }

    private void UpdateStats()
    {
        if (_scan is null) { TxtStats.Text = ""; return; }
        TxtStats.Text = Lang.Stats(
            _scan.Components.Count,
            _scan.TotalDrawables,
            _scan.TotalTextures,
            _scan.TotalProps);
    }
}
