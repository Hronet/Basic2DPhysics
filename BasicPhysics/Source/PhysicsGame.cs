using System.Numerics;
using static SDL2.SDL;

namespace BasicPhysics;

public class PhysicsGame
{
    private const int screenWidth = 1920;
    private const int screenHeight = 1080;

    private const int rectangleWidth = 200;
    private const int rectangleHeight = 200;
    private List<Shape2D> stationaryColliders = [];
    private Shape2D followMouseRectangle;
    private Shape2D stationaryRectangle;
    private Shape2D stationaryTriangle;
    private Shape2D randomShape;
    private readonly Dictionary<Shape2D, (bool colliding, Vector2 mtv)> collidingObjects = new();
    
    public PhysicsGame()
    {
        CreateShapes();
    }

    public void Update()
    {
        SDL_GetMouseState(out var mouseX, out var mouseY);
        followMouseRectangle.UpdateRectangle(mouseX, mouseY, rectangleWidth, rectangleHeight);
        foreach (var shape in stationaryColliders)
        {
            var colliding = SATCollision.IsColliding(followMouseRectangle, shape, out var mtv);
            collidingObjects[shape] = (colliding, mtv);
        }
    }

    public void Draw(IntPtr renderer)
    {
        foreach (var shape in stationaryColliders)
        {           
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
            if (collidingObjects[shape].colliding)
            {
                SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            }
            shape.Draw(renderer);
        }
        SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
        followMouseRectangle.Draw(renderer);
    }
    
    private void CreateShapes()
    {
        followMouseRectangle = Shape2D.CreateRectangle(0, 0, rectangleWidth , rectangleHeight);
        stationaryRectangle = Shape2D.CreateRectangle(screenWidth * 0.75f, screenHeight * 0.5f - rectangleHeight * 0.5f, rectangleWidth, rectangleHeight);
        stationaryTriangle = Shape2D.CreateRightTriangle(screenWidth * 0.5f, screenHeight * 0.5f, rectangleWidth, rectangleWidth);
        randomShape = Shape2D.GenerateRandomConvexPolygon(
            10, 
            screenWidth * 0.25f,
            screenHeight * 0.5f,
            rectangleWidth,
            25f);
        stationaryColliders =
        [
            stationaryRectangle,
            randomShape,
            stationaryTriangle
        ];
    }
}