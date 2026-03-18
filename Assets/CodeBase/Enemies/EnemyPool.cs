using System.Collections.Generic;
using UnityEngine;

namespace EventBus
{
    public class EnemyPool : MonoBehaviour
    {
        public static EnemyPool Instance { get; private set; }

        [SerializeField] private Enemy _prefab;
        [SerializeField] private int _initialSize = 20;

        private readonly Queue<Enemy> _pool = new();

        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < _initialSize; i++)
                CreateEnemy();
        }

        private Enemy CreateEnemy()
        {
            var enemy = Instantiate(_prefab, transform);
            enemy.gameObject.SetActive(false);
            _pool.Enqueue(enemy);
            return enemy;
        }

        public Enemy Get()
        {
            if (_pool.Count == 0)
                CreateEnemy();

            var enemy = _pool.Dequeue();
            enemy.transform.SetParent(null);
            enemy.gameObject.SetActive(true);
            return enemy;
        }

        public void Return(Enemy enemy)
        {
            enemy.transform.SetParent(transform);
            enemy.gameObject.SetActive(false);
            _pool.Enqueue(enemy);
        }
    }
}