using System.Numerics;
using static SDL2.SDL;

namespace BasicPhysics;

public readonly record struct Shape2D(Vector2[] points)
{
    private Vector2[] Points { get; } = points;
    public int PointsCount => Points.Length;
    
    public Vector2 Center
    {
        get
        {
            float xSum = 0, ySum = 0;
            foreach (var vertex in Points)
            {
                xSum += vertex.X;
                ySum += vertex.Y;
            }
            return new(xSum / Points.Length, ySum / Points.Length);
        }
    }

    // Factory method for creating a rectangle
    public static Shape2D CreateRectangle(float x, float y, float width, float height)
    {
        var points = new Vector2[4];
        points[0] = new(x, y);                   // Top-left
        points[1] = new(x + width, y);           // Top-right
        points[2] = new(x + width, y + height);  // Bottom-right
        points[3] = new(x, y + height);          // Bottom-left
        return new(points);
    }

    // Factory method for creating an isosceles right triangle
    public static Shape2D CreateRightTriangle(float x, float y, float baseLength, float height)
    {
        var points = new Vector2[3];
        points[0] = new(x, y);                         // Right-angle vertex
        points[1] = new(x + baseLength, y);            // Base right vertex
        points[2] = new(x, y - height);                // Height vertex
        return new(points);
    }

    // Factory method for creating a custom polygon (e.g., triangle, pentagon, etc.)
    public static Shape2D CreatePolygon(params Vector2[] points)
    {
        return new(points);
    }
    
    public static Shape2D GenerateRandomConvexPolygon(int pointCount, float centerX, float centerY, float radius, float perturbation)
    {
        if (pointCount < 3)
        {
            throw new ArgumentException("Polygon must have at least 3 points.");
        }

        var points = new List<Vector2>();
        var rand = new Random();

        for (var i = 0; i < pointCount; i++)
        {
            // Generate random angle for point on the circle
            var angle = i * 2 * Math.PI / pointCount;

            // Perturb the radius slightly to introduce randomness
            var r = radius + (float)(rand.NextDouble() * 2 * perturbation - perturbation);

            // Calculate the x and y based on angle and perturbed radius, around the center
            var x = centerX + (float)(r * Math.Cos(angle));
            var y = centerY + (float)(r * Math.Sin(angle));

            points.Add(new(x, y));
        }

        // No need to sort points, they are naturally ordered
        return new Shape2D(points.ToArray());
    }

    // Method to update the rectangle's dimensions
    public void UpdateRectangle(float x, float y, float width, float height)
    {
        Points[0] = new(x, y);                   // Top-left
        Points[1] = new(x + width, y);           // Top-right
        Points[2] = new(x + width, y + height);  // Bottom-right
        Points[3] = new(x, y + height);          // Bottom-left
    }

    // Method to update the right triangle's dimensions
    public void UpdateRightTriangle(float x, float y, float baseLength, float height)
    {
        Points[0] = new(x, y);                         // Right-angle vertex
        Points[1] = new(x + baseLength, y);            // Base right vertex
        Points[2] = new(x, y - height);                // Height vertex
    }
    
    // Function to generate axes for SAT (normals of edges)
    public void GetAxes(Vector2[] axes)
    {
        if (axes.Length < Points.Length)
        {
            throw new ArgumentException("The axes array must be at least as long as the number of edges.");
        }

        for (var i = 0; i < Points.Length; i++)
        {
            var edge = Points[(i + 1) % Points.Length] - Points[i];
            axes[i] = new(-edge.Y, edge.X); // Perpendicular vector (normal)
            axes[i] = Vector2.Normalize(axes[i]); // Normalize the axis vector
        }
    }

    // Function to project the shape onto an axis
    public void Project(Vector2 axis, out float min, out float max)
    {
        var dotProduct = Vector2.Dot(axis, Points[0]);
        min = max = dotProduct;

        for (var i = 1; i < Points.Length; i++)
        {
            dotProduct = Vector2.Dot(axis, Points[i]);
            if (dotProduct < min) min = dotProduct;
            if (dotProduct > max) max = dotProduct;
        }
    }
    
    public Vector2 this[int index]
    {
        get
        {
            if (index < 0 || index >= Points.Length)
            {
                throw new IndexOutOfRangeException("Index is out of range.");
            }
            return Points[index];
        }
        set
        {
            if (index < 0 || index >= Points.Length)
            {
                throw new IndexOutOfRangeException("Index is out of range.");
            }
            Points[index] = value;
        }
    }

    public void Draw(IntPtr renderer)
    {
        for (var i = 0; i < PointsCount - 1; i++)
        {
            SDL_RenderDrawLine(renderer, 
                (int)points[i].X, (int)points[i].Y, 
                (int)points[i+1].X, (int)points[i+1].Y);
        }
        SDL_RenderDrawLine(renderer, 
            (int)points[0].X, (int)points[0].Y, 
            (int)points[PointsCount-1].X, (int)points[PointsCount-1].Y);
    }
}