using System.IO;

namespace YMTCreator.Models;

public class FileMapping
{
    public string SourcePath { get; set; }
    public string NewName    { get; set; }
    public string OldName    => Path.GetFileName(SourcePath);

    public FileMapping(string sourcePath, string newName)
    {
        SourcePath = sourcePath;
        NewName    = newName;
    }
}
