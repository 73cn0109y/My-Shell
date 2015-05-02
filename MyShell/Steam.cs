using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace MyShell.Steam
{
    public class Games : Form
    {
        BackgroundWorker ShowandHide, GetSteamImages;
        public bool doneLoading = false;

        public Games(Control Parent)
        {
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(255, 30, 30, 30);
            ForeColor = Color.FromArgb(255, 238, 238, 238);
            StartPosition = FormStartPosition.Manual;
            TopMost = true;

            ShowandHide = new BackgroundWorker();
            ShowandHide.DoWork += ShowandHide_DoWork;

            GetSteamImages = new BackgroundWorker();
            GetSteamImages.DoWork += GetSteamImages_DoWork;
            GetSteamImages.RunWorkerCompleted += GetSteamImages_RunWorkerCompleted;
            GetSteamImages.RunWorkerAsync();

            MinimumSize = Parent.MinimumSize;
            Size = Parent.Size;
            Location = Parent.Location;

            Show();

            Label l = new Label();
            l.Name = "HideSteamGames";
            l.AutoSize = false;
            l.Size = new Size(Width, 30);
            l.Location = new Point(0, 0);
            l.Text = "Hide Games";
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            l.Cursor = Cursors.Hand;
            l.MouseEnter += (se, ev) => { l.BackColor = Color.FromArgb(255, 100, 100, 100); };
            l.MouseLeave += (se, ev) => { l.BackColor = Color.FromArgb(0, 100, 100, 100); };
            l.MouseClick += (se, ev) =>
            {
                Global.Steam.ShowHide();
            };

            Controls.Add(l);

            Opacity = 0;
        }

        private void GetSteamImages_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Owner.Invoke(new MethodInvoker(() =>
            {
                ((Label)Owner.Controls["SteamGames"]).Text = "Steam Games";
            }));
        }

        private void GetSteamImages_DoWork(object sender, DoWorkEventArgs e)
        {
            int top = 35;
            
            foreach (SteamGame Game in Regedit.SteamGames())
            {
                Panel p = new Panel();
                Label l = new Label();

                string ImageLocation = GetGameImage(Game.appID);

                p.Size = new Size(Width, 75);
                p.Location = new Point(0, top);
                p.Name = Game.Name;
                p.BackgroundImage = Image.FromFile(ImageLocation);
                p.BackgroundImageLayout = ImageLayout.Center;
                p.BackColor = Color.Transparent;
                p.MouseEnter += (s, ev) => { l.Show(); };
                p.MouseLeave += (s, ev) =>
                {
                    Point c = Cursor.Position;
                    Point ps = PointToScreen(p.Location);

                    if (c.X >= ps.X && c.Y >= ps.Y && c.X <= ps.X + p.Width && c.Y <= ps.Y + p.Height) return;

                    l.Hide();
                };
                p.MouseClick += (s, ev) => { if (Opacity < 1) return; System.Diagnostics.Process.Start(Game.Path); ShowHide(); };

                l.AutoSize = false;
                l.AutoEllipsis = true;
                l.Size = new Size(Width, 30);
                l.Location = new Point(0, p.Height - l.Height);
                l.Text = Game.Name;
                l.BackColor = Color.FromArgb(255, 30, 30, 30);
                l.ForeColor = Color.FromArgb(255, 238, 238, 238);
                l.Font = new Font("Segoe UI", 8, FontStyle.Regular);
                l.TextAlign = ContentAlignment.MiddleCenter;
                l.MouseLeave += (se, ev) => { l.Hide(); };
                l.MouseClick += (s, ev) => { if (Opacity < 1) return; System.Diagnostics.Process.Start(Game.Path); ShowHide(); };
                l.Hide();

                p.Controls.Add(l);

                Global.Steam.Invoke(new MethodInvoker(() => { Controls.Add(p); }));

                top += p.Height + 5;
            }
        }

        private string GetGameImage(string i)
        {
            string l = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyShell\\SteamImages\\" + i + ".jpg");

            if(!File.Exists(l))
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Only a test!");
                    string s = client.DownloadString("https://steamdb.info/app/" + i);
                    s = s.Split(new string[] { "<table class=\"table table-bordered table-hover table-fixed\">" }, StringSplitOptions.None)[1];
                    s = s.Split(new string[] { "</td>" }, StringSplitOptions.None)[7];
                    s = s.Split(new string[] { "href=\"" }, StringSplitOptions.None)[1];
                    s = s.Split(new string[] { "\" target" }, StringSplitOptions.None)[0];

                    client.DownloadFile(s, l);
                }
            }

            return l;
        }

        public void ShowHide()
        {
            if(!ShowandHide.IsBusy)
                ShowandHide.RunWorkerAsync();
        }

        private void ShowandHide_DoWork(object sender, DoWorkEventArgs e)
        {
            bool show = Global.Steam.Opacity > 0;

            for (int i = 0, c = 100; i < 101; i++, c--)
            {
                Global.Steam.Invoke(new MethodInvoker(() =>
                {
                    if (show)
                        Global.Steam.Opacity = (c * 0.01);
                    else
                        Global.Steam.Opacity = (i * 0.01);
                }));

                System.Threading.Thread.Sleep(5);
            }
        }
    }
}
