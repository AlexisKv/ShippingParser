﻿namespace ShippingParser;

public class FileAndPaths
{
    public string DetermineDesktopPath()
    {
        string? desktopPath = null;

        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                break;
            case PlatformID.Unix:
                desktopPath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Desktop");
                break;
            default:
                Console.WriteLine("Unsupported operating system.");
                Environment.Exit(1);
                break;
        }

        return desktopPath;
    }

    // private bool ShouldProcessFile(FileSystemEventArgs e)
    // {
    //     return e.ChangeType == WatcherChangeTypes.Created;
    // }
}