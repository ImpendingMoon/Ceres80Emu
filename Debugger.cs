namespace Ceres80Emu;

internal static class Debugger
{
    public static bool Paused { get; set; } = true;



    public static void PrintStartupText()
    {
        Console.Write(
            $"Ceres80Emu v{Constants.Version}\n" +
            $"(C) ImpendingMoon, 2025. Licensed under BSD 3-Clause\n" +
            $"\n" +
            $"Type 'help' for a list of available commands." +
            $"\n\n" +
            $"(Ceres80) "
        );
    }



    public static void ProcessInput()
    {
        if (!Console.KeyAvailable) { return; }

        string? line = Console.ReadLine();
        if (!string.IsNullOrEmpty(line))
        {
            string[] args = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                case "quit":
                {
                    Quit();
                    return; // Special case
                }
                case "help":
                {
                    PrintHelp();
                    break;
                }
                default:
                {
                    Console.WriteLine($"Unexpected argument: {args[i]}");
                    break;
                }
                }
            }
        }

        Console.Write("(Ceres80) ");
    }



    public static void RunFrame()
    {
        if (Paused) { return; } // ProcessInput handles single-stepping

        // Run a frame's worth of cycles
    }



    private static void PrintHelp()
    {
        Console.WriteLine(
            $"Ceres80 v{Constants.Version} list of commands:\n" +
            $"\n" +
            $"help\tPrint information about commands\n" +
            $"quit\tQuit Ceres80Emu\n"
        );
    }



    private static void Quit()
    {
        Program.InternalWindowShouldClose = true;
    }
}
