using Controllers;
using Model;
using UnityEngine;

namespace Views
{
    [DefaultExecutionOrder(-1)]
    public class GameView : MonoBehaviour
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
        public static Camera Camera;
        public static AsteroidsView AsteroidsView;
        public static ShipView ShipView;

        public static AsteroidsController AsteroidsController;
        public static ShipController ShipController;

        private void Awake()
        {
            Camera = Camera.main;
            AsteroidsView = _asteroidsView;
            ShipView = _shipView;

            Application.targetFrameRate = 300;

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
            CreateControllers();
            UpdateViews(true);
        }

        private void CreateControllers()
        {
            var asteroidsModel = new AsteroidsModel(_config.GridSize.x, _config.GridSize.y, _config.TimeToSpawnSeconds,
                _config.CollisionDistance);

            AsteroidsController = new AsteroidsController(asteroidsModel, _config.Parts, _config.Seed);

            ShipController = new ShipController(_config.GridSize / 2, Vector2.zero, Vector2.up, _config.GridSize.x,
                _config.GridSize.y, _config.Acceleration, _config.MaxVelocity, _config.Rotation);
        }

        private void OnDied()
        {
            DestroyControllers();
            UpdateViews(false);
        }

        private void UpdateViews(bool gameMode)
        {
            _asteroidsView.gameObject.SetActive(gameMode);
            _shipView.gameObject.SetActive(gameMode);
            _uiView.SetStartButtonActive(!gameMode);
        }

        private static void DestroyControllers()
        {
            AsteroidsController.Dispose();
            AsteroidsController = null;

            ShipController = null;
        }
    }
}
