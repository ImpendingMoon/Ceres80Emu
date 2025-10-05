using Raylib_cs;

namespace Ceres80Emu;

public static class Program
{
    public static bool publicWindowShouldClose { get; set; } = false;

    static void Main(string[] args)
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.SetTraceLogLevel(TraceLogLevel.Error);

        Window.InitWindow();

        Debugger.PrintStartupText();

        while (!Raylib.WindowShouldClose() && !publicWindowShouldClose)
        {
            Debugger.ProcessInput();
            Window.DrawWindow();
        }

        Window.FreeWindow();
    }
}
