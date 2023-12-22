using System;
using UnityEngine;

namespace Views
{
    public class Ship : MonoBehaviour
    {
        [SerializeField]
        private Bullet _bulletPrefab;

        public event Action Died;

        private DelayedUpdater _delayedUpdater;

        private void Start()
        {
            _delayedUpdater = new DelayedUpdater();
            _delayedUpdater.Init(Shoot, 0.2f);
        }

        private void Update()
        {
            _delayedUpdater.Update();
        }

        private void Shoot()
        {
            Bullet bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            bullet.Init(transform.rotation.eulerAngles.z);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Died?.Invoke();
        }
    }
}
