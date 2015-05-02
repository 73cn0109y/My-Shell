using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MyShell.Menu
{
    public class Search : TextBox
    {
        Label o;
        bool isControlDown = false;
        SearchEngine Engine = SearchEngine.Google;

        public enum SearchEngine
        {
            Google = 0,
            Bing = 2,
            Yahoo = 3,
            Youtube = 4
        }

        public Search(Control Parent)
        {
            Size = new Size(300, 50);
            Location = new Point(Parent.Width - Width - 10, 10);
            BackColor = Color.FromArgb(255, 30, 30, 30);
            ForeColor = Color.FromArgb(255, 100, 100, 100);
            BorderStyle = BorderStyle.FixedSingle;
            Font = new Font("Segoe UI", 14, FontStyle.Regular);

            Parent.MouseClick += (se, ev) => { if (Text == "") { o.Show(); o.BringToFront(); } else { o.SendToBack(); o.Show(); o.Focus(); } };

            o = new Label();
            o.AutoSize = false;
            o.Size = new Size(Size.Width - 2,  Size.Height - 2);
            o.Location = new Point(Location.X + 1, Location.Y + 1);
            o.BackColor = Color.Transparent;
            o.ForeColor = ForeColor;
            o.Font = Font;
            o.Text = "Search...";
            o.MouseClick += (se, ev) => { o.Hide(); Focus(); };
            o.Cursor = Cursors.IBeam;
            o.TextAlign = ContentAlignment.MiddleLeft;

            Parent.Controls.Add(this);
            Parent.Controls.Add(o);
            o.BringToFront();

            switch(Regedit.GetValue(Regedit.SubKey.Menu, "SearchEngine", "Google"))
            {
                case "YouTube":
                    Engine = SearchEngine.Youtube;
                    break;
                case "Bing":
                    Engine = SearchEngine.Bing;
                    break;
                case "Yahoo":
                    Engine = SearchEngine.Yahoo;
                    break;
                default:
                    Engine = SearchEngine.Google;
                    break;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if(isControlDown)
            {
                switch(e.KeyCode)
                {
                    case Keys.G:
                        Engine = SearchEngine.Google;
                        DAISy.Out.Say("Search engine has been changed to Google.");
                        Regedit.SetValue(Regedit.SubKey.Menu, "SearchEngine", "Google");
                        break;
                    case Keys.Y:
                        Engine = SearchEngine.Yahoo;
                        DAISy.Out.Say("Search engine has been changed to Yahoo.");
                        Regedit.SetValue(Regedit.SubKey.Menu, "SearchEngine", "Yahoo");
                        break;
                    case Keys.B:
                        Engine = SearchEngine.Bing;
                        DAISy.Out.Say("Search engine has been changed to Bing.");
                        Regedit.SetValue(Regedit.SubKey.Menu, "SearchEngine", "Bing");
                        break;
                    case Keys.T:
                        Engine = SearchEngine.Youtube;
                        DAISy.Out.Say("Search engine has been changed to YouTube.");
                        Regedit.SetValue(Regedit.SubKey.Menu, "SearchEngine", "YouTube");
                        break;
                }
            }

            string searchURL = "http://";

            switch(Engine)
            {
                case SearchEngine.Bing:
                    searchURL += "www.bing.com/search?q=";
                    break;
                case SearchEngine.Yahoo:
                    searchURL += "search.yahoo.com/search?p=";
                    break;
                case SearchEngine.Youtube:
                    searchURL += "www.youtube.com/results?search_query=";
                    break;
                default:
                    searchURL += "www.google.com/search?q=";
                    break;
            }

            if (e.KeyCode == Keys.ControlKey) isControlDown = true;

            if (e.KeyCode == Keys.Enter && (!string.IsNullOrEmpty(Text) && !string.IsNullOrWhiteSpace(Text)))
            {
                new SearchResults(Text, Parent);

                System.Diagnostics.Process.Start(searchURL + Text);
                Text = "";
                o.Show();
                o.BringToFront();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.ControlKey) isControlDown = false;
        }
    }

    public class SearchResults : Panel
    {
        public SearchResults(string q, Control Parent)
        {
            Size = new Size(Parent.Width - 100, Parent.Height - 200);
            Location = new Point(50, 50);
            Name = "SearchResults";

            Parent.Controls.Add(this);
            BringToFront();

            Tabs(q);
        }

        private void Tabs(string q)
        {
            string[] tabList = { "Web Results", "Documents", "Music", "Videos", "Pictures" };
            int left = 0;

            foreach(string Tab in tabList)
            {
                Label l = new Label();
                l.AutoSize = false;
                l.Size = new Size(150, 30);
                l.Location = new Point(left, 0);
                l.Text = Tab;
                l.TextAlign = ContentAlignment.MiddleCenter;
                l.BackColor = Color.Transparent;
                l.ForeColor = Parent.ForeColor;
                l.Font = new Font("Segoe UI", 14, FontStyle.Regular);
                l.Cursor = Cursors.Hand;
                l.MouseEnter += (se, ev) => { ((Label)se).BackColor = Color.FromArgb(100, 255, 255, 255); };
                l.MouseLeave += (se, ev) => { ((Label)se).BackColor = Color.Transparent; };
                l.MouseClick += (se, ev) =>
                {
                    foreach(Panel pa in Controls.OfType<Panel>())
                    {
                        if (pa.Name != ((Label)se).Text.ToLower().Replace(" ", ""))
                            pa.Hide();
                        else
                            pa.Show();
                    }
                };

                Controls.Add(l);

                left += l.Width;

                Panel p = new Panel();
                p.Name = Tab.ToLower().Replace(" ", "");
                p.Size = new Size(Width, Height - 30);
                p.Location = new Point(0, 30);
                p.BackColor = Color.FromArgb(new Random().Next(0, 255));

                Controls.Add(p);

                if(Tab != tabList[0])
                    p.Hide();

                switch(Tab)
                {
                    case "Web Results":
                        break;
                    case "Documents":
                        break;
                    case "Music":
                        GetMusic(q, p);
                        break;
                    case "Videos":
                        break;
                    case "Pictures":
                        break;
                }
            }
        }

        private void GetMusic(string q, Control Parent)
        {
            string[] Locations = Regedit.GetValue(Regedit.SubKey.Menu, "MusicLocations", "").Split(',');

            int left = 5, top = 5;

            foreach(FileInfo f in DAISy.Search.Music.Get(q, Locations))
            {
                Panel p = new Panel();
                p.Size = new Size(200, 100);
                p.Location = new Point(left, top);
                p.BackColor = Color.Transparent;

                Parent.Controls.Add(p);

                Label l = new Label();
                l.AutoSize = false;
                l.Size = p.Size;
                l.Location = new Point(0, 0);
                l.Text = Path.GetFileNameWithoutExtension(f.FullName);
                l.Font = new Font("Segoe UI", 12, FontStyle.Regular);
                l.TextAlign = ContentAlignment.MiddleCenter;

                p.Controls.Add(l);

                if (left + ((p.Width + 5) * 2) > Parent.Width)
                {
                    left = 5;
                    top += p.Height + 5;
                }
                else
                    left += p.Width + 5;

                if (top + p.Height >= Parent.Height) break;
            }
        }
    }
}
