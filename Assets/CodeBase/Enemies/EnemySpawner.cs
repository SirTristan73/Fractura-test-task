using System.Collections.Generic;
using UnityEngine;

namespace EventBus
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnRoot;
        [SerializeField] private int _enemiesPerDifficultyStep = 2;

        private Player _player;
        private readonly List<Enemy> _spawnedEnemies = new();
        private Transform[] _spawnPoints;

        public void Init(Player player)
        {
            _player = player;

            if (_spawnPoints != null) return;

            int childCount = _spawnRoot.childCount;
            _spawnPoints = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
                _spawnPoints[i] = _spawnRoot.GetChild(i);
        }

        public void Spawn(int difficulty)
        {
            int count = Mathf.Min(difficulty * _enemiesPerDifficultyStep, _spawnPoints.Length);

            for (int i = 0; i < count; i++)
            {
                var point = _spawnPoints[i];
                var enemy = EnemyPool.Instance.Get();
                enemy.transform.SetParent(point);
                enemy.transform.position = point.position;
                enemy.transform.rotation = point.rotation;
                enemy.Init(_player);
                _spawnedEnemies.Add(enemy);
            }
        }

        public void DespawnAll()
        {
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy.gameObject.activeSelf)
                    EnemyPool.Instance.Return(enemy);
            }
            _spawnedEnemies.Clear();
        }
    }
}