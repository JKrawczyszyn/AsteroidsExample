using TMPro;
using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _fpsText;

    private void Update()
    {
        float fps = Mathf.Round(1f / Time.deltaTime);

        _fpsText.text = $"FPS: {fps}";
    }
}
