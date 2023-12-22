using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Views
{
    public class AsteroidsView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _asteroidPrefab;

        private Camera _camera;

        private int _halfWidth;
        private int _halfHeight;

        private AsteroidsController AsteroidsController => GameView.AsteroidsController;
        private ShipController ShipController => GameView.ShipController;

        private Dictionary<int, GameObject> _asteroids;
        public IReadOnlyDictionary<int, GameObject> Asteroids => _asteroids;

        private IObjectPool<GameObject> _pool;

        private List<int> _updatedInFrame;

        public int GetId(GameObject go)
        {
            var keyValuePair = _asteroids.FirstOrDefault(a => a.Value == go);

            if (keyValuePair.Value == null)
            {
                return -1;
            }

            return keyValuePair.Key;
        }

        private void OnEnable()
        {
            _camera = Camera.main;

            Assert.IsNotNull(_camera, "Main camera is not found.");

            _halfWidth = (int)Mathf.Ceil(_camera.orthographicSize * _camera.aspect) + 1;
            _halfHeight = (int)Mathf.Ceil(_camera.orthographicSize) + 1;

            _asteroids = new Dictionary<int, GameObject>();

            _pool = new ObjectPool<GameObject>(() => Instantiate(_asteroidPrefab, transform),
                go => go.SetActive(true),
                go => go.SetActive(false),
                go => Destroy(go),
                true,
                100
            );

            _updatedInFrame = new List<int>();
        }

        private void OnDisable()
        {
            foreach (GameObject go in _asteroids.Values)
            {
                _pool.Release(go);
            }

            _asteroids.Clear();
            _pool.Clear();
        }

        private void Update()
        {
            ShipController.Update(Time.deltaTime);

            var shipPositionX = (int)ShipController.Position.x;
            var shipPositionY = (int)ShipController.Position.y;

            int xStart = shipPositionX - _halfWidth;
            int yStart = shipPositionY - _halfHeight;
            int xEnd = shipPositionX + _halfWidth;
            int yEnd = shipPositionY + _halfHeight;

            AsteroidsController.Update(Time.deltaTime, xStart, yStart, xEnd, yEnd);

            _updatedInFrame.Clear();

            UpdateAsteroids(xStart, yStart, xEnd, yEnd);
            RemoveNotUpdated();
        }

        private void UpdateAsteroids(int xStart, int yStart, int xEnd, int yEnd)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    var updated = UpdateCell(new Vector2Int(x, y));
                    _updatedInFrame.AddRange(updated);
                }
            }
        }

        private void RemoveNotUpdated()
        {
            var toRemove = _asteroids.Keys.Except(_updatedInFrame);

            foreach (int id in toRemove.ToArray())
            {
                GameObject go = _asteroids[id];
                _pool.Release(go);
                _asteroids.Remove(id);
            }
        }

        private IEnumerable<int> UpdateCell(Vector2Int cellPosition)
        {
            int[] asteroids = AsteroidsController.GetAsteroidIdsInCell(cellPosition);
            foreach (int id in asteroids)
            {
                if (id == -1)
                {
                    break;
                }

                Vector2 localPosition = AsteroidsController.GetAsteroidLocalPosition(id);
                Vector2 scenePosition = cellPosition + localPosition - ShipController.Position;

                if (_asteroids.TryGetValue(id, out GameObject go))
                {
                    go.transform.position = scenePosition;
                }
                else
                {
                    CreateAsteroid(id, scenePosition);
                }

                yield return id;
            }
        }

        private void CreateAsteroid(int id, Vector2 scenePosition)
        {
            GameObject go = _pool.Get();
            go.transform.position = scenePosition;

            _asteroids.Add(id, go);
        }
    }
}
