using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Сценарій 2 — ШІ перевіряє, чи бачить гравця.
    /// Промінь від ворога до гравця враховує лише шар перешкод: якщо в щось влучили — огляд перекрито.
    /// Доповнення демо-сцени: лінія огляду в Game View, колір ворога і лог тільки при зміні стану.
    /// </summary>
    public class EnemyVision : MonoBehaviour
    {
        public Transform player;
        public float visionRange = 15f;
        public LayerMask obstacleLayer; // стіни і перешкоди

        [Header("Візуалізація (демо)")]
        public RayLine sightLine;
        public Renderer body;
        public Color idleColor = new Color(0.6f, 0.6f, 0.6f);
        public Color alertColor = new Color(0.95f, 0.25f, 0.2f);

        public bool SeesPlayer { get; private set; }
        public string Reason { get; private set; } = "—";

        bool CanSeePlayer()
        {
            if (player == null) return false;

            Vector3 directionToPlayer = player.position - transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Гравець занадто далеко
            if (distanceToPlayer > visionRange)
            {
                Reason = $"Занадто далеко: {distanceToPlayer:F1} м > {visionRange} м";
                if (sightLine != null) sightLine.Hide();
                return false;
            }

            // Кидаємо промінь до гравця
            Ray ray = new Ray(transform.position, directionToPlayer.normalized);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distanceToPlayer, obstacleLayer))
            {
                // Щось перекриває огляд
                Debug.DrawLine(transform.position, hit.point, Color.red);
                if (sightLine != null) sightLine.Show(transform.position, hit.point, Color.red);
                Reason = "Огляд перекриває: " + hit.transform.name;
                return false;
            }

            // Нічого не перекриває — ворог бачить гравця
            Debug.DrawLine(transform.position, player.position, Color.green);
            if (sightLine != null) sightLine.Show(transform.position, player.position, Color.green);
            Reason = $"Бачить гравця, відстань {distanceToPlayer:F1} м";
            return true;
        }

        void Update()
        {
            bool sees = CanSeePlayer();

            if (sees != SeesPlayer)
            {
                SeesPlayer = sees;
                // В уроці лог іде кожен кадр — тут лише в момент зміни
                Debug.Log(sees ? "Ворог бачить гравця!" : "Ворог втратив гравця з поля зору");
                if (body != null) body.material.color = sees ? alertColor : idleColor;
                // тут логіка переслідування
            }
        }

        void OnDisable()
        {
            SeesPlayer = false;
            if (body != null) body.material.color = idleColor;
            if (sightLine != null) sightLine.Hide();
        }
    }
}
