using Raylib_cs;

namespace Ceres80Emu;

internal static class Program
{
    public static bool InternalWindowShouldClose { get; set; } = false;

    static void Main(string[] args)
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.SetTraceLogLevel(TraceLogLevel.Error);

        Window.InitWindow();

        Debugger.PrintStartupText();

        while (!Raylib.WindowShouldClose() && !InternalWindowShouldClose)
        {
            Debugger.ProcessInput();
            Window.DrawWindow();
        }

        Window.FreeWindow();
    }
}
