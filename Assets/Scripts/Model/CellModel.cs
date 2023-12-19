using System.Collections.Generic;
using UnityEngine.Assertions;

public class CellModel
{
    private readonly List<int> _asteroids = new();
    public IEnumerable<int> Asteroids => _asteroids;

    public void AddAsteroid(int index)
    {
        Assert.IsTrue(!_asteroids.Contains(index), "Asteroid already exists in cell.");

        _asteroids.Add(index);
    }

    public void RemoveAsteroid(int index)
    {
        _asteroids.Remove(index);
    }
}
