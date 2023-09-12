using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing;
using System.Timers;

class PalmTreeTime
{
    static System.Timers.Timer checkTimer;
    static int maxTime;
    static DateTime lastTimeUsed;
    static DateTime currentTime;
    static bool debug = false;

    static void Main(string[] args)
    {
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
    }

    static void Init()
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
                Shutdown(true, true);
                return;
            }
        }
        checkTimer = new System.Timers.Timer(60000);
        checkTimer.Elapsed += Timer;
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

    static void Timer(Object source, ElapsedEventArgs e)
    {
        Console.WriteLine("Did a check.");
        if (currentTime >= lastTimeUsed.AddMinutes(maxTime))
            {
                Shutdown(true);
                return;
            }
    }
    static void Shutdown(bool withWarning, bool now = false)
    {
        if (debug)
        {
            Process.Start("notepad.exe");
            return;
        }

        if (withWarning)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("ZEIT ABGELAUFEN");
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Speichere dein Spiel und drücke Enter.");
            Console.ReadLine();
        }
        Process.Start("shutdown.exe /t 0");
        
    }

}