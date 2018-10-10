using Fougerite;
using RustPP;
using System;
using RustPP.Commands;

public class rustpp : ConsoleSystem
{
    [Admin]
    public static void day(ref ConsoleSystem.Arg arg)
    {
        World.GetWorld().Time = 6f;
        arg.ReplyWith("☢ Good day.");
    }

    [Admin]
    public static void night(ref ConsoleSystem.Arg arg)
    {
        World.GetWorld().Time = 18f;
        arg.ReplyWith("☢ Good evening.");
    }

    [Admin]
    public static void savealldata(ref ConsoleSystem.Arg arg)
    {
        TimedEvents.savealldata();
        arg.ReplyWith("☢ Saved all Rust++ data.");
    }

    [Admin]
    public static void shutdown(ref ConsoleSystem.Arg arg)
    {
        ShutDownCommand.StartShutdown();
        arg.ReplyWith("☢ Initiating server shutdown in: " + ShutDownCommand.ShutdownTime + " second(s).");
        //TimedEvents.shutdown();
    }
}