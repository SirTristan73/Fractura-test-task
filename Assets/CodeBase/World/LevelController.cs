using UnityEngine;

namespace EventBus
{
    public class LevelController : MonoBehaviour
    {
        [Header("Chunks")]
        [SerializeField] private LevelChunk _startChunk;
        [SerializeField] private LevelChunk[] _chunks;
        [SerializeField] private float _chunkLength = 75f;

        [Header("References")]
        [SerializeField] private Transform _vehicle;

        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _levelLength = 300f;

        private float _distanceTravelled;
        private LevelState _currentState = LevelState.Idle;

        private void Awake()
        {
            PlaceChunks();
        }

        private void OnEnable()
        {
            EventBus.SubscribeToEvent<OnClickEvent>(OnButtonClicked);
            EventBus.SubscribeToEvent<VehicleDestroyedEvent>(OnVehicleDestroyed);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeFromEvent<OnClickEvent>(OnButtonClicked);
            EventBus.UnsubscribeFromEvent<VehicleDestroyedEvent>(OnVehicleDestroyed);
        }

        private void OnButtonClicked(OnClickEvent e)
        {
            switch (e.Data)
            {
                case ButtonEvent.StartGame:
                    InitLevel();
                    break;
            }
        }

        private void OnVehicleDestroyed(VehicleDestroyedEvent e)
        {
            SetState(LevelState.Failed);
        }

        private void Update()
        {
            if (_currentState != LevelState.Playing) return;

            MoveChunks();
            CheckStartChunk();
            CheckChunkRecycle();
            CheckLevelComplete();
        }

        private void InitLevel()
        {
            _distanceTravelled = 0f;
            PlaceChunks();
            SetState(LevelState.Playing);
        }

        private void PlaceChunks()
        {
            _startChunk.PlaceAt(-_chunkLength);

            float nextZ = 0f;
            foreach (var chunk in _chunks)
            {
                chunk.PlaceAt(nextZ);
                nextZ += _chunkLength;
            }
        }

        private void MoveChunks()
        {
            float delta = _moveSpeed * Time.deltaTime;
            _distanceTravelled += delta;

            if (_startChunk.gameObject.activeSelf)
                _startChunk.transform.position += Vector3.back * delta;

            foreach (var chunk in _chunks)
            {
                if (!chunk.gameObject.activeSelf) continue;
                chunk.transform.position += Vector3.back * delta;
            }
        }

        private void CheckStartChunk()
        {
            if (!_startChunk.gameObject.activeSelf) return;

            if (_startChunk.transform.position.z + _chunkLength < 0f)
                _startChunk.Deactivate();
        }

        private void CheckChunkRecycle()
        {
            foreach (var chunk in _chunks)
            {
                if (!chunk.gameObject.activeSelf) continue;

                if (chunk.transform.position.z + _chunkLength < 0f)
                {
                    if (_distanceTravelled < _levelLength)
                        RecycleChunk(chunk);
                    else
                        chunk.Deactivate();
                }
            }
        }

        private void RecycleChunk(LevelChunk chunk)
        {
            float furthestZ = float.MinValue;

            foreach (var c in _chunks)
            {
                if (!c.gameObject.activeSelf) continue;
                if (c.transform.position.z > furthestZ)
                    furthestZ = c.transform.position.z;
            }

            chunk.PlaceAt(furthestZ + _chunkLength);
        }

        private void CheckLevelComplete()
        {
            if (_distanceTravelled >= _levelLength)
                SetState(LevelState.Completed);
        }

        private void SetState(LevelState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;

            switch (newState)
            {
                case LevelState.Completed:
                    EventBus.Trigger(new LevelCompletedEvent());
                    break;
                case LevelState.Failed:
                    
                    break;
            }
        }
    }

    public enum LevelState
    {
        Idle,
        Playing,
        Completed,
        Failed,
    }
}