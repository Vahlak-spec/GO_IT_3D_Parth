using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int MaxHealth => _maxHealth;
    public int Damage => _damage;
    public float Speed => _speed;
    public float DistanceToChase => _distanceToChase;
    public float DistanceToFire => _distanceToFire;
    public float ShootCooldown => _shootCooldown;

    [SerializeField] private int _maxHealth;
    [SerializeField] private int _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _distanceToChase;
    [SerializeField] private float _distanceToFire;
    [SerializeField] private float _shootCooldown;
}
