using UnityEngine;

public class ShipView : MonoBehaviour
{
    [SerializeField] private GameObject _ship;

    private ShipModel _shipModel;

    private void Start()
    {
        _shipModel = GameContext.ShipModel;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _shipModel.Accelerate();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _shipModel.Decelerate();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _shipModel.RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _shipModel.RotateRight();
        }

        _ship.transform.rotation
            = Quaternion.Euler(0, 0, (Mathf.Atan2(_shipModel.Angle.y, _shipModel.Angle.x) * Mathf.Rad2Deg) - 90);
    }
}
