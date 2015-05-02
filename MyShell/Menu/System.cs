using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MyShell.Menu
{
    public class CPU : Panel
    {
        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        BackgroundWorker t;
        float UpdateCPU;
        Label l;

        public CPU(Form Parent)
        {
            t = new BackgroundWorker();
            t.DoWork += t_DoWork;
            t.RunWorkerCompleted += T_RunWorkerCompleted;
            t.RunWorkerAsync();

            Size = new Size(100, 100);
            Location = new Point(10, Parent.Height - Height - 10);
            BackColor = Color.FromArgb(255, 30, 30, 30);
            Paint += (sender, e) =>
            {
                using (Graphics g = e.Graphics)
                {
                    g.Clear(BackColor);

                    using (Pen color = new Pen(Color.FromArgb(255, 100, 100, 100), 3))
                    {
                        Rectangle rect = new Rectangle(1, 1, Width - (int)color.Width, Height - (int)color.Width);

                        float startAngle = 260.0f;
                        float sweepAngle = UpdateCPU * 4;
                        sweepAngle = sweepAngle < 1 ? 1 : sweepAngle;

                        g.DrawArc(color, rect, startAngle, sweepAngle);
                    }
                }

                l.Text = ((int)UpdateCPU).ToString() + "%";
            };

            l = new Label();
            l.AutoSize = false;
            l.Size = Size;
            l.Location = new Point(0, 0);
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.BackColor = Color.Transparent;
            l.ForeColor = Color.FromArgb(255, 238, 238, 238);
            l.Text = UpdateCPU.ToString();
            l.Font = new Font("Segoe UI", 16, FontStyle.Regular);

            Controls.Add(l);

            Parent.Controls.Add(this);
        }

        private void T_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            t.RunWorkerAsync();
        }

        private void t_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateCPU = cpuCounter.NextValue();
            
            for(int i = 0; i < 25; i++)
                System.Threading.Thread.Sleep(10);

            UpdateCPU = cpuCounter.NextValue();

            Global.Menu.Invoke(new MethodInvoker(() =>
            {
                Invalidate();
                Update();
            }));

            for(int i = 0; i < 25; i++)
                System.Threading.Thread.Sleep(10);
        }
    }

    public class RAM : Panel
    {
        System.Timers.Timer t;
        Label l;

        public RAM(Form Parent)
        {
            Size = new Size(100, 100);
            Location = new Point(120, Parent.Height - Height - 10);
            BackColor = Color.FromArgb(255, 30, 30, 30);
            Paint += (sender, e) =>
            {
                e.Graphics.Clear(BackColor);

                using (Pen color = new Pen(Color.FromArgb(255, 100, 100, 100), 3))
                {
                    Rectangle rect = new Rectangle(1, 1, Width - (int)color.Width, Height - (int)color.Width);

                    float startAngle = 260.0f;
                    float sweepAngle = ((float)GetRAM() * 4); // Multiply to create a full circle, (1 - 90, 2 - 180, 3 - 270, 4 - 360)
                    sweepAngle = sweepAngle < 1 ? 1 : sweepAngle;

                    e.Graphics.DrawArc(color, rect, startAngle, sweepAngle);
                }

                l.Text = ((int)GetRAM()).ToString() + "%";
            };

            l = new Label();
            l.AutoSize = false;
            l.Size = Size;
            l.Location = new Point(0, 0);
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.BackColor = Color.Transparent;
            l.ForeColor = Color.FromArgb(255, 238, 238, 238);
            l.Text = GetRAM().ToString();
            l.Font = new Font("Segoe UI", 16, FontStyle.Regular);

            Controls.Add(l);

            Parent.Controls.Add(this);

            t = new System.Timers.Timer(100);
            t.Elapsed += (sender, e) => { if (Global.Menu == null) return; try { Global.Menu.Invoke(new MethodInvoker(() => { Invalidate(); Update(); })); } catch { } };
            t.Start();
        }

        private static double GetRAM()
        {
            double a = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            double t = PerformanceInfo.GetTotalMemoryInMiB();
            double u = (t - a);

            return u / t * 100;
        }
    }

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static long GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            else
                return -1;
        }

        public static long GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            else
                return -1;
        }
    }
}
