using Raylib_cs;

namespace Ceres80Emu;

// This could be a singleton, but Raylib throws CLR errors when used inside Lazy<T>
public static class Window
{
    private static RenderTexture2D _targetTexture;

    public static bool IsInitialized { get; private set; } = false;

    public static void InitWindow()
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException("Window is already initialized");
        }

        Raylib.InitWindow(Constants.DisplayWidth * 4, Constants.DisplayHeight * 4, "Ceres80Emu");
        Raylib.SetTargetFPS(60);
        Raylib.SetWindowMinSize(Constants.DisplayWidth, Constants.DisplayHeight);
        _targetTexture = Raylib.LoadRenderTexture(Constants.DisplayWidth, Constants.DisplayHeight);
        IsInitialized = true;
    }



    public static void FreeWindow()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("Window is not initialized");
        }

        Raylib.UnloadRenderTexture(_targetTexture);
        Raylib.CloseWindow();
        IsInitialized = false;
    }



    public static void DrawWindow()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("Window is not initialized");
        }

        float scale = Math.Min(
            (float)Raylib.GetScreenWidth() / Constants.DisplayWidth,
            (float)Raylib.GetScreenHeight() / Constants.DisplayHeight
        );

        Raylib.BeginTextureMode(_targetTexture);

        Raylib.ClearBackground(Color.Blue);
        Raylib.DrawText("Hello, Ceres80!", 1, 1, 8, Color.White);
        Raylib.DrawText($"Current Scale: {scale}", 1, 9, 8, Color.White);

        Raylib.EndTextureMode();

        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.Black);
        Raylib.DrawTexturePro(
            _targetTexture.Texture,
            new Rectangle(0, 0, _targetTexture.Texture.Width, -_targetTexture.Texture.Height),
            new Rectangle(
                (Raylib.GetScreenWidth() - Constants.DisplayWidth * scale) / 2,
                (Raylib.GetScreenHeight() - Constants.DisplayHeight * scale) / 2,
                Constants.DisplayWidth * scale,
                Constants.DisplayHeight * scale
            ),
            new System.Numerics.Vector2(0, 0),
            0,
            Color.White
        );

        Raylib.EndDrawing();
    }
}
