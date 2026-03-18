using UnityEngine;

namespace EventBus
{
    public class Player : MonoBehaviour, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _laneDistance = 3f;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _sensitivity = 1f;
        [SerializeField] private float _tiltAngle = 60f;
        [SerializeField] private float _tiltSpeed = 50f;
        [SerializeField] private float _inputTimeout = 0.1f;

        [Header("References")]
        [SerializeField] private Transform _modelRoot;

        private float _health;
        private float _targetX;
        private float _currentX;
        private float _inputTimer;
        private bool _isPlaying;

        private void OnEnable()
        {
            EventBus.SubscribeToEvent<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.SubscribeToEvent<MoveInputEvent>(OnMoveInput);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeFromEvent<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.UnsubscribeFromEvent<MoveInputEvent>(OnMoveInput);
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _isPlaying = e.Data == GameState.Playing;
            if (_isPlaying) Init();
        }

        private void OnMoveInput(MoveInputEvent e)
        {
            if (!_isPlaying) return;
            _inputTimer = _inputTimeout;
            _targetX = Mathf.Clamp(_targetX + e.Delta.x * _sensitivity, -_laneDistance, _laneDistance);
        }

        private void Init()
        {
            _health = _maxHealth;
            _targetX = 0f;
            _currentX = 0f;
            _inputTimer = 0f;

            Vector3 pos = transform.position;
            pos.x = 0f;
            transform.position = pos;
        }

        private void Update()
        {
            if (!_isPlaying) return;

            _inputTimer -= Time.deltaTime;

            if (_inputTimer > 0f)
                _currentX = Mathf.MoveTowards(_currentX, _targetX, _moveSpeed * Time.deltaTime);

            Vector3 pos = transform.position;
            pos.x = _currentX;
            transform.position = pos;

            float diff = _currentX - _targetX;
            bool isMoving = _inputTimer > 0f && Mathf.Abs(diff) > 0.01f;
            float tiltZ = isMoving ? Mathf.Clamp(diff, -_tiltAngle, _tiltAngle) : 0f;
            float tiltY = isMoving ? Mathf.Clamp(-diff, -_tiltAngle * 0.5f, _tiltAngle * 0.5f) : 0f;

            _modelRoot.localRotation = Quaternion.Lerp(
                _modelRoot.localRotation,
                Quaternion.Euler(0f, tiltY, tiltZ),
                _tiltSpeed * Time.deltaTime
            );
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            if (_health <= 0f)
                EventBus.Trigger(new VehicleDestroyedEvent());
        }
    }
}