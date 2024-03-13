using System.Collections.Concurrent;

namespace WindowsMonitor;

public static class Program
{
    private static readonly ConcurrentDictionary<IntPtr, string> Windows = new();

    private static string _searchTerm = "Hello World!";
    private static string _replaceTerm = "Bingo";
    private static bool _caseSensitive;
    private static Action<IntPtr, string>? _callback;

    public static void Main()
    {
        ExitHandler.HandleAppExit();

        //There would be pasting allowed in the console but skipped for brevity
        GetSearchTerm();
        GetReplacementTerm();
        GetCaseSensitivity();

        Console.WriteLine("Press 'q' to stop the monitor when ready.");

        var listenToKeyPressThread = new Thread(() =>
        {
            while (true)
            {
                if (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Q)
                    continue;
                Application.Exit();
                break;
            }
        });
        listenToKeyPressThread.Start();

        _callback = (_, _) => { };
        var monitor = new ForegroundMonitor((handle, title) =>
        {
            _callback.Invoke(handle, title);
        });
        monitor.Start();

        var addOrUpdateThread = new Thread(() =>
        {
            _callback += AddOrUpdateWindow;
        });
        var replaceThread = new Thread(() =>
        {
            _callback += Replace;
        });
        addOrUpdateThread.Start();
        replaceThread.Start();

        Application.Run();

        monitor.Stop();
        listenToKeyPressThread.Join();
        addOrUpdateThread.Join();
        replaceThread.Join();

        SaveToFileIfRequired();
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    private static void AddOrUpdateWindow(IntPtr handle, string title)
    {
        Console.WriteLine(title);
        Windows.AddOrUpdate(handle, title, (_, _) => title);
    }

    private static void Replace(IntPtr handle, string title)
    {
        if (!title.Contains(_searchTerm, _caseSensitive
                ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
            return;
        Console.WriteLine("Found a match!");

        var newTitle = title.Replace(_searchTerm, _replaceTerm, _caseSensitive
            ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        Windows.AddOrUpdate(handle, newTitle, (_, _) => newTitle);
    }

    private static void GetCaseSensitivity()
        => _caseSensitive = GetYesNoInput("Should the search be case sensitive?", _caseSensitive);

    private static void GetReplacementTerm()
    {

        Console.WriteLine($"Please pass a replacement term as an argument to the program or " +
                          $"press enter to use the default replacement term '{_replaceTerm}'");
        var input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
            _replaceTerm = input;
    }

    private static void GetSearchTerm()
    {
        Console.WriteLine($"Please pass a search term as an argument to the program or " +
                          $"press enter to use the default search term '{_searchTerm}'");
        var input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
            _searchTerm = input;
    }

    private static void SaveToFileIfRequired()
    {
        var shouldSave = GetYesNoInput("Do you want to save the window titles to a file?", true);
        if (!shouldSave) return;

        var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var path = Path.Combine(userDir, "WindowsTitles.txt");

        Console.WriteLine($"Please pass a file path to save the window titles to or " +
                          $"press enter to use the default path '{path}'.");
        var input = Console.ReadLine();
        //There should be prevention of invalid characters
        //As well as checking if the path is valid but skipped for brevity
        if (!string.IsNullOrWhiteSpace(input))
            _searchTerm = input;

        if (File.Exists(path))
        {
            var shouldOverwrite = GetYesNoInput("The file already exists, do you want to overwrite it?", true);
            if (!shouldOverwrite) return;
        }

        var lines = Windows.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToArray();
        File.WriteAllLines(path, lines);
        Console.WriteLine($"Window titles have been saved to {path}");
    }

    private static bool GetYesNoInput(string message, bool defaultValue)
    {
        var defaultYesNo = defaultValue ? 'y' : 'n';
        Console.WriteLine($"{message} (y/n) | Default is '{defaultYesNo}'");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return defaultValue;
        while (input.ToLower() != "y" && input.ToLower() != "n")
        {
            Console.WriteLine($"Invalid input, please enter 'y' or 'n' or press enter to use the default value: {defaultYesNo}.");
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return defaultValue;
        }

        return !string.IsNullOrWhiteSpace(input) && input.ToLower() == "y";
    }
}