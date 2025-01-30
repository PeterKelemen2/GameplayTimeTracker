using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameplayTimeTracker.SGDB;

public class SGDBFileHandler
{
    public static Dictionary<string, string> GetSGDBFiles(string name = "")
    {
        Dictionary<string, string> files = new Dictionary<string, string>();
        Guid guid = Guid.NewGuid();
        if (name.Length > 0)
        {
            char[] invalidChars = Path.GetInvalidPathChars();
            name = new string(name.Where(c => !invalidChars.Contains(c)).ToArray()).Replace(" ", "_") + "_";
        }

        files.Add("icon", Path.Combine(AppFiles.SavedImagesPath, $"{name}{guid}_icon.png"));
        files.Add("hero", Path.Combine(AppFiles.SavedImagesPath, $"{name}{guid}_hero.png"));

        return files;
    }
}