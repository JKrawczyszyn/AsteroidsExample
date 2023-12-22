using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameController : MonoBehaviour
{
    [SerializeField]
    private AsteroidsView _asteroidsView;

    [SerializeField]
    private ShipView _shipView;

    [SerializeField]
    private UIView _uiView;

    [SerializeField]
    private Config _config;

    // Static fields are used for simplicity.
    public static AsteroidsController AsteroidsController;
    public static ShipController ShipController;

    private void Awake()
    {
        Application.targetFrameRate = -1;

        _uiView.Start += OnStart;
        _shipView.Died += OnDied;

        _asteroidsView.gameObject.SetActive(false);
        _shipView.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _uiView.Start -= OnStart;
        _shipView.Died -= OnDied;
    }

    private void OnStart()
    {
        var asteroidsModel = new AsteroidsModel(_config.GridSize.x, _config.GridSize.y, _config.TimeToSpawnSeconds,
            _config.CollisionDistance);

        AsteroidsController = new AsteroidsController(asteroidsModel, _config.Parts, _config.Seed);

        ShipController = new ShipController(_config.GridSize / 2, Vector2.zero, Vector2.up, _config.GridSize.x,
            _config.GridSize.y, _config.Acceleration, _config.MaxVelocity, _config.Rotation);

        _asteroidsView.gameObject.SetActive(true);
        _shipView.gameObject.SetActive(true);
        _uiView.HideButton();
    }

    private void OnDied()
    {
        AsteroidsController.Dispose();
        AsteroidsController = null;

        ShipController = null;

        _asteroidsView.gameObject.SetActive(false);
        _shipView.gameObject.SetActive(false);
        _uiView.ShowButton();
    }
}
