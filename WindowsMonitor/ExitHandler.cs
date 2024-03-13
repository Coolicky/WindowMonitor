using System.Runtime.InteropServices;

namespace WindowsMonitor;

public static class ExitHandler
{
    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

    private delegate bool EventHandler(CtrlType sig);

    private enum CtrlType
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }

    public static void HandleAppExit()
    {
        SetConsoleCtrlHandler(Handler, true);
    }

    private static bool Handler(CtrlType sig)
    {
        switch (sig)
        {
            case CtrlType.CTRL_C_EVENT:
            case CtrlType.CTRL_CLOSE_EVENT:
            case CtrlType.CTRL_LOGOFF_EVENT:
            case CtrlType.CTRL_SHUTDOWN_EVENT:
            case CtrlType.CTRL_BREAK_EVENT:
                Environment.Exit(-1);
                return true;
            default:
                return false;
        }
    }
}