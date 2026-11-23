using NUnit.Framework;
using UnityEngine;

public class GunView : MonoBehaviour
{
    [SerializeField] private GameObject[] _weaponObjects;

    public Transform gunPoint_1;
    public Transform gunPoint_2;

    private int _damage;
    private GameObject _bulletPrefab;
    private Team _team;

    public void SetTeam(Team team)
    {
        _team = team;
    }

    public void SetData(WeaponData weaponData)
    {
        _bulletPrefab = weaponData.BulletPrefab;
        _damage = weaponData.Damage;

        foreach (var weaponObject in _weaponObjects)
        {
            if(weaponObject.name == weaponData.ModelName)
                weaponObject.SetActive(true);
            else
                weaponObject.SetActive(false);
        }
    }

    public void Shoot()
    {
        Bullet bulletGO = Instantiate(_bulletPrefab, gunPoint_1.transform.position, Quaternion.identity).GetComponent<Bullet>();

        Vector3 direction = gunPoint_2.position - gunPoint_1.position;
        direction.Normalize();

        bulletGO.Launch(direction, _damage, _team);
    }

    public void Shoot(Vector3 shootTarget)
    {



        Bullet bulletGO = Instantiate(_bulletPrefab, gunPoint_1.transform.position, Quaternion.identity).GetComponent<Bullet>();
        
        Vector3 direction = shootTarget - gunPoint_1.position;
        direction.Normalize();

        bulletGO.Launch(direction, _damage, _team);
    }
}
