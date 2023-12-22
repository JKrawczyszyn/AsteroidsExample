using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _fpsText;

    [SerializeField]
    private Button _startButton;

    public event Action Start;

    private void OnEnable()
    {
        _startButton.onClick.AddListener(() => Start?.Invoke());
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(() => Start?.Invoke());
    }

    private void Update()
    {
        float fps = Mathf.Round(1f / Time.deltaTime);

        _fpsText.text = $"FPS: {fps}";
    }

    public void HideButton()
    {
        _startButton.gameObject.SetActive(false);
    }

    public void ShowButton()
    {
        _startButton.gameObject.SetActive(true);
    }
}
