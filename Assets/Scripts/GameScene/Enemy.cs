using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Datas")]
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private WeaponData _weaponData;
    [Header("UI")]
    [SerializeField] private Slider _slider;
    [Header("Weapons")]
    [SerializeField] private GunView _gun;


    private IAstarAI _path;

    private Transform _target;

    private int _health;

    private float _timeToShoot;

    private int _maxHealth;
    private int _damage;
    private float _distanceToChase;
    private float _distanceToFire;
    private float _shootCooldown;

    private void Awake()
    {
        _path = GetComponent<IAstarAI>();

        _maxHealth = _enemyData.MaxHealth;
        _damage = _enemyData.Damage;
        _distanceToChase = _enemyData.DistanceToChase;
        _distanceToFire = _enemyData.DistanceToFire;
        _shootCooldown = _enemyData.ShootCooldown;
        _path.maxSpeed = _enemyData.Speed;

        _gun.SetData(_weaponData);
        _gun.SetTeam(Team.Monters);
    }
    private void Update()
    {
        _maxHealth = _enemyData.MaxHealth;
        _damage = _enemyData.Damage;
        _distanceToChase = _enemyData.DistanceToChase;
        _distanceToFire = _enemyData.DistanceToFire;
        _shootCooldown = _enemyData.ShootCooldown;
        _path.maxSpeed = _enemyData.Speed;
    }

    private void Start()
    {
        _health = _maxHealth;
        _slider.value = 1;

        _target = FindObjectOfType<PlayerController>().gameObject.transform;
        _timeToShoot = _shootCooldown;
    }

    private void FixedUpdate()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);
        _path.destination = _target.position;

        if (distanceToTarget <= _distanceToChase)
        {
            _path.canMove = true;
        }
        else 
        {
            _path.canMove = false;
        }

        if (distanceToTarget <= _distanceToFire)
        {
            _timeToShoot -= Time.fixedDeltaTime;

            if (_timeToShoot <= 0)
            {
                _gun.Shoot();
                _timeToShoot = _shootCooldown;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;

        _slider.value = (float)_health / (float)_maxHealth;

        if (_health <= 0)
        {
            Destroy(gameObject);

            ScoreController scoreController = FindObjectOfType<ScoreController>();
            scoreController.AddScore();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player;

        if (collision.gameObject.TryGetComponent<PlayerController>(out player))
        {
            player.TakeDamage(_damage);
        }
    }
}
