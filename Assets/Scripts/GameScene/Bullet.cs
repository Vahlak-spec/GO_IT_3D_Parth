using UnityEngine;
using static Bullet;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody _body;
    [SerializeField] private float _lifeTime;

    private int _damage;
    private Team _team;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    public void Launch(Vector3 startDirection, int damage, Team team)
    {
        _damage = damage;
        _team = team;
        _body.AddForce(speed * startDirection, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collision)
    {
        switch (_team)
        {
            case Team.Player:
                if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(_damage);
                    Destroy(gameObject);
                }
                break;
            case Team.Monters:
                if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
                {
                    player.TakeDamage(_damage);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
public enum Team
{
    Player,
    Monters
}
