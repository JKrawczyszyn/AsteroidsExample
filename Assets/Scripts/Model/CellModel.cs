using System;

namespace Model
{
    public class CellModel
    {
        public readonly int[] Asteroids = new int[20];

        private int _cursor;

        public CellModel()
        {
            for (var i = 0; i < Asteroids.Length; i++)
                Asteroids[i] = -1;
        }

        public void AddAsteroid(int id)
        {
            // Assert.IsTrue(!Asteroids.Contains(id), "Asteroid already exists in cell.");
            // Assert.IsTrue(_cursor < Asteroids.Length, "Cell is full.");

            Asteroids[_cursor++] = id;
        }

        public void RemoveAsteroid(int id)
        {
            // Assert.IsTrue(Asteroids.Contains(id), "Asteroid doesn't exist in cell.");
            // Assert.IsTrue(_cursor > 0, "Cell is empty.");

            int idInCell = Array.IndexOf(Asteroids, id);

            _cursor--;

            Asteroids[idInCell] = Asteroids[_cursor];
            Asteroids[_cursor] = -1;
        }
    }
}
