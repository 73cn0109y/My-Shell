using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace MyShell.SideBar
{
    class ProgramWatcher
    {
        private BackgroundWorker Watcher;
        private List<ProcessData> ActiveProcesses;

        public ProgramWatcher()
        {
            ActiveProcesses = new List<ProcessData>();

            Watcher = new BackgroundWorker();
            Watcher.DoWork += Watcher_DoWork;
            Watcher.RunWorkerCompleted += Watcher_RunWorkerCompleted;
            Watcher.RunWorkerAsync();
        }

        private void Watcher_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 50; i++)
            {
                System.Threading.Thread.Sleep(10);
                Application.DoEvents();
            }

            foreach(Process proc in Process.GetProcesses())
            {
                ProcessData d = new ProcessData() { Name = proc.MainWindowTitle, ID = proc.Id };
                if (proc.MainWindowTitle == "" || ActiveProcesses.Where(a=>a.ID == proc.Id).ToList().Count > 0 || proc.MainWindowTitle == "My Shell") continue;

                MousePanel p = new MousePanel(new RegistryValues() { Name = "OpenApp" + proc.MainWindowTitle, Value = proc.MainModule.FileName }, Global.Pin.Width, 0, 0, true);
                foreach(Control c in p.Controls)
                {
                    c.MouseClick += (se, ev) =>
                    {
                        Win32.WINDOWPLACEMENT placement = new Win32.WINDOWPLACEMENT();

                        Win32.GetWindowPlacement(proc.MainWindowHandle, ref placement);

                        ProcessInfo info = Global.ProcessInformation.Where(i => i.Path == proc.MainModule.FileName).First();

                        switch (placement.showCmd)
                        {
                            case 0:
                            case 2: // Minimized
                                if (placement.showCmd == 0)
                                    Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Show);
                                Win32.SetWindowPos(proc.MainWindowHandle, (IntPtr)0, info.Size.Width, info.Size.Height, info.Location.X, info.Location.Y, Win32.SWP.SHOWWINDOW);
                                Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Restore);
                                break;
                            case 1: // Normal
                            case 3: // Maximized
                                if (Win32.GetForegroundWindow() == proc.MainWindowHandle)
                                {
                                    Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Minimize);
                                    Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Hide);
                                }
                                else
                                    Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Show);
                                Global.UpdateProcessInfo(proc);
                                break;
                        }
                    };
                }

                p.MouseClick += (se, ev) =>
                {
                    Win32.WINDOWPLACEMENT placement = new Win32.WINDOWPLACEMENT();

                    Win32.GetWindowPlacement(proc.MainWindowHandle, ref placement);

                    ProcessInfo info = Global.ProcessInformation.Where(i => i.Path == proc.MainModule.FileName).First();

                    switch (placement.showCmd)
                    {
                        case 0:
                        case 2: // Minimized
                            if(placement.showCmd == 0)
                                Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Show);
                            Win32.SetWindowPos(proc.MainWindowHandle, (IntPtr)0, info.Size.Width, info.Size.Height, info.Location.X, info.Location.Y, Win32.SWP.SHOWWINDOW);
                            Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Restore);
                            break;
                        case 1: // Normal
                        case 3: // Maximized
                            if (Win32.GetForegroundWindow() == proc.MainWindowHandle)
                            {
                                Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Minimize);
                                Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Hide);
                            }
                            else
                                Win32.ShowWindow(proc.MainWindowHandle, (int)Win32.WindowShowStyle.Show);
                            Global.UpdateProcessInfo(proc);
                            break;
                    }
                };

                Global.SideBar.Invoke(new MethodInvoker(() => { Global.SideBar.Controls.Add(p); }));

                Reposition();

                ActiveProcesses.Add(d);
                Global.ProcessInformation.Add(new ProcessInfo() { Name = proc.ProcessName, Path = proc.MainModule.FileName, Location = default(Point), Size = default(Size) });
            }

            for(int i = 0; i < ActiveProcesses.Count; i++)
            {
                if(!Win32.ProcessExists(ActiveProcesses[i].ID))
                {
                    foreach(Control c in Global.SideBar.Controls)
                    {
                        if(c.Name == "OpenApp" + ActiveProcesses[i].Name)
                        {
                            Global.SideBar.Invoke(new MethodInvoker(() => { c.Dispose(); }));

                            ActiveProcesses.RemoveAt(i);
                            try { Global.UpdateProcessInfo(Process.GetProcesses().Where(idd => idd.Id == ActiveProcesses[i].ID).First()); } catch { }
                            Reposition();
                            break;
                        }
                    }
                }
            }
        }

        private void Reposition()
        {
            List<Control> Controls = new List<Control>();

            foreach (Control c in Global.SideBar.Controls)
                if (c.Name.StartsWith("OpenApp")) Controls.Add(c);

            int left = 0;
            int top = 0;

            top = (Global.Pin.Height / 2) - (80 * ((Controls.Count - 1) / 2));

            foreach (Control c in Controls)
            {
                Global.Pin.Invoke(new MethodInvoker(() => { c.Location = new Point(left, top); }));

                if (left > 0)
                {
                    left = 0;
                    top += c.Height + 5;
                }
                else
                    left += c.Width;
            }
        }

        private void Watcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Watcher.RunWorkerAsync();
        }
    }

    public class ProcessData
    {
        public string Name;
        public int ID;
    }

    public class HideAfterMinimized
    {
        System.Timers.Timer t;
        IntPtr i;

        public HideAfterMinimized(IntPtr i)
        {
            t = new System.Timers.Timer(500);

            t.Elapsed += (sender, e) =>
            {
                Win32.ShowWindow(i, (int)Win32.WindowShowStyle.Hide);
                t.Stop();
            };
            t.Start();
        }
    }
}
