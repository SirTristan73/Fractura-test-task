using System.Collections.Generic;
using UnityEngine;

namespace EventBus
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance { get; private set; }

        [SerializeField] private Bullet _prefab;
        [SerializeField] private int _initialSize = 20;

        private readonly Queue<Bullet> _pool = new();

        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < _initialSize; i++)
                CreateBullet();
        }

        private Bullet CreateBullet()
        {
            var bullet = Instantiate(_prefab, transform);
            bullet.gameObject.SetActive(false);
            _pool.Enqueue(bullet);
            return bullet;
        }

        public Bullet Get()
        {
            if (_pool.Count == 0)
                CreateBullet();

            var bullet = _pool.Dequeue();
            bullet.gameObject.SetActive(true);
            return bullet;
        }

        public void Return(Bullet bullet)
        {
            bullet.transform.SetParent(transform);
            bullet.gameObject.SetActive(false);
            _pool.Enqueue(bullet);
        }
    }
}