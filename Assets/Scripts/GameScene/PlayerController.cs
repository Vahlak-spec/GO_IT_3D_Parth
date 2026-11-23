using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int _maxHealth;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GroundTriger _groundTriger;
    [SerializeField] private GunView _gun;
    [Space]
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _weaponNameText;
    [Space]
    [SerializeField] private MeshRenderer _render;
    [Space]
    [SerializeField] private Material[] _materials;
    [Space]
    [SerializeField] private WeaponData[] _weaponDatas;

    private int _healt;

    private int _currentMaterialIndex;
    private int _currentWeaponIndex;

    void Start()
    {
        _slider.value = 1;
        _healt = _maxHealth;

        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        _currentMaterialIndex = PlayerPrefs.GetInt("SkinIndex");
        _render.material = _materials[_currentMaterialIndex];

        _currentWeaponIndex = 0;

        _gun.SetTeam(Team.Player);
        _gun.SetData(_weaponDatas[_currentWeaponIndex]);
        _weaponNameText.text = _weaponDatas[_currentWeaponIndex].WeaponName;
    }

    public void NextWeapon()
    {
        _currentWeaponIndex++;
        if (_currentWeaponIndex >= _weaponDatas.Length)
            _currentWeaponIndex = 0;
        _gun.SetData(_weaponDatas[_currentWeaponIndex]);
        _weaponNameText.text = _weaponDatas[_currentWeaponIndex].WeaponName;
    }

    public void PreviousWeapon()
    {
        _currentWeaponIndex--;
        if (_currentWeaponIndex < 0)
            _currentWeaponIndex = _weaponDatas.Length - 1;
        _gun.SetData(_weaponDatas[_currentWeaponIndex]);
        _weaponNameText.text = _weaponDatas[_currentWeaponIndex].WeaponName;
    }

    public void NextSkin()
    {
        _currentMaterialIndex++;

        if (_currentMaterialIndex >= _materials.Length)
            _currentMaterialIndex = 0;

        _render.material = _materials[_currentMaterialIndex];
    }

    public void TakeDamage(int damage)
    {
        _healt -= damage;

        _slider.value = (float)_healt / (float)_maxHealth;

        if (_healt <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            NextWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PreviousWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Space) && _groundTriger.IsGround)
        {
            _rigidbody.AddForce(Vector3.up * _jumpPower);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            _gun.Shoot();
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = Vector3.zero;

        float horisontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direction += Vector3.forward * vertical;
        direction += Vector3.right * horisontal;

        direction.Normalize();

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed * Time.deltaTime);

        direction *= speed;

        direction.y = _rigidbody.linearVelocity.y;

        _rigidbody.linearVelocity = direction;
    }
}
