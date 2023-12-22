using UnityEngine;

namespace Controllers
{
    public class ShipController
    {
        private Vector2 _position;
        public Vector2 Position => _position;

        private Vector2 _velocity;

        private Vector2 _angle;
        public Vector2 Angle => _angle;

        private readonly int _width;
        private readonly int _height;

        private readonly float _acceleration;
        private readonly float _maxVelocity;
        private readonly float _rotation;

        public ShipController(Vector2 position, Vector2 velocity, Vector2 angle, int width, int height, float acceleration,
            float maxVelocity, float rotation)
        {
            _position = position;
            _velocity = velocity;
            _angle = angle;
            _width = width;
            _height = height;
            _acceleration = acceleration;
            _maxVelocity = maxVelocity;
            _rotation = rotation;
        }

        public void Update(float deltaTime)
        {
            _position += _velocity * deltaTime;
            _position = _position.Wrap(_width, _height);
        }

        public void Accelerate()
        {
            _velocity += _angle.normalized * _acceleration;

            _velocity = Vector2.ClampMagnitude(_velocity, _maxVelocity);
        }

        public void Decelerate()
        {
            _velocity -= _angle.normalized * _acceleration;

            _velocity = Vector2.ClampMagnitude(_velocity, _maxVelocity);
        }

        public void RotateLeft()
        {
            _angle = Quaternion.Euler(0, 0, _rotation) * _angle;
        }

        public void RotateRight()
        {
            _angle = Quaternion.Euler(0, 0, -_rotation) * _angle;
        }
    }
}
