using Microsoft.Win32;
using System.Collections.Generic;

public class Regedit
{
    public enum SubKey
    {
        MyShell = 0,
        SideBar = 1,
        Menu = 2,
        Pinned = 3,
        Steam = 4
    }

    public static void CheckStructure()
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software");

        if (key.OpenSubKey("MyShell") == null)
            key.CreateSubKey("MyShell");

        key = key.OpenSubKey("MyShell", true);

        #region SideBar
        if (key.OpenSubKey("SideBar") == null)
            key.CreateSubKey("SideBar");

        if (key.OpenSubKey("SideBar\\Pinned") == null)
            key.CreateSubKey("SideBar\\Pinned");
        #endregion

        #region Menu
        if (key.OpenSubKey("Menu") == null)
            key.CreateSubKey("Menu");
        #endregion

        #region Steam
        if (key.OpenSubKey("Steam") == null)
            key.CreateSubKey("Steam");
        #endregion
    }

    public static void SetValue(SubKey sk, string valueName, string value)
    {
        if (Global.Empty(valueName, value)) return;

        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\MyShell", true);

        switch(sk)
        {
            case SubKey.Menu:
                key = key.OpenSubKey("Menu", true);
                break;
            case SubKey.SideBar:
                key = key.OpenSubKey("SideBar", true);
                break;
            case SubKey.Pinned:
                key = key.OpenSubKey("SideBar\\Pinned", true);
                break;
        }

        key.SetValue(valueName, value, RegistryValueKind.String);
    }

    public static bool PinnedExists(string valueName, string value)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\MyShell\\SideBar\\Pinned", true);

        string[] subKeyNames = key.GetValueNames();

        foreach (string subKeyName in subKeyNames)
            if (value == key.GetValue(subKeyName).ToString()) return true;

        return false;
    }

    public static string GetValue(SubKey sk, string valueName, string defaultValue)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\MyShell");

        switch(sk)
        {
            case SubKey.Menu:
                key = key.OpenSubKey("Menu", true);
                break;
            case SubKey.SideBar:
                key = key.OpenSubKey("SideBar", true);
                break;
        }

        object value = key.GetValue(valueName);

        if (value == null)
            SetValue(sk, valueName, defaultValue);

        value = key.GetValue(valueName);

        return value == null ? null : value.ToString();
    }

    public static List<RegistryValues> AllObjectsInKey(SubKey sk)
    {
        List<RegistryValues> List = new List<RegistryValues>();
        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\MyShell");

        switch(sk)
        {
            case SubKey.Menu:
                key = key.OpenSubKey("Menu");
                break;
            case SubKey.SideBar:
                key = key.OpenSubKey("SideBar");
                break;
            case SubKey.Pinned:
                key = key.OpenSubKey("SideBar\\Pinned");
                break;
            case SubKey.Steam:
                key = key.OpenSubKey("Steam");
                break;
        }

        string[] valueNames = key.GetValueNames();

        foreach(string v in valueNames)
            List.Add(new RegistryValues() { Name = v, Value = key.GetValue(v).ToString() });

        return List;
    }

    public static List<SteamGame> SteamGames()
    {
        List<SteamGame> List = new List<SteamGame>();
        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\MyShell\\Steam");

        string[] Games = key.GetSubKeyNames();

        foreach(string Game in Games)
        {
            RegistryKey tmp = key.OpenSubKey(Game);

            string Name = Game;
            string Path = tmp.GetValue("Path").ToString();
            string appID = tmp.GetValue("appID").ToString();

            List.Add(new SteamGame() { Name = Name, Path = Path, appID = appID });
        }

        return List;
    }
}

public class RegistryValues
{
    public string Name;
    public string Value;
}

public class SteamGame
{
    public string Name;
    public string Path;
    public string appID;
}
