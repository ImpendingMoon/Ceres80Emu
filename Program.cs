using Raylib_cs;

namespace Ceres80Emu;

internal static class Program
{
    static RenderTexture2D target = Raylib.LoadRenderTexture(Constants.DisplayWidth, Constants.DisplayHeight);

    static void Main(string[] args)
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.SetTraceLogLevel(TraceLogLevel.Error);

        Window.InitWindow();

        while (!Raylib.WindowShouldClose())
        {
            Window.DrawWindow();
        }

        Window.FreeWindow();
    }
}

