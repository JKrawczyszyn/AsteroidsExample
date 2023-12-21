using UnityEngine;

public static class Extensions
{
    public static Vector2 Wrap(this Vector2 value, float xSize, float ySize)
    {
        return new Vector2(value.x.Wrap(xSize), value.y.Wrap(ySize));
    }

    public static Vector2Int Wrap(this Vector2Int value, int xSize, int ySize)
    {
        return new Vector2Int(value.x.Wrap(xSize), value.y.Wrap(ySize));
    }

    public static int Wrap(this int value, int size)
    {
        while (value < 0)
        {
            value += size;
        }

        while (value >= size)
        {
            value -= size;
        }

        return value;
    }

    public static float Wrap(this float value, float size)
    {
        while (value < 0)
        {
            value += size;
        }

        while (value >= size)
        {
            value -= size;
        }

        return value;
    }

    public static bool Between(this float value, float min, float max)
    {
        return value >= min && value <= max;
    }
}
