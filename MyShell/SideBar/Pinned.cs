using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;

namespace MyShell.SideBar
{
    public class Pinned : Form
    {
        private BackgroundWorker ShowandHide;

        public Pinned(Control Parent)
        {
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(255, 30, 30, 30);
            ForeColor = Color.FromArgb(255, 238, 238, 238);
            StartPosition = FormStartPosition.Manual;
            TopMost = true;

            ShowandHide = new BackgroundWorker();
            ShowandHide.DoWork += ShowandHide_DoWork;
            ShowandHide.WorkerSupportsCancellation = true;

            MinimumSize = Parent.MinimumSize;
            Size = Parent.Size;
            Location = Parent.Location;

            Show();

            UpdateList();

            Opacity = 0;

            Label l = new Label();
            l.Name = "PinnedVisibleLabel";
            l.AutoSize = false;
            l.Size = new Size(Width, 30);
            l.Location = new Point(0, Height - l.Height);
            l.Text = "Hide Pinned";
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
        }

        private void UpdateList()
        {
            RegistryValues[] RegistryV = Regedit.AllObjectsInKey(Regedit.SubKey.Pinned).ToArray();

            int WidthHeight = 80;
            int left = 0;
            int top = 0;

            foreach(RegistryValues rv in RegistryV)
            {
                if (!System.IO.File.Exists(rv.Value)) continue;

                MousePanel p = (MousePanel)Controls[rv.Name];

                if (p == null)
                {
                    p = new MousePanel(rv, Width / 2, left, top);
                    p.MouseClick += (sender, e) => { if (Opacity < 1) return; ShowHidePinned(); };
                    foreach (Control c in p.Controls) c.MouseClick += (sender, e) => { if (Opacity < 1) return; ShowHidePinned(); };

                    Controls.Add(p);
                }
            }

            if (RegistryV.Length > 0)
            {
                if (Width > 100)
                    top = (Height / 2) - ((WidthHeight * (RegistryV.Length / 2)) / 2);
                else
                    top = (Height / 2) - ((WidthHeight * RegistryV.Length) / 2);
            }

            foreach (MousePanel p in Controls.OfType<MousePanel>())
            {
                p.Location = new Point(left, top);

                if (Width > 100)
                {
                    if (left != 0)
                    {
                        left = 0;
                        top = p.Location.Y + p.Height + 5;
                    }
                    else
                        left = p.Location.X + p.Width;
                }
                else
                    top = p.Location.Y + p.Height + 5;
            }
        }

        public void ShowHidePinned()
        {
            UpdateList();

            if (!ShowandHide.IsBusy)
                ShowandHide.RunWorkerAsync();
        }

        private void ShowandHide_DoWork(object sender, DoWorkEventArgs e)
        {
            bool show = Global.Pin.Opacity > 0;

            for (int i = 0, c = 100; i < 101; i++, c--)
            {
                Global.Pin.Invoke(new MethodInvoker(() =>
                {
                    if (show)
                        Global.Pin.Opacity = (c * 0.01);
                    else
                        Global.Pin.Opacity = (i * 0.01);
                }));

                System.Threading.Thread.Sleep(5);
            }
        }
    }
}
