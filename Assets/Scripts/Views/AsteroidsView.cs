using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class AsteroidsView : MonoBehaviour
{
    [SerializeField]
    private GameObject _asteroidPrefab;

    private Camera _camera;

    private int _halfWidth;
    private int _halfHeight;

    private AsteroidsController AsteroidsController => GameController.AsteroidsController;
    private ShipController ShipController => GameController.ShipController;

    private Dictionary<int, GameObject> _asteroids;

    private IObjectPool<GameObject> _pool;

    private List<int> _updatedInFrame;

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
        foreach (var go in _asteroids.Values)
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

        foreach (int index in toRemove.ToArray())
        {
            GameObject go = _asteroids[index];
            _pool.Release(go);
            _asteroids.Remove(index);
        }
    }

    private IEnumerable<int> UpdateCell(Vector2Int cellPosition)
    {
        int[] asteroids = AsteroidsController.GetAsteroidIdsInCell(cellPosition);
        foreach (int index in asteroids)
        {
            if (index == -1)
            {
                break;
            }

            Vector2 localPosition = AsteroidsController.GetAsteroidLocalPosition(index);
            Vector2 scenePosition = cellPosition + localPosition - ShipController.Position;

            if (_asteroids.TryGetValue(index, out GameObject go))
            {
                go.transform.position = scenePosition;
            }
            else
            {
                go = CreateAsteroid(index, scenePosition);
            }

            Vector2Int wrappedCellPosition
                = cellPosition.Wrap(AsteroidsController.CellsWidth, AsteroidsController.CellsHeight);
            UpdateText(go, index, wrappedCellPosition + localPosition);

            yield return index;
        }
    }

    private void UpdateText(GameObject go, int index, Vector2 position)
    {
        var text = go.GetComponentInChildren<TextMeshPro>();
        text.text = $"{index}\n{position}";
    }

    private GameObject CreateAsteroid(int index, Vector2 scenePosition)
    {
        GameObject go = _pool.Get();
        go.transform.position = scenePosition;

        _asteroids.Add(index, go);

        return go;
    }
}
