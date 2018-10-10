using System;
using System.Diagnostics;
using System.Timers;
using Fougerite;
using Fougerite.Events;

namespace RustPP.Commands
{
    public class ShutDownCommand : ChatCommand
    {
        internal static System.Timers.Timer _timer;
        internal static System.Timers.Timer _timer2;
        public static int ShutdownTime = 60;
        public static int TriggerTime = 10;
        internal static int Time = 0;

        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            if (ChatArguments.Length == 1)
            {
                if (ChatArguments[0] == "urgent")
                {
                    Fougerite.Hooks.IsShuttingDown = true;
                    Fougerite.Server.GetServer().BroadcastFrom(Core.Name, "☢ " + cyan + "Immediate server shutdown!");
                    //UnityEngine.Application.Quit();
                    Process.GetCurrentProcess().Kill();
                }
                else if (ChatArguments[0] == "safeurgent")
                {
                    Fougerite.Hooks.IsShuttingDown = true;
                    Fougerite.Server.GetServer().BroadcastFrom(Core.Name, yellow + "☢ " + green + "Saving all server files...");
                    ServerSaveManager.AutoSave();
                    Fougerite.Server.GetServer().BroadcastFrom(Core.Name, yellow + "☢ " + green + "Saved all server files!");
                    Fougerite.Server.GetServer().BroadcastFrom(Core.Name, "☢ Server is shutting down in: " + cyan + ShutdownTime + white + " seconds.");
                    _timer = new Timer(TriggerTime * 1000);
                    _timer.Elapsed += Trigger;
                    _timer.Start();
                }
                return;
            }
            StartShutdown();
        }


        public static void StartShutdown()
        {
            string cyan = "[color #00FFFF]";
            string white = "[color #FFFFFF]";
            try
            {
                ShutdownTime = int.Parse(Core.config.GetSetting("Settings", "shutdown_countdown"));
                TriggerTime = int.Parse(Core.config.GetSetting("Settings", "shutdown_trigger"));
            }
            catch
            {
                Logger.LogError("[RustPP] Failed to execute shutdown! Invalid config options!");
                return;
            }
            Fougerite.Hooks.IsShuttingDown = true;
            Fougerite.Server.GetServer().BroadcastFrom(Core.Name, "☢ Server is shutting down in: " + cyan + ShutdownTime + white + " seconds.");
            _timer = new Timer(TriggerTime * 1000);
            _timer.Elapsed += Trigger;
            _timer.Start();
        }

        internal static void Trigger(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Time += TriggerTime;
            string cyan = "[color #00FFFF]";
            string green = "[color #00FF00]";
            string yellow = "[color #FFFF00]";
            string white = "[color #FFFFFF]";
            if (Time >= ShutdownTime)
            {
                _timer.Dispose();
                Fougerite.Server.GetServer().BroadcastFrom(Core.Name, yellow + "☢ " + green + "Saving all server files...");
                ServerSaveManager.AutoSave();
                Fougerite.Server.GetServer().BroadcastFrom(Core.Name, yellow + "☢ " + green + "Saved all server files!");
                Fougerite.Server.GetServer().BroadcastFrom(Core.Name, "☢ Server is shutting down in: " + cyan + "15" + white + " seconds.");
                _timer2 = new Timer(15000);
                _timer2.Elapsed += Trigger2;
                _timer2.Start();
            }
            else
            {
                Fougerite.Server.GetServer().BroadcastFrom(Core.Name, "☢ Server is shutting down in: " + cyan + (ShutdownTime - Time) + white + " seconds.");
            }
        }

        internal static void Trigger2(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            string cyan = "[color #00FFFF]";
            Fougerite.Server.GetServer().BroadcastFrom(Core.Name, "☢ " + cyan + "Immediate server shutdown!");
            _timer2.Dispose();
            //Loom.QueueOnMainThread(UnityEngine.Application.Quit);
            //UnityEngine.Application.Quit();
            Process.GetCurrentProcess().Kill();
        }
    }
}
