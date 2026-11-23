using UnityEngine;
using UnityEngine.AI;

namespace NavMeshLesson
{
    /// <summary>
    /// Обв'язка демки «Втеча з лабіринту»: ловить момент, коли ворог наздогнав гравця,
    /// перезапускає раунд і показує стан у HUD. [M] — перемкнути рівень 1 / рівень 2 у EnemyChaser.
    /// </summary>
    public class MazeGame : MonoBehaviour, IRestartable, IStatusSource
    {
        public PlayerMover player;
        public EnemyChaser enemy;
        public float catchDistance = 1.2f;

        private Vector3 playerStart;
        private int caught;
        private string lastEvent = "Тікай! WASD — рух, пробіл — стрибок";

        void Awake()
        {
            if (player != null) playerStart = player.transform.position;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M) && enemy != null)
            {
                enemy.updateEveryFrame = !enemy.updateEveryFrame;
                if (!enemy.updateEveryFrame) enemy.Restart(); // рівень 1: одна команда і все
            }

            if (player == null || enemy == null) return;

            Vector3 p = player.transform.position;
            Vector3 e = enemy.transform.position;
            float flat = Vector2.Distance(new Vector2(p.x, p.z), new Vector2(e.x, e.z));

            if (flat < catchDistance && Mathf.Abs(p.y - e.y) < 1f)
            {
                caught++;
                lastEvent = $"Спіймано! (вже {caught} раз)";
                Debug.Log("Ворог спіймав гравця");
                GetComponentInParent<DemoZone>()?.RestartZone();
            }
        }

        public void Restart()
        {
            if (player != null) player.Teleport(playerStart);
        }

        public string Status
        {
            get
            {
                string mode = enemy != null && enemy.updateEveryFrame
                    ? "Рівень 2: SetDestination в Update() — переслідує"
                    : "Рівень 1: SetDestination лише раз — іде в стару точку";
                string reach = "";
                if (enemy != null && enemy.Agent != null && enemy.Agent.hasPath &&
                    enemy.Agent.pathStatus == NavMeshPathStatus.PathPartial)
                    reach = "\nВорог не може дістатися гравця (PathPartial) — ти в зоні безпеки!";
                return $"{mode}   [M]\n{lastEvent}{reach}";
            }
        }
    }
}
