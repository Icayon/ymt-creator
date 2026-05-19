using System.IO;
using System.Xml;
using CodeWalker.GameFiles;

namespace YMTCreator.Services;

/// <summary>
/// Compiles a CPedVariationInfo XML string into a binary .ymt file
/// using CodeWalker.Core (same library as YMTEditor).
/// </summary>
public static class YmtBinaryWriter
{
    /// <summary>
    /// Converts the given CPedVariationInfo XML to a binary .ymt byte array.
    /// </summary>
    public static byte[] Compile(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);

        var meta = XmlMeta.GetMeta(doc);
        var data = ResourceBuilder.Build(meta, 2);
        return data;
    }

    /// <summary>
    /// Saves the given CPedVariationInfo XML as a binary .ymt file.
    /// </summary>
    public static void Save(string xml, string outputPath)
    {
        var data = Compile(xml);
        File.WriteAllBytes(outputPath, data);
    }
}
