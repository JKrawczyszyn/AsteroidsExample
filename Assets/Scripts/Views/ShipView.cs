using System;
using UnityEngine;

public class ShipView : MonoBehaviour
{
    [SerializeField]
    private Ship _ship;

    public event Action Died
    {
        add => _ship.Died += value;
        remove => _ship.Died -= value;
    }

    private ShipController ShipController => GameController.ShipController;

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
