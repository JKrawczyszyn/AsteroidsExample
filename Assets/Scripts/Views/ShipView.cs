using UnityEngine;

public class ShipView : MonoBehaviour
{
    [SerializeField] private GameObject _ship;

    private ShipController _shipController;

    private void Start()
    {
        _shipController = GameContext.ShipController;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _shipController.Accelerate();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _shipController.Decelerate();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _shipController.RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _shipController.RotateRight();
        }

        _ship.transform.rotation
            = Quaternion.Euler(0, 0, (Mathf.Atan2(_shipController.Angle.y, _shipController.Angle.x) * Mathf.Rad2Deg) - 90);
    }
}
