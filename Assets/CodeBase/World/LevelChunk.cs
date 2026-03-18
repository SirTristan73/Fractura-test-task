using UnityEngine;

namespace EventBus
{
    public class LevelChunk : MonoBehaviour
    {
        [SerializeField] private GameObject _chunkObject;
        [SerializeField] private EnemySpawner _enemySpawner;

        public void Init(Player player)
        {
            _enemySpawner?.Init(player);
        }

        public void PlaceAt(float zPosition, int difficulty = 0)
        {
            transform.position = new Vector3(0f, 0f, zPosition);
            _chunkObject.SetActive(true);
            _enemySpawner?.Spawn(difficulty);
        }

        public void Deactivate()
        {
            _chunkObject.SetActive(false);
            _enemySpawner?.DespawnAll();
        }

        public bool IsActive => _chunkObject.activeSelf;
    }
}