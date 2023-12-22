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
        private Button _startButton;

        public event Action Start;

        private DelayedUpdater _fpsUpdater;

        public void SetStartButtonActive(bool value)
        {
            _startButton.gameObject.SetActive(value);
        }

        private void OnEnable()
        {
            _fpsUpdater = new DelayedUpdater();
            _fpsUpdater.Init(UpdateFps, 0.3f);

            _startButton.onClick.AddListener(() => Start?.Invoke());
        }

        private void OnDisable()
        {
            _fpsUpdater.Dispose();

            _startButton.onClick.RemoveListener(() => Start?.Invoke());
        }

        private void Update()
        {
            _fpsUpdater.Update(Time.deltaTime);
        }

        private void UpdateFps()
        {
            float fps = 1f / Time.deltaTime;

            _fpsText.text = $"FPS: {Mathf.Round(fps)}";
        }
    }
}
