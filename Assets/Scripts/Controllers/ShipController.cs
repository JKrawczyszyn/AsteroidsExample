using System;
using UnityEngine;

namespace Controllers
{
    public class ShipController : IDisposable
    {
        public event Action Shoot;

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

        private readonly DelayedUpdater _shootUpdater;

        public ShipController(Config config, Vector2 position, Vector2 velocity, Vector2 angle)
        {
            _position = position;
            _velocity = velocity;
            _angle = angle;
            _width = config.GridSize.x;
            _height = config.GridSize.y;
            _acceleration = config.Acceleration;
            _maxVelocity = config.MaxVelocity;
            _rotation = config.Rotation;

            _shootUpdater = new DelayedUpdater();
            _shootUpdater.Init(InvokeShoot, config.ShootDelay);
        }

        public void Dispose()
        {
            _shootUpdater.Dispose();
        }

        private void InvokeShoot()
        {
            Shoot?.Invoke();
        }

        public void Update(float deltaTime)
        {
            _position += _velocity * deltaTime;
            _position = _position.Wrap(_width, _height);

            _shootUpdater.Update(deltaTime);
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
