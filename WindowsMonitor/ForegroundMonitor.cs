using System.Runtime.InteropServices;

namespace WindowsMonitor;

public class ForegroundMonitor
{
    private static Action<IntPtr, string>? OnForegroundChanged { get; set; }
    private IntPtr _hook;

    public ForegroundMonitor(Action<IntPtr, string>? onForegroundChanged)
    {
        OnForegroundChanged = onForegroundChanged;
    }

    public void Start()
    {
        _hook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
            WinEventProc, 0, 0, WINEVENT_OUTOFCONTEXT);
    }

    public void Stop()
    {
        UnhookWinEvent(_hook);
    }

    private const uint EVENT_SYSTEM_FOREGROUND = 3;
    private const uint WINEVENT_OUTOFCONTEXT = 0;

    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(
        uint eventMin,
        uint eventMax,
        IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc,
        uint idProcess,
        uint idThread,
        uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    private delegate void WinEventDelegate(
        IntPtr hWinEventHook,
        uint eventType,
        IntPtr hwnd,
        int idObject,
        int idChild,
        uint dwEventThread,
        uint dwmsEventTime);

    private static void WinEventProc(
        IntPtr hWinEventHook,
        uint eventType,
        IntPtr hwnd,
        int idObject,
        int idChild,
        uint dwEventThread,
        uint dwmsEventTime)
    {
        OnForegroundChanged?.Invoke(hwnd, WindowTitleReader.GetWindowTitle(hwnd));
    }
}