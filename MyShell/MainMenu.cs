using System;
using System.Drawing;
using System.Windows.Forms;
using MyShell.Menu;

namespace MyShell
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();

            Size = new Size(Screen.AllScreens[1].Bounds.Width, Screen.AllScreens[1].Bounds.Height);
            Screen LeftScreen = PositionLeft();
            Location = new Point(LeftScreen.Bounds.Left, LeftScreen.Bounds.Top);

            new Time(this);
            new RAM(this);
            new CPU(this);

            new Search(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            Win32.SetWindowPos(Handle, Win32.HWND.Bottom, 0, 0, 0, 0, Win32.SWP.NOACTIVATE | Win32.SWP.NOSIZE | Win32.SWP.NOMOVE);
        }

        protected override void OnActivated(EventArgs e)
        {
            Win32.SetWindowPos(Handle, Win32.HWND.Bottom, 0, 0, 0, 0, Win32.SWP.NOACTIVATE | Win32.SWP.NOSIZE | Win32.SWP.NOMOVE);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (WindowState != FormWindowState.Normal) WindowState = FormWindowState.Normal;
        }

        private Screen PositionLeft()
        {
            Screen LeftScreen = Screen.AllScreens[0];

            foreach (Screen scr in Screen.AllScreens)
                if (scr.Bounds.Left < LeftScreen.Bounds.Left)
                    LeftScreen = scr;

            return LeftScreen;
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
    }
}
