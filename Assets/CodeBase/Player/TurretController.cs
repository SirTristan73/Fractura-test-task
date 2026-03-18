using UnityEngine;

namespace EventBus
{
    public class TurretController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _detectAngle = 40f;      
        [SerializeField] private float _loseAngle = 55f;        
        [SerializeField] private float _fireRate = 0.5f;
        [SerializeField] private float _bulletDamage = 25f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _detectionRange = 25f;   

        [Header("References")]
        [SerializeField] private Transform _turretHead;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private ParticleSystem _fireEffect;    
        [SerializeField] private ParticleSystem _hitEffectPrefab; 
        private Enemy _currentTarget;
        private float _fireTimer;
        private float _findTimer;
        private bool _isPlaying;

        private enum TurretState { Searching, Shooting }
        private TurretState _state = TurretState.Searching;

        private void OnEnable()
        {
            EventBus.SubscribeToEvent<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeFromEvent<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _isPlaying = e.Data == GameState.Playing;

            if (!_isPlaying)
            {
                _currentTarget = null;
                _state = TurretState.Searching;
            }
        }

        private void Update()
        {
            if (!_isPlaying) return;

            _fireTimer -= Time.deltaTime;
            _findTimer -= Time.deltaTime;

            switch (_state)
            {
                case TurretState.Searching:
                    if (_findTimer <= 0f)
                    {
                        _findTimer = _fireRate; 
                        FindTarget();
                    }

                    if (_currentTarget != null)
                        _state = TurretState.Shooting;
                    break;

                case TurretState.Shooting:
                    if (!IsTargetValid())
                    {
                        DropTarget();
                        break;
                    }

                    RotateToTarget();
                    TryFire();
                    break;
            }
        }

        private void FindTarget()
        {
            _currentTarget = null;
            float bestScore = float.MaxValue;

            Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRange);

            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent<Enemy>(out Enemy enemy)) continue;
                if (!enemy.gameObject.activeInHierarchy) continue;

                float angle = GetAngleToTarget(enemy.transform.position);
                if (angle > _detectAngle) continue;

                if (angle < bestScore)
                {
                    bestScore = angle;
                    _currentTarget = enemy;
                }
            }
        }

        private bool IsTargetValid()
        {
            if (_currentTarget == null) return false;
            if (!_currentTarget.gameObject.activeInHierarchy) return false;

            float angle = GetAngleToTarget(_currentTarget.transform.position);
            if (angle > _loseAngle)
                return false;

            return true;
        }

        private void DropTarget()
        {
            _currentTarget = null;
            _state = TurretState.Searching;
        }

        private void RotateToTarget()
        {
            if (_currentTarget == null) return;

            Vector3 dir = GetFlatDirection(_currentTarget.transform.position);
            if (dir == Vector3.zero) return;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            float baseY = transform.eulerAngles.y;
            float targetY = targetRot.eulerAngles.y;

            float localY = targetY - baseY;
            if (localY > 180f) localY -= 360f;

            localY = Mathf.Clamp(localY, -_detectAngle, _detectAngle);

            Quaternion finalRot = Quaternion.Euler(
                _turretHead.eulerAngles.x,
                baseY + localY,
                _turretHead.eulerAngles.z
            );

            _turretHead.rotation = Quaternion.Lerp(
                _turretHead.rotation,
                finalRot,
                _rotationSpeed * Time.deltaTime
            );
        }

        private void TryFire()
        {
            if (_currentTarget == null) return;

            float aimAngle = GetAngleFromHead(_currentTarget.transform.position);

            if (aimAngle > 10f) return; 

            if (_fireTimer <= 0f)
            {
                _fireTimer = _fireRate;

                if (_fireEffect != null)
                    _fireEffect.Play();

                _currentTarget.TakeDamage(_bulletDamage);

                if (_hitEffectPrefab != null)
                {
                    ParticleSystem hit = Instantiate(_hitEffectPrefab, _currentTarget.transform.position, Quaternion.identity);
                    hit.Play();
                    Destroy(hit.gameObject, hit.main.duration + 0.1f);
                }
            }
        }

        private float GetAngleToTarget(Vector3 targetPos)
        {
            Vector3 dir = GetFlatDirection(targetPos);
            Vector3 forward = transform.forward;
            forward.y = 0;
            return Vector3.Angle(forward, dir);
        }

        private float GetAngleFromHead(Vector3 targetPos)
        {
            Vector3 dir = GetFlatDirection(targetPos);
            Vector3 forward = _turretHead.forward;
            forward.y = 0;
            return Vector3.Angle(forward, dir);
        }

        private Vector3 GetFlatDirection(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            dir.y = 0;
            return dir.normalized;
        }
    }
}