using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody _body;
    [SerializeField] private int _damage;
    [SerializeField] private float _lifeTime;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    public void Launch(Vector3 startDirection)
    {
        _body.AddForce(speed * startDirection, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.TakeDamage(_damage);
        }
    }
}
