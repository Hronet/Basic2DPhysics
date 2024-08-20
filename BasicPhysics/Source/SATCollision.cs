using System.Numerics;

namespace BasicPhysics;

public abstract class SATCollision
{
    private const int maxBufferSize = 10;
    private static readonly Vector2[] _axesBuffer1;
    private static readonly Vector2[] _axesBuffer2;

    static SATCollision()
    {
        _axesBuffer1 = new Vector2[maxBufferSize];
        _axesBuffer2 = new Vector2[maxBufferSize];
    }
    
    public static bool IsColliding(Shape2D shape1, Shape2D shape2, out Vector2 mtv)
    {
        mtv = Vector2.Zero;
        var minOverlap = float.MaxValue;

        // Get axes for shape1
        shape1.GetAxes(_axesBuffer1);

        // Check for collision on each axis of shape1
        if (!CheckCollisionOnAxes(shape1, shape2, _axesBuffer1, ref minOverlap, ref mtv))
        {
            return false;
        }

        // Get axes for shape2
        shape2.GetAxes(_axesBuffer2);

        // Check for collision on each axis of shape2
        return CheckCollisionOnAxes(shape1, shape2, _axesBuffer2, ref minOverlap, ref mtv);
    }

    private static bool CheckCollisionOnAxes(Shape2D shape1, Shape2D shape2, Vector2[] axes, ref float minOverlap, ref Vector2 mtv)
    {
        foreach (var axis in axes)
        {
            if (axis == Vector2.Zero)
            {
                break; // Exit early if we've reached the end of the used axes
            }

            shape1.Project(axis, out var min1, out var max1);
            shape2.Project(axis, out var min2, out var max2);

            // Calculate overlap
            var overlap = Math.Min(max1, max2) - Math.Max(min1, min2);
            if (overlap <= 0)
            {
                // No collision
                return false;
            }

            if (!(overlap < minOverlap)) continue;
            minOverlap = overlap;
            mtv = axis * overlap;
        }

        return true;
    }
}