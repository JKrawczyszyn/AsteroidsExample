using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Views
{
    public class DebugView : MonoBehaviour
    {
#if UNITY_EDITOR
        private GUIStyle _guiStyle;

        private void Start()
        {
            _guiStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = Color.white
                }
            };
        }

        private void OnDrawGizmos()
        {
            var asteroids = GameView.AsteroidsView.Asteroids;

            if (asteroids == null)
                return;

            foreach ((int index, GameObject go) in asteroids)
            {
                var position = go.transform.position;

                var localPosition = GameView.AsteroidsController.GetAsteroidLocalPosition(index);
                var cellPosition = GameView.AsteroidsController.GetAsteroidCellPosition(index);
                var text = $"{index}\n{cellPosition.x + localPosition.x:F2}, {cellPosition.y + localPosition.y:F2}";

                Handles.Label(position, text, _guiStyle);
            }
        }
#endif
    }
}
