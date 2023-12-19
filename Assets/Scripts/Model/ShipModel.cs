using UnityEngine;

public class ShipModel
{
    private const float Acceleration = 0.1f;
    private const float MaxVelocity = 2f;

    private Vector2 _position;
    public Vector2 Position => _position;

    private Vector2 _velocity;

    private Vector2 _angle;
    public Vector2 Angle => _angle;

    private readonly int _width;
    private readonly int _height;

    public ShipModel(Vector2 position, Vector2 velocity, Vector2 angle, int width, int height)
    {
        _position = position;
        _velocity = velocity;
        _angle = angle;
        _width = width;
        _height = height;
    }

    public void Update(float deltaTime)
    {
        _position += _velocity * deltaTime;
        _position = _position.Wrap(_width, _height);
    }

    public void Accelerate()
    {
        _velocity += _angle.normalized * Acceleration;

        _velocity = Vector2.ClampMagnitude(_velocity, MaxVelocity);
    }

    public void Decelerate()
    {
        _velocity -= _angle.normalized * Acceleration;

        _velocity = Vector2.ClampMagnitude(_velocity, MaxVelocity);
    }

    public void RotateLeft()
    {
        _angle = Quaternion.Euler(0, 0, 1) * _angle;
    }

    public void RotateRight()
    {
        _angle = Quaternion.Euler(0, 0, -1) * _angle;
    }
}
