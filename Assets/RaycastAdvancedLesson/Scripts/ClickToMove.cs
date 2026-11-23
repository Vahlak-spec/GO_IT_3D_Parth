using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Сценарій 3 — Переміщення персонажа по кліку (як у стратегіях).
    /// ПКМ по землі задає ціль, персонаж іде до неї по прямій.
    /// Доповнення демо-сцени: маркер цілі, видимий у Game View.
    /// </summary>
    public class ClickToMove : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public LayerMask groundLayer;

        [Header("Візуалізація (демо)")]
        public Transform targetMarker;

        private Vector3 targetPosition;
        private bool isMoving = false;

        public bool IsMoving => isMoving;

        void Update()
        {
            if (Input.GetMouseButtonDown(1)) // правий клік — задати ціль
            {
                SetMoveTarget();
            }

            if (isMoving)
            {
                MoveToTarget();
            }
        }

        void SetMoveTarget()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, groundLayer))
            {
                targetPosition = hit.point;
                targetPosition.y = transform.position.y; // зберігаємо висоту персонажа
                isMoving = true;

                Debug.DrawRay(hit.point, Vector3.up * 2, Color.cyan, 1f); // маркер цілі

                if (targetMarker != null)
                {
                    targetMarker.gameObject.SetActive(true);
                    targetMarker.position = hit.point + Vector3.up * 0.02f;
                }
            }
        }

        void MoveToTarget()
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget < 0.1f)
            {
                isMoving = false;
                if (targetMarker != null) targetMarker.gameObject.SetActive(false);
                return;
            }

            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Повертаємо персонажа обличчям до цілі
            transform.rotation = Quaternion.LookRotation(direction);
        }

        void OnDisable()
        {
            isMoving = false;
            if (targetMarker != null) targetMarker.gameObject.SetActive(false);
        }
    }
}
