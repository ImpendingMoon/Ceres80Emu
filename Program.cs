using Raylib_cs;

namespace Ceres80Emu;

internal class Program
{
    const int lcdWidth = 128;
    const int lcdHeight = 64;
    static RenderTexture2D target = Raylib.LoadRenderTexture(lcdWidth, lcdHeight);

    static void Main(string[] args)
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(512, 256, "Ceres80Emu");
        Raylib.SetTargetFPS(60);
        Raylib.SetWindowMinSize(128, 64);

        while (!Raylib.WindowShouldClose())
        {
            float scale = Math.Min(
                (float)Raylib.GetScreenWidth() / lcdWidth,
                (float)Raylib.GetScreenHeight() / lcdHeight
            );

            Raylib.BeginTextureMode(target);

            Raylib.ClearBackground(Color.Blue);
            Raylib.DrawText("Hello, Ceres80!", 1, 1, 8, Color.White);
            Raylib.DrawText($"Current Scale: {scale}", 1, 9, 8, Color.White);

            Raylib.EndTextureMode();

            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.Black);
            Raylib.DrawTexturePro(
                target.Texture,
                new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
                new Rectangle(
                    (Raylib.GetScreenWidth() - lcdWidth * scale) / 2,
                    (Raylib.GetScreenHeight() - lcdHeight * scale) / 2,
                    lcdWidth * scale,
                    lcdHeight * scale
                ),
                new System.Numerics.Vector2(0, 0),
                0,
                Color.White
            );

            Raylib.EndDrawing();
        }

        Raylib.UnloadRenderTexture(target);
        Raylib.CloseWindow();
    }
}

