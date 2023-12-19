using UnityEngine;

public record AsteroidModel(int Id)
{
    public Vector2 LocalPosition;
    public Vector2 Velocity;

    public AsteroidModel(int id, Vector2 localPosition, Vector2 velocity) : this(id)
    {
        LocalPosition = localPosition;
        Velocity = velocity;
    }
}
