using System.Runtime.InteropServices;

namespace WindowsMonitor;

public static class WindowTitleReader
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    public static string GetWindowTitle(IntPtr handle) => GetTitle(handle);

    public static string GetActiveWindowTitle() => GetTitle(GetForegroundWindow());

    private static string GetTitle(IntPtr handle)
    {
        const int nChars = 256;
        var builder = new System.Text.StringBuilder(nChars);
        return GetWindowText(handle, builder, nChars) > 0 ? builder.ToString() : "";
    }
}