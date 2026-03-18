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
        [SerializeField] private Player _vehicle;

        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _maxMoveSpeed = 30f;
        [SerializeField] private float _speedIncreaseRate = 2f;
        [SerializeField] private float _levelLength = 300f;

        private float _distanceTravelled;
        private float _currentSpeed;
        private int _chunksSpawned;
        private LevelState _currentState = LevelState.Idle;

        public float Progress => Mathf.Clamp01(_distanceTravelled / _levelLength);

        private void Start()
        {
            InitChunks();
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
                case ButtonEvent.RestartGame:
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

            IncreaseSpeed();
            MoveChunks();
            CheckStartChunk();
            CheckChunkRecycle();
            CheckLevelComplete();
        }

        private void InitChunks()
        {
            _startChunk.Init(_vehicle);
            foreach (var chunk in _chunks)
                chunk.Init(_vehicle);
        }

        private void InitLevel()
        {
            _distanceTravelled = 0f;
            _currentSpeed = _moveSpeed;
            _chunksSpawned = 0;
            PlaceChunks();
            SetState(LevelState.Playing);
        }

        private void PlaceChunks()
        {
            _startChunk.PlaceAt(0f, 0);

            float nextZ = _chunkLength;
            foreach (var chunk in _chunks)
            {
                chunk.PlaceAt(nextZ, ++_chunksSpawned);
                nextZ += _chunkLength;
            }
        }

        private void IncreaseSpeed()
        {
            _currentSpeed = Mathf.Min(_currentSpeed + _speedIncreaseRate * Time.deltaTime, _maxMoveSpeed);
        }

        private void MoveChunks()
        {
            float delta = _currentSpeed * Time.deltaTime;
            _distanceTravelled += delta;

            if (_startChunk.IsActive)
                _startChunk.transform.position += Vector3.back * delta;

            foreach (var chunk in _chunks)
            {
                if (!chunk.IsActive) continue;
                chunk.transform.position += Vector3.back * delta;
            }
        }

        private void CheckStartChunk()
        {
            if (!_startChunk.IsActive) return;

            if (_startChunk.transform.position.z + _chunkLength < 0f)
                _startChunk.Deactivate();
        }

        private void CheckChunkRecycle()
        {
            foreach (var chunk in _chunks)
            {
                if (!chunk.IsActive) continue;

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
                if (!c.IsActive) continue;
                if (c.transform.position.z > furthestZ)
                    furthestZ = c.transform.position.z;
            }

            chunk.PlaceAt(furthestZ + _chunkLength, ++_chunksSpawned);
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
                    EventBus.Trigger(new LevelCompletedEvent(true));
                    break;
                case LevelState.Failed:
                    EventBus.Trigger(new LevelCompletedEvent(false));
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