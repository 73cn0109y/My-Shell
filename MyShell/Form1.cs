using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using MyShell.SideBar;
using MyShell.Steam;
using System.Linq;

namespace MyShell
{
    public partial class Form1 : Form
    {
        private static Win32.Rect oDeskArea;
        private string[] ValidExtensions = { ".exe" };

        public Form1()
        {
            Global.SideBar = this;

            InitializeComponent();

            MinimumSize = new Size(100, 100);
            Size = new Size(SideBar_Settings.Large == 1 ? 200 : 100, Screen.PrimaryScreen.Bounds.Height);
            int Left = SideBar_Settings.PositionLeft == 1 ? Screen.PrimaryScreen.Bounds.Left : Screen.PrimaryScreen.Bounds.Right - Width;
            Location = new Point(Left, 0);
            
            // Initialize DAISy
            new DAISy.Init();
            DAISy.Settings.HourlyTime = bool.Parse(Regedit.GetValue(Regedit.SubKey.MyShell, "HourlyTime", "false"));

            // Initialize Commands for DAISy
            new AllCommands();

            // Load Extras
            Global.Menu = new MainMenu();
            Global.Pin = new Pinned(this);
            Global.Pin.Owner = this;
            Global.Steam = new Games(this);
            Global.Steam.Owner = this;
            Regedit.CheckStructure();
            new ProgramWatcher();
            // --

            Win32.Rect DeskArea = new Win32.Rect();
            Win32.SystemParametersInfo(48, 0, ref DeskArea, 2);
            oDeskArea = DeskArea;
            if (SideBar_Settings.PositionLeft == 1)
            {
                DeskArea.Left = Width;
                DeskArea.Right = Screen.PrimaryScreen.Bounds.Width;
            }
            else
            {
                DeskArea.Left = Screen.PrimaryScreen.Bounds.Location.X;
                DeskArea.Right = Screen.PrimaryScreen.Bounds.Width - Width;
            }
            DeskArea.Bottom = Screen.PrimaryScreen.Bounds.Height;
            Win32.SystemParametersInfo(47, 0, ref DeskArea, 2);

            Label l = new Label();
            l.Name = "PinnedVisibleLabel";
            l.AutoSize = false;
            l.Size = new Size(Width, 30);
            l.Location = new Point(0, Height - l.Height);
            l.Text = "Show Pinned";
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            l.Cursor = Cursors.Hand;
            l.MouseEnter += (sender, e) => { l.BackColor = Color.FromArgb(255, 100, 100, 100); };
            l.MouseLeave += (sender, e) => { l.BackColor = Color.FromArgb(0, 100, 100, 100); };
            l.MouseClick += (sender, e) =>
            {
                Global.Pin.ShowHidePinned();
            };

            Controls.Add(l);

            Label s = new Label();
            s.Name = "SteamGames";
            s.AutoSize = false;
            s.Size = new Size(Width, 30);
            s.Location = new Point(0, 0);
            s.Text = "Loading...";
            s.TextAlign = ContentAlignment.MiddleCenter;
            s.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            s.Cursor = Cursors.Hand;
            s.MouseEnter += (sender, e) => { s.BackColor = Color.FromArgb(255, 100, 100, 100); };
            s.MouseLeave += (sender, e) => { s.BackColor = Color.FromArgb(0, 100, 100, 100); };
            s.MouseClick += (sender, e) =>
            {
                Global.Steam.ShowHide();
            };

            Controls.Add(s);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c taskkill /im explorer.exe /f";
            p.Start();

            foreach(Process proc in Process.GetProcesses())
            {
                if (proc.MainWindowTitle == "" || proc.MainWindowTitle == "My Shell") continue;

                Win32.WINDOWPLACEMENT t = new Win32.WINDOWPLACEMENT();

                Win32.GetWindowPlacement(proc.MainWindowHandle, ref t);

                Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Minimize);

                if (t.showCmd == 3)
                    Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.ShowMaximized);
                else if (t.showCmd == 2)
                    Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.ShowMinimized);
                else
                    Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.ShowNormal);
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            drgevent.Effect = DragDropEffects.Link;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            foreach(string File in (string[])drgevent.Data.GetData(DataFormats.FileDrop, false))
            {
                bool valid = ValidExtensions.Where(f => f == System.IO.Path.GetExtension(File)).ToArray().Length > 0;

                if(!valid)
                {
                    DAISy.Out.Say("I could not pin a program because of an invalid file extension!");
                    continue;
                }

                AddPinned add = new AddPinned(this);
                add.ShowDialog();

                if (add.DialogResult == DialogResult.Cancel)
                {
                    DAISy.Out.Say("I did not pin the program for you.");
                    continue;
                }

                if(Regedit.PinnedExists(add.Result, File))
                {
                    DAISy.Out.Say("You have already pinned a program with that name or location.");
                    continue;
                }

                Regedit.SetValue(Regedit.SubKey.Pinned, add.Result, File);
                DAISy.Out.Say("I have pinned " + add.Result + " for you.");

                add.Dispose();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (WindowState != FormWindowState.Normal) WindowState = FormWindowState.Normal;
        }

        protected override void DefWndProc(ref Message m)
        {
            const int WM_MOUSEACTIVATE = 0x21;
            const int MA_NOACTIVATE = 0x0003;

            switch (m.Msg)
            {
                case WM_MOUSEACTIVATE:
                    m.Result = (IntPtr)MA_NOACTIVATE;
                    return;
            }

            base.DefWndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams ret = base.CreateParams;
                ret.ExStyle |= 0x8000000;
                return ret;
            }
        }
    }
}
