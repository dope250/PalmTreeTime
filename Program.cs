using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing;
using System.Timers;
using System.Runtime.InteropServices;

class PalmTreeTime
{
    private const int MF_BYCOMMAND = 0x00000000;
    public const int SC_CLOSE = 0xF060;

    [DllImport("user32.dll")]
    public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

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

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow([In] IntPtr hWnd, [In] int nCmdShow);
    private const int SW_SHOWNORMAL = 1;
    private const int SW_MINIMIZE = 6;

    static System.Timers.Timer checkTimer = new System.Timers.Timer(60000);
    static int maxTime;
    static DateTime lastTimeUsed;
    static DateTime currentTime;
    static bool debug = false;

    public static void Main(string[] args)
    {
        DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
        ShowWindow(GetConsoleWindow(), SW_MINIMIZE);

        if (args.Contains("debug"))
        {
            debug = true;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("NOT ARMED!");
            Console.ResetColor();
        }

        Console.WriteLine("Ready!");
        Init();
        Console.ReadLine();
    }

    public static void Init()
    {
        maxTime = ReadReg<int>("TimeContingent");
        lastTimeUsed = DateTime.Parse(ReadReg<string>("LastTimeRun"));
        currentTime = DateTime.Now;
        if (currentTime.Date != lastTimeUsed.Date)
        {
            WriteReg("LastTimeRun", currentTime.ToString());
            lastTimeUsed = DateTime.Parse(ReadReg<string>("LastTimeRun"));
        }
        else
        {
            if (currentTime >= lastTimeUsed.AddMinutes(maxTime))
            {
                Shutdown(true);
                return;
            }
        }

        checkTimer.Elapsed += OnTimedEvent;
        checkTimer.AutoReset = true;
        checkTimer.Enabled = true;
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

    static void WriteReg(string registryName, string valueToWrite)
    {
        RegistryKey PTTKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\electronic Ping [eP]\PalmTreeTime", true);
        if (PTTKey != null)
        {
            PTTKey.SetValue(registryName, valueToWrite);
        }
        return;
    }

    static void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        Console.Write(".");
        if (DateTime.Now >= lastTimeUsed.AddMinutes(maxTime))
        {
            Shutdown(true);
        }
    }
    static void Shutdown(bool withWarning)
    {
        ShowWindow(GetConsoleWindow(), SW_SHOWNORMAL);
        SetWindowPos(GetConsoleWindow(), new IntPtr(HWND_TOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

        if (withWarning)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("ZEIT ABGELAUFEN");
            Console.WriteLine(DateTime.Now.ToString());
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Speichere dein Spiel und drücke Enter.");
            Console.ReadLine();
        }

        if (debug)
        {
            Process.Start("notepad.exe");
        }
        else
        {
            Process.Start("shutdown.exe", "/s /t 0");
        }
    }

}