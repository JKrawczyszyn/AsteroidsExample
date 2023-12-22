using System;
using Controllers;
using UnityEngine;

namespace Views
{
    public class ShipView : MonoBehaviour
    {
        [SerializeField]
        private Ship _ship;

        public event Action Died
        {
            add => _ship.Died += value;
            remove => _ship.Died -= value;
        }

        private ShipController ShipController => GameView.ShipController;

        private void OnEnable()
        {
            ShipController.Shoot += _ship.Shoot;
        }

        private void OnDisable()
        {
            ShipController.Shoot -= _ship.Shoot;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                ShipController.Accelerate();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                ShipController.Decelerate();
            }

            if (Input.GetKey(KeyCode.A))
            {
                ShipController.RotateLeft();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                ShipController.RotateRight();
            }

            _ship.transform.rotation
                = Quaternion.Euler(0, 0, (Mathf.Atan2(ShipController.Angle.y, ShipController.Angle.x) * Mathf.Rad2Deg) - 90);
        }
    }
}
