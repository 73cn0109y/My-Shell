using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace MyShell.Menu
{
    public class Time
    {
        BackgroundWorker timeKeeper;
        Label time, date;

        public Time(Form Parent)
        {
            time = new Label();
            time.AutoSize = false;
            time.Size = new Size(200, 50);
            time.TextAlign = ContentAlignment.MiddleCenter;
            time.Location = new Point(Parent.Width / 2 - (time.Width / 2), Parent.Height / 2 - (time.Height / 2));
            time.Text = DateTime.Now.ToString("t");
            time.Font = new Font("Arial", 20, FontStyle.Regular);

            Parent.Controls.Add(time);

            date = new Label();
            date.AutoSize = false;
            date.Size = new Size(500, 50);
            date.TextAlign = ContentAlignment.TopCenter;
            date.Location = new Point(Parent.Width / 2 - (date.Width / 2), time.Location.Y + time.Height);
            date.Text = DateTime.Now.ToString("dddd d MMMM yyyy");
            date.Font = new Font("Arial", 12, FontStyle.Regular);

            Parent.Controls.Add(date);

            timeKeeper = new BackgroundWorker();
            timeKeeper.DoWork += timeKeeper_DoWork;
            timeKeeper.RunWorkerCompleted += timeKeeper_RunWorkerCompleted;
            timeKeeper.RunWorkerAsync();
        }

        private void timeKeeper_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; i++)
                System.Threading.Thread.Sleep(10);

            try
            {
                time.Invoke(new MethodInvoker(() =>
                {
                    time.Text = string.Format("{0:hh}{1}{0:mm} {0:tt}", DateTime.Now, time.Text.Contains(":") ? " " : ":");
                }));

                date.Invoke(new MethodInvoker(() =>
                {
                    date.Text = DateTime.Now.ToString("dddd d MMMM yyyy");
                }));
            }
            catch { }
        }

        private void timeKeeper_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timeKeeper.RunWorkerAsync();
        }
    }
}
