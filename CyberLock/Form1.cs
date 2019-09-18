using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime;

namespace CyberLock
{
    public partial class Form1 : Form
    {
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        private Rectangle ScreenSize = Screen.PrimaryScreen.WorkingArea;
        private int ScreenX;
        private int ScreenY;

        private bool BypassQuit = false;

        [DllImport("user32.dll",EntryPoint = "SetForegroundWindow")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll",EntryPoint = "SetThreadExecutionState")]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ScreenX = ScreenSize.Width;
            ScreenY = ScreenSize.Height;
            info_text.Location = new Point(ScreenX / 3, ScreenY / 3);
            this.WindowState = FormWindowState.Maximized;
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(Update);
            aTimer.Interval = 10;
            aTimer.Enabled = true;
        }

        private void Update(object source, ElapsedEventArgs e)
        {
            
            PreventSleep();
            try
            {
                if (Form.ActiveForm != this)
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        SetForegroundWindow(this.Handle);
                    }));
                }
            }
            catch
            {

            }
            if(Cursor.Position != new Point(ScreenX / 2, ScreenY / 2))
            {
                Cursor.Position = new Point(ScreenX / 2, ScreenY / 2);
                Cursor.Hide();
            }
            try
            {
                info_text.BeginInvoke(new MethodInvoker(() =>
                {
                    info_text.Location = new Point(ScreenX / 4, ScreenY / 3);
                }));
            }
            catch
            {

            }
            var processes = Process.GetProcesses();
            foreach(var Process in processes)
            {
                var Blocked = Properties.Resources.BlockedProcesses.Split(Convert.ToChar(","));
                foreach (var Block in Blocked)
                {
                    if(Process.ProcessName == Block)
                    {
                        try
                        {
                            Console.WriteLine("Killing process \"" + Process.ProcessName + "\", ID: " + Process.Id);
                            Process.Kill();
                        }
                        catch(Exception a)
                        {
                            Console.WriteLine(a.Message);
                            if(a.Message == "Access is denied")
                            {
                                Console.WriteLine("Acces denied, Using hide method");
                                var processess = Process.GetProcesses();
                                foreach (var Processs in processess)
                                {
                                    try
                                    {
                                        if (Processs.PriorityClass != ProcessPriorityClass.Normal)
                                        {
                                            SetForegroundWindow(Processs.Handle);
                                            Console.WriteLine("Setting process\"" + Processs.ProcessName + "\" to front");
                                        }
                                    }
                                    catch(Exception b)
                                    {
                                        
                                    }
                                }
                                BeginInvoke(new MethodInvoker(() =>
                                {
                                    SetForegroundWindow(this.Handle);
                                }));
                                //SetForegroundWindow(this.Handle);
                                Console.WriteLine("Finishing by setting this to front");
                            }
                        }
                    }
                }
            }
        }

        private void Close(object sender, FormClosingEventArgs e)
        {
            if (BypassQuit)
            {
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
                SetMsg("LoL! Nice try.");
            }
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.H && e.Shift)
            {
                BypassQuit = true;
                Application.Exit();
            }
        }

        private void SetMsg(string text)
        {
            info_text.BeginInvoke(new MethodInvoker(() =>
            {
                info_text.Text = text;
            }));
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(ResetMsg);
            aTimer.Interval = 3000;
            aTimer.Enabled = true;
        }

        private void ResetMsg(object source, ElapsedEventArgs e)
        {
            info_text.BeginInvoke(new MethodInvoker(() =>
            {
                info_text.Text = "CYBERLOCK IS ACTIVE";
            }));
        }

        void PreventSleep()
        {
            // Prevent Idle-to-Sleep (monitor not affected) (see note above)
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
            Console.WriteLine("Preventing Sleep");
        }

    }
}
