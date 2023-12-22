using UnityEngine;

namespace Views
{
    public class Bullet : MonoBehaviour
    {
        private float _halfHeight;
        private float _halfWidth;

        private Transform _transform;

        public void Init(float angle)
        {
            _transform = transform;

            _transform.rotation = Quaternion.Euler(0, 0, angle);

            _halfHeight = GameView.Camera.orthographicSize;
            _halfWidth = GameView.Camera.aspect * _halfHeight;
        }

        private void Update()
        {
            if (OutsideCamera())
            {
                Destroy(gameObject);
            }

            _transform.position += _transform.up * Time.deltaTime * 5;
        }

        private bool OutsideCamera()
        {
            Vector3 position = transform.position;

            return position.x < -_halfWidth
                || position.x > _halfWidth
                || position.y < -_halfHeight
                || position.y > _halfHeight;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Destroy(gameObject);

            int id = GameView.AsteroidsView.GetId(collision.gameObject);
            if (id != -1)
            {
                GameView.AsteroidsController.Destroy(id);
            }
        }
    }
}
