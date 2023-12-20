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
    private ShipController _shipController;

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
        _shipController = GameContext.ShipController;

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
        _shipController.Update(Time.deltaTime);

        _updatedInFrame.Clear();

        UpdateAsteroids();
        RemoveNotUpdated();
    }

    private void FixedUpdate()
    {
        _asteroidsController.FixedUpdate();
    }

    private void UpdateAsteroids()
    {
        var shipPositionX = (int)_shipController.Position.x;
        var shipPositionY = (int)_shipController.Position.y;

        for (int x = shipPositionX - _halfWidth; x < shipPositionX + _halfWidth; x++)
        {
            for (int y = shipPositionY - _halfHeight; y < shipPositionY + _halfHeight; y++)
            {
                var updated = UpdateCell(new Vector2Int(x, y));
                _updatedInFrame.AddRange(updated);
            }
        }
    }

    private void RemoveNotUpdated()
    {
        var toRemove = _asteroids.Keys.Except(_updatedInFrame);
        foreach (int index in toRemove.ToArray())
        {
            GameObject go = _asteroids[index];
            _pool.Release(go);
            _asteroids.Remove(index);
        }
    }

    private IEnumerable<int> UpdateCell(Vector2Int cellPosition)
    {
        int[] asteroids = _asteroidsController.GetAsteroidIdsInCell(cellPosition);
        foreach (int index in asteroids)
        {
            if (index == -1)
            {
                break;
            }

            Vector2 localPosition = _asteroidsController.GetAsteroidLocalPosition(index);
            Vector2 scenePosition = cellPosition + localPosition - _shipController.Position;

            if (_asteroids.TryGetValue(index, out GameObject go))
            {
                go.transform.position = scenePosition;
            }
            else
            {
                CreateAsteroid(index, scenePosition, cellPosition);
            }

            yield return index;
        }
    }

    private void CreateAsteroid(int index, Vector2 scenePosition, Vector2Int cellPosition)
    {
        GameObject instance = _pool.Get();
        instance.transform.position = scenePosition;

        var text = instance.GetComponentInChildren<TextMeshPro>();
        text.text = cellPosition.ToString();

        _asteroids.Add(index, instance);
    }
}
