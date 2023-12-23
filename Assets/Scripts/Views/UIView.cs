using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class UIView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _fpsText;

        [SerializeField]
        private TextMeshProUGUI _scoreText;

        [SerializeField]
        private GameObject _lostPanel;

        [SerializeField]
        private Button _startButton;

        public event Action Started;

        private DelayedUpdater _fpsUpdater;

        private void Awake()
        {
            _fpsUpdater = new DelayedUpdater();
            _fpsUpdater.Init(UpdateFps, 0.3f);

            _startButton.onClick.AddListener(() => Started?.Invoke());
        }

        private void OnDestroy()
        {
            _fpsUpdater.Dispose();

            _startButton.onClick.RemoveListener(() => Started?.Invoke());
        }

        public void SetMode(bool gameMode)
        {
            _lostPanel.SetActive(!gameMode);

            if (gameMode)
            {
                _scoreText.text = $"SCORE: {GameView.AsteroidsController.DestroyedCount}";

                GameView.AsteroidsController.Destroyed += OnAsteroidDestroyed;
            }
            else
            {
                GameView.AsteroidsController.Destroyed -= OnAsteroidDestroyed;
            }
        }

        private void Update()
        {
            _fpsUpdater.Update(Time.deltaTime);
        }

        private void OnAsteroidDestroyed()
        {
            _scoreText.text = $"SCORE: {GameView.AsteroidsController.DestroyedCount}";
        }

        private void UpdateFps()
        {
            float fps = 1f / Time.deltaTime;

            _fpsText.text = $"FPS: {Mathf.Round(fps)}";
        }
    }
}
