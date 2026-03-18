using UnityEngine;

namespace EventBus
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private float _detectionRange = 20f;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackDamage = 10f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _knockbackForce = 5f;
        [SerializeField] private float _knockbackDuration = 0.2f;

        [Header("References")]
        [SerializeField] private Animator _animator;

        private Player _vehicle;
        private Transform _vehicleTransform;
        private EnemyState _state = EnemyState.Idle;
        private float _health;
        private float _attackTimer;
        private float _knockbackTimer;
        private Vector3 _knockbackDir;

        private static readonly int AnimState = Animator.StringToHash("State");

        private void OnEnable()
        {
            EventBus.SubscribeToEvent<LevelCompletedEvent>(OnLevelEnded);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeFromEvent<LevelCompletedEvent>(OnLevelEnded);
        }

        private void OnLevelEnded(LevelCompletedEvent e) => SetState(EnemyState.Idle);

        public void Init(Player player)
        {
            _vehicle = player;
            _vehicleTransform = player.transform;
            _health = _maxHealth;
            _knockbackTimer = 0f;
            SetState(EnemyState.Idle);
        }

        public void TakeDamage(float damage)
        {
            if (_state == EnemyState.Dead) return;

            _health -= damage;
            _knockbackDir = -_vehicleTransform.forward;
            _knockbackTimer = _knockbackDuration;

            if (_health <= 0f)
                SetState(EnemyState.Dead);
        }

        private void Update()
        {
            if (_state == EnemyState.Dead) return;

            _attackTimer -= Time.deltaTime;

            if (_knockbackTimer > 0f)
            {
                _knockbackTimer -= Time.deltaTime;
                transform.position += _knockbackDir * _knockbackForce * Time.deltaTime;
                return;
            }

            switch (_state)
            {
                case EnemyState.Idle:   CheckDetection(); break;
                case EnemyState.Chase:  Chase();          break;
                case EnemyState.Attack: Attack();         break;
            }
        }

        private void CheckDetection()
        {
            if (_vehicleTransform == null) return;

            if (DistanceToVehicle() <= _detectionRange)
                SetState(EnemyState.Chase);
        }

        private void Chase()
        {
            if (DistanceToVehicle() <= _attackRange)
            {
                SetState(EnemyState.Attack);
                return;
            }

            Vector3 dir = (_vehicleTransform.position - transform.position).normalized;
            transform.position += dir * _moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        private void Attack()
        {
            if (DistanceToVehicle() > _attackRange)
            {
                SetState(EnemyState.Chase);
                return;
            }

            if (_attackTimer <= 0f)
            {
                _attackTimer = _attackCooldown;
                _vehicle.TakeDamage(_attackDamage);
            }
        }

        private float DistanceToVehicle()
        {
            return Vector3.Distance(transform.position, _vehicleTransform.position);
        }

        private void SetState(EnemyState newState)
        {
            if (_state == newState) return;
            _state = newState;
            _animator.SetInteger(AnimState, (int)newState);

            if (newState == EnemyState.Dead)
                EnemyPool.Instance.Return(this);
        }
    }

    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead,
    }
}