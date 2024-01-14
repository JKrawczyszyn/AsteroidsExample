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
        public static AsteroidsView AsteroidsView;
        public static ShipView ShipView;
        public static Config Config;

        public static AsteroidsController AsteroidsController;
        public static ShipController ShipController;

        private void Awake()
        {
            AsteroidsView = _asteroidsView;
            ShipView = _shipView;
            Config = _config;

            Application.targetFrameRate = 300;

            _uiView.Started += OnStarted;

            _asteroidsView.gameObject.SetActive(false);
            _shipView.gameObject.SetActive(false);
        }

        private void Start()
        {
            OnStarted();
        }

        private void OnDestroy()
        {
            _uiView.Started -= OnStarted;
        }

        private void OnStarted()
        {
            CreateControllers();
            UpdateViews(true);
        }

        private void CreateControllers()
        {
            var asteroidsModel = new AsteroidsModel(_config);
            var randomProvider = new RandomProvider(_config.GridSize.x, _config.GridSize.y);
            AsteroidsController = new AsteroidsController(asteroidsModel, _config.Parts, _config.Seed, randomProvider);
            ShipController = new ShipController(_config, _config.GridSize / 2, Vector2.zero, Vector2.up);
        }

        private void UpdateViews(bool gameMode)
        {
            _asteroidsView.gameObject.SetActive(gameMode);
            _shipView.gameObject.SetActive(gameMode);
            _uiView.SetMode(gameMode);

            if (gameMode)
                _shipView.Died += OnDied;
            else
                _shipView.Died -= OnDied;
        }

        private void OnDied()
        {
            DestroyControllers();
            UpdateViews(false);
        }

        private static void DestroyControllers()
        {
            ShipController.Dispose();
        }
    }
}
