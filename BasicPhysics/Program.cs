using static SDL2.SDL;

namespace BasicPhysics;

internal abstract class Program
{
    private static bool _quit = false;
    private static IntPtr _windowHandle;
    private static IntPtr _renderer;

    private static readonly PhysicsGame _physicsGame = new();
    
    private static void Main()
    {
        InitSDL();
        GameLoop(static () =>
        {
            HandleEvents();
            Update();
            Draw();
        });
        CleanUp();
    }

    private static void GameLoop(Action gameLoop)
    {
        while (!_quit)
        {
            gameLoop();
        }
    }

    private static void HandleEvents()
    {
        // Event handling
        while (SDL_PollEvent(out var e) != 0)
        {
            if (e.type == SDL_EventType.SDL_QUIT)
            {
                _quit = true;
            }
        }
    }

    private static void Update()
    {
        _physicsGame.Update();
    }

    private static void Draw()
    {
        // Clear screen
        SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
        SDL_RenderClear(_renderer);

        // Set draw color (red)
        _physicsGame.Draw(_renderer);

        // Update the screen
        SDL_RenderPresent(_renderer);
    }

    private static void CleanUp()
    {
        // Clean up
        SDL_DestroyRenderer(_renderer);
        SDL_DestroyWindow(_windowHandle);
        SDL_Quit();
    }

    private static void InitSDL()
    {
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine("SDL could not initialize! SDL_Error: {0}", SDL_GetError());
            return;
        }

        _windowHandle = SDL_CreateWindow("SDL2 Line Drawing in C#",
            SDL_WINDOWPOS_UNDEFINED,
            SDL_WINDOWPOS_UNDEFINED,
            1920, 1080,
            SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (_windowHandle == IntPtr.Zero)
        {
            Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL_GetError());
            SDL_Quit();
            return;
        }

        _renderer = SDL_CreateRenderer(_windowHandle, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        if (_renderer != IntPtr.Zero) return;
        
        Console.WriteLine("Renderer could not be created! SDL_Error: {0}", SDL_GetError());
        SDL_DestroyWindow(_windowHandle);
        SDL_Quit();
    }
}