using UnityEngine;

public class NewplayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _smoothForce = 8f;

    private const string XAXIS = "Horizontal";
    private const string YAXIS = "Vertical";

    private Vector3 _currentDirection;
    private Vector3 _targetDirection;
    private bool _isRunning;

    float targetMoveX;
    float targetMoveZ;

    private float curentMoveX;
    private float curentMoveZ;

    private float targetHandWeight;
    private float curentHandWeight;

    private void Update()
    {
        // 1. ВВІД (Завжди в Update)
        // Використовуємо GetAxisRaw, щоб уникнути подвійного згладжування
        targetMoveX = Input.GetAxisRaw(XAXIS);
        targetMoveZ = Input.GetAxisRaw(YAXIS);

        // Використовуємо GetKey, щоб біг працював поки кнопка затиснута
        _isRunning = Input.GetKey(KeyCode.LeftShift);

        // Нормалізуємо вектор, щоб по діагоналі гравець не рухався швидше
        _targetDirection = new Vector3(targetMoveX, 0f, targetMoveZ).normalized;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            targetHandWeight = 1;
        }
        else
        {
            targetHandWeight = 0;
        }


        SmoothDirection();
        DoAnim();
    }

    private void FixedUpdate()
    {
        // 2. ФІЗИКА (Завжди в FixedUpdate)
        DoMove();
    }

    private void SmoothDirection()
    {
        // Згладжуємо напрямок руху
        _currentDirection = Vector3.Lerp(_currentDirection, _targetDirection, Time.deltaTime * _smoothForce);

        curentMoveX = Mathf.Lerp(curentMoveX, targetMoveX, Time.deltaTime * _smoothForce);
        curentMoveZ = Mathf.Lerp(curentMoveZ, targetMoveZ, Time.deltaTime * _smoothForce);

        curentHandWeight = Mathf.Lerp(curentHandWeight, targetHandWeight, Time.deltaTime * _smoothForce);
    }

    private void DoMove()
    {
        float currentSpeed;

        if (_isRunning)
        {
            currentSpeed = _runSpeed;
        }
        else
        {
            currentSpeed = _walkSpeed;
        }

        // Якщо напрямок по Z від'ємний (рух назад), зменшуємо швидкість удвічі
        if (_currentDirection.z < 0)
        {
            currentSpeed *= 0.5f;
        }

        // Задаємо швидкість напряму. Це прибирає ефект "ковзання на льоду"
        Vector3 targetVelocity = _currentDirection * currentSpeed;

        _rigidbody.linearVelocity = targetVelocity;
    }

    private void DoAnim()
    {
        // Зазвичай Z - це рух вперед/назад (Speed), а X - вліво/вправо (Strafe)
        // Додав множник для анімації бігу, якщо це підтримується твоїм Blend Tree
        float animSpeedMultiplier = _isRunning ? 2f : 1f;

        if (!_isRunning)
        {
            animator.SetFloat("X", curentMoveX / 2);
            animator.SetFloat("Y", curentMoveZ / 2);
        }
        else
        {
            animator.SetFloat("X", curentMoveX);
            animator.SetFloat("Y", curentMoveZ);
        }

        animator.SetLayerWeight(1, curentHandWeight);
    }
}
