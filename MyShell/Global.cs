using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;
using MyShell;
using System.Diagnostics;

public class Global
{
    public static Form SideBar;
    public static MyShell.SideBar.Pinned Pin;
    public static MyShell.Steam.Games Steam;
    private static MyShell.MainMenu MainMenu;
    public static List<ProcessInfo> ProcessInformation = new List<ProcessInfo>();
    //public static List<Command> Commands = new List<Command>();

    public static MyShell.MainMenu Menu
    {
        get
        {
            return MainMenu;
        }
        set
        {
            MainMenu = value;
            MainMenu.Name = "MainMenu";
            MainMenu.Text = "MainMenu";
            MainMenu.Show();
        }
    }

    public static bool Empty(params string[] v)
    {
        bool isEmpty = false;

        foreach(string s in v)
        {
            isEmpty = string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);

            if (isEmpty) break;
        }

        return isEmpty;
    }

    public static void UpdateProcessInfo(Process Process)
    {
        ProcessInfo p = ProcessInformation.Where(proc => proc.Path == Process.MainModule.FileName).First();
        Win32.WINDOWPLACEMENT placement = new Win32.WINDOWPLACEMENT();
        Win32.Rect rect = new Win32.Rect();

        Win32.GetWindowPlacement(Process.MainWindowHandle, ref placement);
        Win32.GetWindowRect(Process.MainWindowHandle, out rect);

        switch (placement.showCmd)
        {
            case 0:
            case 2:
                p.State = FormWindowState.Minimized;
                break;
            case 3:
                p.State = FormWindowState.Maximized;
                break;
            default:
                p.State = FormWindowState.Normal;
                break;
        }

        if (p.State != FormWindowState.Minimized)
        {
            p.Location = new Point(rect.Left, rect.Top);
            p.Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
        }
    }
}

public class ProcessInfo
{
    public string Name;
    public string Path;
    public Size Size;
    public Point Location;
    public FormWindowState State;
}

public class Command
{
    public Command()
    {
        
    }

    public string Name;
    public bool isProgram = false;
    public Action Run;
}