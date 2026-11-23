using UnityEngine;
using UnityEngine.AI;

namespace NavMeshLesson
{
    /// <summary>
    /// Допоміжна демка до міні-практики «Майстер ландшафтів» (сходинки, острови, міст):
    /// ЛКМ по поверхні — агент іде туди через SetDestination(hit.point).
    /// Це Raycast з уроків 65–66 + NavMeshAgent з цього уроку.
    /// pathStatus показує, чи досяжна точка: PathComplete / PathPartial.
    /// </summary>
    public class ClickToMoveAgent : MonoBehaviour, IRestartable, IStatusSource
    {
        public float maxDistance = 200f;
        public Transform clickMarker;

        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                {
                    agent.SetDestination(hit.point);
                    if (clickMarker != null)
                    {
                        clickMarker.gameObject.SetActive(true);
                        clickMarker.position = hit.point + Vector3.up * 0.02f;
                    }
                }
            }
        }

        public void Restart()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent.isOnNavMesh) agent.ResetPath();
            if (clickMarker != null) clickMarker.gameObject.SetActive(false);
        }

        public string Status
        {
            get
            {
                if (agent == null || !agent.hasPath) return "Клікни, куди йти";
                switch (agent.pathStatus)
                {
                    case NavMeshPathStatus.PathComplete:
                        return $"PathComplete — шлях є, лишилось {agent.remainingDistance:F1} м";
                    case NavMeshPathStatus.PathPartial:
                        return "PathPartial — точка недосяжна, агент іде до найближчого можливого місця";
                    default:
                        return "PathInvalid — шляху немає";
                }
            }
        }
    }
}
