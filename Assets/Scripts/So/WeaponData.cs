using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string ModelName => _modelName;
    public string WeaponName => _weaponName;
    public int Damage => _damage;
    public GameObject BulletPrefab => _bulletPrefab;

    [SerializeField] private string _weaponName;
    [SerializeField] private string _modelName;
    [SerializeField] private int _damage;
    [SerializeField] private GameObject _bulletPrefab;
}
