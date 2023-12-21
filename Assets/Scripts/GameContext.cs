using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameContext : MonoBehaviour
{
    [SerializeField]
    private Config _config;

    // Static fields are used for simplicity.
    public static AsteroidsController AsteroidsController;
    public static ShipController ShipController;

    private void Awake()
    {
        Application.targetFrameRate = -1;

        var asteroidsModel = new AsteroidsModel(_config.GridSize.x, _config.GridSize.y, _config.TimeToSpawnSeconds, _config.CollisionDistance);
        AsteroidsController = new AsteroidsController(asteroidsModel, _config.Parts, _config.Seed);

        ShipController = new ShipController(_config.GridSize / 2, Vector2.zero, Vector2.up, _config.GridSize.x, _config.GridSize.y);
    }

    private void OnDestroy()
    {
        AsteroidsController.Dispose();
    }
}
