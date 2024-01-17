using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing;
using System.Timers;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace PalmTreeTime
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        DispatcherTimer checkTimer = new DispatcherTimer();
        int maxTime;
        int bonusTime;
        int tick = 0;
        DateTime lastTimeUsed;
        DateTime startedOn;
        DateTime currentTime;
        bool debug = false;
        bool ExitPossible = false;

        public MainWindow()
        {
            InitializeComponent();

            string[] args = new string[Environment.GetCommandLineArgs().Length];
            args = Environment.GetCommandLineArgs();

            if (args.Contains("debug"))
            {
                debug = true;
                WriteStatusText("NOT ARMED", Colors.Transparent, Colors.DarkGreen);
            }
            else
            {
                WriteStatusText("Ready!", Colors.Transparent, Colors.DarkGreen);
            }

            Init();
        }

        public void Init()
        {
            txt_Log.Content = "";
            startedOn = DateTime.Now;
            maxTime = ReadReg<int>("TimeContingent");
            
            bonusTime = ReadReg<int>("BonusContingent");
            if (DateTime.Now.DayOfWeek.ToString() == "Saturday" || DateTime.Now.DayOfWeek.ToString() == "Sunday")
            {
                maxTime += bonusTime;
            }

            lastTimeUsed = DateTime.Parse(ReadReg<string>("LastTimeRun"));
            currentTime = DateTime.Now;
            if (currentTime.Date != lastTimeUsed.Date)
            {
                WriteReg("LastTimeRun", currentTime.ToString());
                lastTimeUsed = DateTime.Parse(ReadReg<string>("LastTimeRun"));
                WriteReg("Tick", 0);
                tick = 0;
            }
            else
            {
                tick = ReadReg<int>("Tick");
                maxTime -= tick;
                if (currentTime >= lastTimeUsed.AddMinutes(maxTime))
                {
                    Shutdown(true);
                    return;
                }
            }

            checkTimer.Interval = TimeSpan.FromSeconds(60);
            checkTimer.Tick += checkTimer_Tick;
            checkTimer.Start();
        }

        static dynamic ReadReg<T>(string RegistryName)
        {
            RegistryKey PTTKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\electronic Ping [eP]\PalmTreeTime");

            if (typeof(T) == typeof(int))
            {
                if (PTTKey != null)
                {
                    if (PTTKey.GetValue(RegistryName) != null)
                    {
                        return Convert.ToInt32(PTTKey.GetValue(RegistryName));
                    }
                    return 0;
                }
                return 0;
            }
            else if (typeof(T) == typeof(string))
            {
                if (PTTKey != null)
                {
                    if (PTTKey.GetValue(RegistryName) != null)
                    {
                        return PTTKey.GetValue(RegistryName);
                    }
                    return false;
                }
                return false;
            }
            else
            {
                return false;
            }


        }

        static void WriteReg(string registryName, object valueToWrite)
        {
            RegistryKey PTTKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\electronic Ping [eP]\PalmTreeTime", true);
            if (PTTKey != null)
            {
                PTTKey.SetValue(registryName, valueToWrite);
            }
            return;
        }

        void checkTimer_Tick(object source, EventArgs e)
        {
            WriteLogText(".", Colors.Black);
            tick += 1;
            WriteReg("Tick", tick);

            if (DateTime.Now >= startedOn.AddMinutes(maxTime))
            {
                Shutdown(true);
            }
        }
        void Shutdown(bool withWarning)
        {
            if (wnd_main.WindowState == WindowState.Minimized) { wnd_main.WindowState = WindowState.Normal; }
                        
            //ShowWindow(GetConsoleWindow(), SW_SHOWNORMAL);
            //SetWindowPos(GetConsoleWindow(), new IntPtr(HWND_TOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

            if (withWarning)
            {
                WriteStatusText("ZEIT ABGELAUFEN!", Colors.White, Colors.Red);
                WriteLogText("Zeit Kontingent: " + maxTime.ToString(), Colors.Black);
                WriteLogText("Speichere dein Spiel und schließe dieses Fenster.", Colors.Red);

                if (debug)
                {
                    Process.Start("notepad.exe");
                }
                else
                {
                    ExitPossible = true;
                }
            }
            //Shutdown without Warning needed?

        }

        void WriteStatusText(string text, System.Windows.Media.Color BackgroundColor, System.Windows.Media.Color ForegroundColor)
        {
            txt_Status.Background = new SolidColorBrush(BackgroundColor);
            txt_Status.Foreground = new SolidColorBrush(ForegroundColor);
            txt_Status.Text = text;
        }

        void WriteLogText(string text, System.Windows.Media.Color ForegroundColor)
        {
            txt_Log.Foreground = new SolidColorBrush(ForegroundColor);
            txt_Log.Content += DateTime.Now.ToString() + " - " + text + "\n";
            txt_Log.ScrollToBottom();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (ExitPossible)
            {
                e.Cancel = false;
                Process.Start("shutdown.exe", "/s /t 0");
            }
            else
            {
                e.Cancel = true;
                WriteLogText("Tried to exit", Colors.Black);
            }
            
        }

    }
}
