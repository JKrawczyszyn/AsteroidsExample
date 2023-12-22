using System;
using UnityEngine;

namespace Views
{
    public class Ship : MonoBehaviour
    {
        [SerializeField]
        private Bullet _bulletPrefab;

        public event Action Died;

        public void Shoot()
        {
            Bullet bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity, GameView.ShipView.transform);
            bullet.Init(GameView.Config, transform.rotation.eulerAngles.z);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Died?.Invoke();
        }
    }
}
