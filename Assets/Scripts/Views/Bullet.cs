using UnityEngine;

namespace Views
{
    public class Bullet : MonoBehaviour
    {
        private float _bulletSpeed;

        private Transform _transform;

        private DelayedUpdater _destroyUpdater;

        public void Init(Config config, float angle)
        {
            _bulletSpeed = config.BulletSpeed;

            _transform = transform;
            _transform.rotation = Quaternion.Euler(0, 0, angle);

            _destroyUpdater = new DelayedUpdater();
            _destroyUpdater.Init(Destroy, config.BulletDestroyTime);
        }

        private void OnDestroy()
        {
            _destroyUpdater.Dispose();
        }

        private void Update()
        {
            _transform.position += _transform.up * Time.deltaTime * _bulletSpeed;

            _destroyUpdater.Update(Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Destroy();

            int id = GameView.AsteroidsView.GetId(collision.gameObject);
            if (id != -1)
                GameView.AsteroidsController.Destroy(id);
        }

        private void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
