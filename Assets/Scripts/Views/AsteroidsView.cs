using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class AsteroidsView : MonoBehaviour
{
    [SerializeField] private GameObject _asteroidPrefab;

    private Camera _camera;

    private int _halfWidth;
    private int _halfHeight;

    private AsteroidsController _asteroidsController;
    private ShipModel _shipModel;

    private Dictionary<int, GameObject> _asteroids;

    private IObjectPool<GameObject> _pool;

    private List<int> _updatedInFrame;

    private void Start()
    {
        _camera = Camera.main;

        Assert.IsNotNull(_camera, "Main camera is not found.");

        _halfWidth = (int)Mathf.Ceil(_camera.orthographicSize * _camera.aspect) + 1;
        _halfHeight = (int)Mathf.Ceil(_camera.orthographicSize) + 1;

        _asteroidsController = GameContext.AsteroidsController;
        _shipModel = GameContext.ShipModel;

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

    private void Update()
    {
        _asteroidsController.Update(Time.deltaTime);
        _shipModel.Update(Time.deltaTime);

        _updatedInFrame.Clear();

        UpdateAsteroids();
        RemoveNotUpdated();
    }

    private void UpdateAsteroids()
    {
        var shipPositionX = (int)_shipModel.Position.x;
        var shipPositionY = (int)_shipModel.Position.y;

        for (int x = shipPositionX - _halfWidth; x < shipPositionX + _halfWidth; x++)
        {
            for (int y = shipPositionY - _halfHeight; y < shipPositionY + _halfHeight; y++)
            {
                var cellPosition = new Vector2Int(x, y);
                var updated = UpdateCell(cellPosition);
                _updatedInFrame.AddRange(updated);
            }
        }
    }

    private void RemoveNotUpdated()
    {
        var toRemove = _asteroids.Keys.Except(_updatedInFrame);
        foreach (int asteroid in toRemove.ToArray())
        {
            _pool.Release(_asteroids[asteroid]);
            _asteroids.Remove(asteroid);
        }
    }

    private IEnumerable<int> UpdateCell(Vector2Int cellPosition)
    {
        var asteroids = _asteroidsController.GetAsteroidIdsInCell(cellPosition);
        foreach (int index in asteroids)
        {
            Vector2 asteroidLocalPosition = _asteroidsController.AsteroidLocalPosition(index);
            Vector2 position = cellPosition + asteroidLocalPosition - _shipModel.Position;

            if (_asteroids.TryGetValue(index, out GameObject go))
            {
                go.transform.position = position;
            }
            else
            {
                GameObject instance = _pool.Get();
                instance.transform.position = position;

                var text = instance.GetComponentInChildren<TextMeshPro>();
                text.text = cellPosition.ToString();

                _asteroids.Add(index, instance);
            }

            yield return index;
        }
    }
}
