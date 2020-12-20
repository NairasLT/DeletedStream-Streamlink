using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public static class DeleteSystem
{
    public static void Parse(IList<FileEntry> EntryList)
    {
        foreach (FileEntry entry in EntryList)
        {
            if (entry.DeleteTime > DateTime.Now)
                continue; // Jeigu dar neatejo Laikas.

            TryDelete(entry.Path);
        }
    }

    private static void TryDelete(string Path)
    {
        try
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
                ConsoleHelpers.WriteInColor($"File auto deleted: {Path}", ConsoleColor.Red);
            }
        } catch(Exception x) { ConsoleHelpers.WriteInColor($"Exception occured while trying to auto delete file: {Path} error message: {x.Message}", ConsoleColor.Red); }
    }

}

public struct FileEntry
{
    public string Path;
    public DateTime DeleteTime;

    public FileEntry(string path, DateTime deleteTime)
    {
        Path = path;
        DeleteTime = deleteTime;
    }
}
