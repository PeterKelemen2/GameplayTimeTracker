using System;
using System.Collections.Generic;
using System.IO;

namespace GameplayTimeTracker.SGDB;

public class SGDBFileHandler
{
    public static Dictionary<string, string> GetSGDBFiles()
    {
        Dictionary<string, string> files = new Dictionary<string, string>();
        Guid guid = Guid.NewGuid();
        files.Add("icon", Path.Combine(AppFiles.SGDBFolder, $"{guid}_icon.png"));
        files.Add("hero", Path.Combine(AppFiles.SGDBFolder, $"{guid}_hero.png"));

        return files;
    }
}