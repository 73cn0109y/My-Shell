namespace MyShell
{
    public class Settings
    {

    }

    public class SideBar_Settings
    {
        public static int Large
        {
            get { return Regedit.GetValue(Regedit.SubKey.SideBar, "Large", "0").ToInt(); }
            set { Regedit.SetValue(Regedit.SubKey.SideBar, "Large", value.ToString()); }
        }

        public static int PositionLeft
        {
            get { return Regedit.GetValue(Regedit.SubKey.SideBar, "PositionLeft", "0").ToInt(); }
            set { Regedit.SetValue(Regedit.SubKey.SideBar, "PositionLeft", value.ToString()); }
        }
    }

    public class Menu_Settings
    {

    }
}
